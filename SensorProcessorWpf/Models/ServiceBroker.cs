using Google.Protobuf;
using Google.Protobuf.Examples.AddressBoook;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;

namespace SensorProcessorWpf.Models
{
    class ServiceBroker
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

        #region TESTING

        private static Timer _timer;

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

            _timer = new Timer(_ =>
            {
                // Read from the send queue until interrupted.
                AddressBook addressBook = new AddressBook();
                Person person = new Person
                {
                    Name = "Tyler",
                    Id = 123,
                    Email = "bangingtest@wtf.com",
                };
                //= { Seconds = DateTime.UtcNow.Second, Nanos = DateTime.UtcNow.Nanosecond }

                System.Diagnostics.Debug.WriteLine("ServiceBroker::run() - Enqueing data: " + person.ToString());
                SendData(person.ToByteArray());

            }, null, 0, 3000);

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

            _workQueue.Enqueue(workItem);
            _workQueSignaler.Set();
        }


        private void run()
        {
            _brokerThreadSignaler.WaitOne();

            using (var client = new DealerSocket())
            using (var poller = new NetMQPoller())
            {
                client.ReceiveReady += Client_ReceiveReady;
                client.Connect("tcp://127.0.0.1:5556");
                poller.RunAsync();

                while (true)
                {
                    WaitHandle.WaitAny(
                    [
                        _cancellationToken.WaitHandle,
                        _workQueSignaler
                    ]);

                    // TODO: Start critical section

                    WorkItem workItem;
                    while (!_workQueue.IsEmpty)
                    {
                        // Todo check cancellation token and break out 

                        if (_workQueue.TryDequeue(out workItem))
                        {
                            System.Diagnostics.Debug.WriteLine("ServiceBroker::run() - Sending Frame");
                            client.SendFrame(workItem.Data);
                        }
                    }

                    // Reset queue signals
                    _workQueSignaler.Reset();

                    // TODO: End critical section
                }
            }
        }

        private void Client_ReceiveReady(object? sender, NetMQSocketEventArgs e)
        {
            //e.Socket.ReceiveMultipartBytes()
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
