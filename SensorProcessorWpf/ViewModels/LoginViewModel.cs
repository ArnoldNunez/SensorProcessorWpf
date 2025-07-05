using ReactiveUI;
using SensorProcessorWpf.Interfaces;
using SensorProcessorWpf.Models;
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
    public class LoginViewModel : ReactiveObject, IRoutableViewModel
    {
        #region SESSION

        /**
         * The session service handling session related tasks.
         */
        private readonly ISessionService sessionService;


        #endregion

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

        private readonly ObservableAsPropertyHelper<string> loginErrorMsg;
        public string LoginErrorMsg => loginErrorMsg.Value;


        private readonly ObservableAsPropertyHelper<UserCredentials> credentials;
        public UserCredentials Credentials => credentials.Value;


        /**
         * Command executed when user attempts to login
         */
        public ReactiveCommand<Unit, string> Login { get; }



        /**
         * Cosntructor
         */
        public LoginViewModel(IScreen screen = null, ISessionService sessionService = null)
        {
            #region DEPENDENCIES

            this.sessionService = sessionService ?? Locator.Current.GetService<ISessionService>();

            #endregion

            #region PAGE NAVIGATION

            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            #endregion

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

            var canLogin = this.WhenAnyValue(x => x.Username, x => x.Password,
                (u, p) => !string.IsNullOrWhiteSpace(u) &&  !string.IsNullOrWhiteSpace(p));
            Login = ReactiveCommand.CreateFromTask(LoginAsync, canLogin);
            Login.Subscribe(_ => { System.Diagnostics.Debug.WriteLine("Login Command Finished!"); });
            loginErrorMsg = Login.ToProperty(this, x => x.LoginErrorMsg);
        }



        /**
         * Login command work.
         */
        private async Task<string> LoginAsync()
        {
            System.Diagnostics.Debug.WriteLine("LoginViewModel::LoginAsync");

            CoreServices.LoginResponse result = await sessionService.Login(credentials.Value, 3000);

            switch (result.Result)
            {
                case CoreServices.LoginStatus.Unknown:
                    return "Unknown Error occurred";

                case CoreServices.LoginStatus.LoggedIn:
                    return $"Login Success! session: {result.SessionId}";

                case CoreServices.LoginStatus.LoggedOut:
                    return "User is logged out";

                case CoreServices.LoginStatus.Unauthorized:
                    return "Invalid Credentials!";

                case CoreServices.LoginStatus.TimedOut:
                    return "The network connection timed out";

                case CoreServices.LoginStatus.NetworkError:
                    return "A network error has occurred";

                default:
                    return "Failed to send request. Contact support.";
            }
        }
    }
}
