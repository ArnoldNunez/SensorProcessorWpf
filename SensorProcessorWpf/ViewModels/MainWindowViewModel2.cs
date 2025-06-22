using ReactiveUI;
using SensorProcessorWpf.Views;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorProcessorWpf.ViewModels
{
    public class MainWindowViewModel2 : ReactiveObject, IScreen
    {
        #region PAGE NAVIGATION

        // The router associated with this Screen.
        // Required by the IScreen interface
        public RoutingState Router { get; }

        // The command that navigates a user to the first view model.
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToLogin { get; }

        #endregion


        /**
         * Constructor
         */
        public MainWindowViewModel2()
        {
            #region PAGE NAVIGATION

            Router = new RoutingState();

            // Router uses splat.locator to resolve views for
            // viewmodels, so we need to register our views
            // using Locator.CurrentMutable.Register* methods.
            //
            // Instead of registering views manually, you can
            // use custom IVewLocation implementation,
            // see "View Location" section for details.
            //
            Locator.CurrentMutable.Register(() => new LoginView(), typeof(IViewFor<LoginViewModel>));

            // Manage the routing state. Use the Router.Navigate.Execute
            // command to navigate to different view models.
            //
            // Note, that the Navigate.Execute method accepts an instance
            // of a view model, this allows you to pass parameters to your
            // view models, or to reuse existing view models.
            NavigateToLogin = ReactiveCommand.CreateFromObservable(() => Router.Navigate.Execute(new LoginViewModel()));

            #endregion
        }
    }
}
