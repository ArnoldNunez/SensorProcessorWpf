using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorProcessorWpf.ViewModels
{
    public class LoginViewModel : ReactiveObject, IRoutableViewModel
    {
        #region PAGE NAVIGATION

        /**
         * String token representing the current view model for
         * use in routing.
         */
        public string? UrlPathSegment => "LoginPage";

        /**
         * Typically contains the instance of the host screen.
         */
        public IScreen HostScreen { get; }

        #endregion

        /**
         * Cosntructor
         */
        public LoginViewModel(IScreen screen = null)
        {
            #region PAGE NAVIGATION

            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            #endregion
        }
    }
}
