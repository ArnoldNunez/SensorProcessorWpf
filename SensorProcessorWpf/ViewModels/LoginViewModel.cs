using ReactiveUI;
using SensorProcessorWpf.Models;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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

        private string username;

        public string Username
        {
            get => username;
            set => this.RaiseAndSetIfChanged(ref username, value);
        }

        private string password;

        public string Password
        {
            get => password;
            set => this.RaiseAndSetIfChanged(ref password, value);
        }

        private readonly ObservableAsPropertyHelper<UserCredentials> credentials;

        public UserCredentials Credentials => credentials.Value;


        /**
         * Cosntructor
         */
        public LoginViewModel(IScreen screen = null)
        {
            #region PAGE NAVIGATION

            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            #endregion

            // Get an observable from the Username property. The observable reacts
            // to changes from the Username property.
            IObservable<string> usernameObs = this.WhenAnyValue(x => x.Username);

            // When any of the username or password fields changes, update the stored
            // user credentials property.
            credentials = this.WhenAnyValue(x => x.Username, x => x.Password,
                (u, p) => 
                {
                    return new UserCredentials { Username = u, Password = p };
                })
                .ToProperty(this, x => x.Credentials);

            this.WhenAnyValue(x => x.Credentials).Subscribe(x =>
            {
                System.Diagnostics.Debug.WriteLine($"Credentials: {x.Username}:{x.Password}:{x.Accepted}");
            });
        }
    }
}
