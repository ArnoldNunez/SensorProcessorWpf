using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace SensorProcessorWpf.ViewModels
{
    public class NavigationBarViewModel : ReactiveObject
    {
        /**
         * The router associated with this Screen. Handles
         * Navigation between screens
         */
        public RoutingState Router { get; set; }

        /**
         * Command navigates user to the next page
         */
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToLogin { get; }


        /**
         * Constructor
         */
        public NavigationBarViewModel()
        {
            NavigateToLogin = ReactiveCommand
                .CreateFromObservable(() => Router.Navigate.Execute(new LoginViewModel()));
        }
    }
}
