using Communication.Sensors;
using DynamicData.Aggregation;
using Google.Protobuf;
using Google.Protobuf.Examples.AddressBoook;
using Google.Protobuf.WellKnownTypes;
using NetMQ;
using NetMQ.Sockets;
using SensorProcessorWpf.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;

namespace SensorProcessorWpf.Models
{
    public class ServiceBroker : ISessionMessenger
    {
        /**
         * Thread that handles processing data from the server.
         */
        private Thread _brokerThread;

        /**
         * Syncronization signal for starting and stopping
         * the main broker thread
         */
        private static ManualResetEvent _brokerThreadSignaler;

        /**
         * Syncronization signal for notifying that 
         * work exists on the queue.
         */
        private static ManualResetEvent _workQueSignaler;

        /**
         * Cancellation token source used to break out of waiting for work items
         * to be pushed into the queue.
         */
        private static CancellationTokenSource _cancellationTokenSource;

        /**
         * Cancellation token for the work queue processing
         */
        private static CancellationToken _cancellationToken;

        /**
         * Queue containing messaging work that the broker has to
         * execute.
         */
        private static ConcurrentQueue<WorkItem> _workQueue;

        /**
         * Thread syncronization primitive for the work queue.
         */
        private readonly Object _workQueueLock = new object();

        /**
         * The max number of messages that a single connection will read at
         * one time. An upper limit exists to avoid starvation of connections.
         */
        private static int MAX_MSGS_PER_ITER = 1000;

        #region TESTING

        private static Timer _timer;

        #endregion

        #region SessionEvents

        public event EventHandler<CoreServices.LoginResponse> LoginResponse;

        #endregion



        /**
         * Constructor
         */
        public ServiceBroker()
        {
            _brokerThread = new Thread(this.run);
            _brokerThreadSignaler = new ManualResetEvent(false);
            _workQueSignaler = new ManualResetEvent(false);
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _workQueue = new ConcurrentQueue<WorkItem>();

            #region TESTING

            //_timer = new Timer(_ =>
            //{
            //    // Read from the send queue until interrupted.
            //    AddressBook addressBook = new AddressBook();
            //    Person person = new Person
            //    {
            //        Name = "Tyler",
            //        Id = 123,
            //        Email = "bangingtest@wtf.com",
            //    };
            //    //= { Seconds = DateTime.UtcNow.Second, Nanos = DateTime.UtcNow.Nanosecond }

            //    System.Diagnostics.Debug.WriteLine("ServiceBroker::run() - Enqueing data: " + person.ToString());
            //    SendData(person.ToByteArray());

            //}, null, 0, 3000);

            #endregion

            _brokerThread.Start();
        }

        /**
         * Method to start the broker service. This will
         * kickoff the network connection and processing of 
         * messages.
         */
        public void Start()
        {
            System.Diagnostics.Debug.WriteLine("ServiceBroker::Start()");
            // Start the message processing.
            _brokerThreadSignaler.Set();
        }

        public void SendData(byte[] data)
        {
            WorkItem workItem = new WorkItem { Data = data };

            lock(_workQueueLock)
            {
                _workQueue.Enqueue(workItem);
                _workQueSignaler.Set();
            }
        }

        public void RequestLogin(UserCredentials credentials)
        {
            // Convert to protobuf API message
            CoreServices.Command request = new CoreServices.Command
            {
                CommandId = CoreServices.CORE_COMMAND_ID.CommandIdLoginRequest,
                LoginRequest = new CoreServices.LoginRequest
                {
                    Username = credentials.Username,
                    Password = credentials.Password,
                    Timestamp = Timestamp.FromDateTime(DateTime.UnixEpoch)
                }
            };

            SendData(request.ToByteArray());
        }

        private void run()
        {
            _brokerThreadSignaler.WaitOne();

            var timer = new NetMQTimer(TimeSpan.FromSeconds(1));
            using (var clientReceiver = new PullSocket("@tcp://0.0.0.0:5558"))
            using (var clientSender = new PushSocket(">tcp://127.0.0.1:5557"))
            using (var poller = new NetMQPoller() { clientSender, clientReceiver, timer })
            {
                clientReceiver.ReceiveReady += (s, a) =>
                {
                    System.Diagnostics.Debug.WriteLine("Client ReceiveReady");

                    byte[] msg;

                    for (int count = 0; count < MAX_MSGS_PER_ITER; count++)
                    {
                        // Exit the for loop if failed to receive a message
                        if (!a.Socket.TryReceiveFrameBytes(out msg))
                        {
                            break;
                        }

                        string logStr = "ReceiveRead:: messageIn: ";
                            CoreServices.Response response = CoreServices.Response.Parser.ParseFrom(msg);
                        if (response != null)
                        {
                            switch (response.ResponseId)
                            {
                                case CoreServices.CORE_RESPONSE_ID.ResponseIdLoginResponse:
                                    logStr += "Login Response";
                                    this.LoginResponse?.Invoke(this, response.LoginResponse);
                                    break;

                                default:
                                    break;
                            }
                            System.Diagnostics.Debug.WriteLine(logStr);
                        }
                    }
                };

                timer.Elapsed += (s, a) =>
                {
                    System.Diagnostics.Debug.WriteLine("Timer elapsed...");
                };


                poller.RunAsync();

                while (!_cancellationToken.IsCancellationRequested)
                {
                    WaitHandle.WaitAny(
                    [
                        _cancellationToken.WaitHandle,
                        _workQueSignaler
                    ]);

                    lock (_workQueueLock)
                    {
                        WorkItem workItem;
                        while (!_workQueue.IsEmpty)
                        {
                            // Todo check cancellation token and break out 
                            if (_cancellationToken.IsCancellationRequested)
                            {
                                break;
                            }

                            if (_workQueue.TryDequeue(out workItem))
                            {
                                System.Diagnostics.Debug.WriteLine("ServiceBroker::run() - Sending Frame");
                                clientSender.SendFrame(workItem.Data);
                            }
                        }

                        // Reset queue signals
                        _workQueSignaler.Reset();
                    }
                }
            }
        }

        private void Timer_Elapsed(object? sender, NetMQTimerEventArgs e)
        {
            Console.WriteLine("Timer elapsed...");
        }

        private void Client_ReceiveReady(object? sender, NetMQSocketEventArgs e)
        {
            Console.WriteLine("Client ReceiveReady");
            //e.Socket.ReceiveMultipartBytes()
            byte[] msg;
            for (int count = 0; count < MAX_MSGS_PER_ITER; count++) 
            {
                if (!e.Socket.TryReceiveFrameBytes(out msg))
                {
                    break;
                }

                Console.WriteLine("RECEIVED MESSAGE FROM SERVER!");

                // Process Message
                Sensor sensorMsg = Sensor.Parser.ParseFrom(msg);
                Console.WriteLine(sensorMsg.ToString());
                System.Diagnostics.Debug.WriteLine(sensorMsg);
            } 
        }


        /**
         * A work item that the broker has to do.
         */
        private class WorkItem
        {
            /**
             * The raw data associated with the work item.
             */
            public byte[] Data { get; set; } = [];
        }
    }
}
