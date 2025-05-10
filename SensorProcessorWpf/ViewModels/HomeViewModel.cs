using DynamicData;
using DynamicData.Binding;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ReactiveUI;
using SensorProcessorWpf.Models;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;

namespace SensorProcessorWpf.ViewModels
{
    public class HomeViewModel : ReactiveObject, IRoutableViewModel
    {
        #region PAGE NAVIGATION

        // String token representing the current view model.
        public string UrlPathSegment => "home";

        // Typically contains the instance of the host screen
        // used by an application.
        public IScreen HostScreen { get; }

        #endregion

        #region OXYPLOT

        private readonly Timer _timer;
        private const int _maxSecondsToShow = 20;
        public PlotModel PlotModel { get; private set; }


        #endregion

        #region DYNAMIC DATA

        //private readonly ReadOnlyObservableCollection<Sensor.SensorData> _sensorData;
        //public ReadOnlyObservableCollection<Sensor.SensorData> SensorData => _sensorData;

        #endregion

        private static ServiceBroker _serviceBroker;

        /**
         * Constructor
         */
        public HomeViewModel(IScreen screen = null)
        {
            #region PAGE NAVIGATION

            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            #endregion

            #region OXYPLOT

            PlotModel = new PlotModel { Title = "Live Sensor Example 1" };
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = -1, Maximum = 1 });
            PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid });

            _timer = new Timer(OnTimerElapsed);
            _timer.Change(1000, 33);

            #endregion

            #region DYNAMIC DATA

            //var sensor = new Sensor();
            //sensor.Connect()
            //    //.Transform(data => data.X) // Project elements to other collection
            //    //.Filter(data => data.X) // Filter is like Where in LINQ.
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Bind(out _sensorData)
            //    .Subscribe();

            #endregion

            _serviceBroker = new ServiceBroker();
            _serviceBroker.Start();
        }

        /**
         * Updates the plot model data.
         */
        private void OnTimerElapsed(object state)
        {
            // https://github.com/oxyplot/oxyplot/blob/develop/Source/Examples/WPF/WpfExamples/Examples/RealtimeDemo/MainViewModel.cs
            lock (this.PlotModel.SyncRoot)
            {
                // Update points
                for (int i = 0; i < PlotModel.Series.Count; i++) 
                {
                    var s = (LineSeries)PlotModel.Series[i];

                    double x = s.Points.Count > 0 ? s.Points[s.Points.Count - 1].X + 1 : 0;
                    if (s.Points.Count >= 200)
                    {
                        s.Points.RemoveAt(0);
                    }

                    double y = 0;
                    int m = 80;
                    for (int j = 0; j < m; j++)
                    {
                        y += Math.Cos(0.001 * x * j * j);
                    }
                    y /= m;

                    s.Points.Add(new DataPoint(x, y));
                }
            }

            this.PlotModel.InvalidatePlot(true);
        }
    }

    //public class Sensor
    //{
    //    // The internal source sensor data
    //    private readonly SourceList<SensorData> _sensorData = new SourceList<SensorData>();

    //    // Expose the Connect() since we are interested in a stream of changes.
    //    public IObservable<IChangeSet<SensorData>> Connect() => _sensorData.Connect();

    //    /**
    //     * Constructor
    //     */
    //    public Sensor() 
    //    {
    //        _sensorData.Add(new SensorData { X = 0, Y = 0 });
    //        _sensorData.Add(new SensorData { X = 1, Y = 1 });
    //        _sensorData.Add(new SensorData { X = 2, Y = 1 });
    //        _sensorData.Add(new SensorData { X = 3, Y = 0 });
    //    }


    //    public class SensorData
    //    {
    //        public int X { get; set; }
    //        public int Y { get; set; }
    //    }
    //}
}
