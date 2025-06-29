using ReactiveUI;
using SensorProcessorWpf.Interfaces;
using SensorProcessorWpf.Models;
using Splat;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Windows;

namespace SensorProcessorWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceBroker _serviceBroker;
        private readonly SessionService _sessionService;

        public App()
        {
            _serviceBroker = new ServiceBroker();
            _sessionService = new SessionService(_serviceBroker);

            initDependencyInjection();

            _serviceBroker.Start();
        }

        void initDependencyInjection()
        {
            // Register all classes that derive off IViewFor into the
            // dependency injection container.
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());

            Locator.CurrentMutable.RegisterConstant(_serviceBroker, typeof(ISessionMessenger));
            Locator.CurrentMutable.RegisterConstant(_sessionService, typeof(ISessionService));
        }
    }

}
