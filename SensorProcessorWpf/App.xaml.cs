using ReactiveUI;
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
        public App()
        {
            // Register all classes that derive off IViewFor into the
            // dependency injection container.
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        }
    }

}
