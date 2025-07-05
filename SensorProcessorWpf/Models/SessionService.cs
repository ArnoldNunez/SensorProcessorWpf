using SensorProcessorWpf.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;

namespace SensorProcessorWpf.Models
{
    /**
     * Session service that handles establishing and tearing down
     * a client/server session.
     */
    public class SessionService : Microservice, ISessionService
    {
        /**
         * Task completion source for when a response to a login command is received.
         */
        private TaskCompletionSource<CoreServices.LoginResponse> _loginTCS;

        /**
         * Timeout for when login should be considered to have failed. (ms)
         */
        private readonly int LOGIN_TIMEOUT = 3000;

        /**
         * Constructor.
         */
        public SessionService(ServiceBroker serviceBroker) : base(serviceBroker)
        {
            serviceBroker.LoginResponse += ServiceBroker_LoginResponse;
        }

        /**
         * Login to the server.
         */
        public Task<CoreServices.LoginResponse> Login(UserCredentials credentials, int timeout = 3000)
        {
            if (_loginTCS == null || _loginTCS.Task.IsCompleted)
            {
                System.Diagnostics.Debug.WriteLine("SessionService::Login - Running login task.");
                ServiceBroker.RequestLogin(credentials);
                _loginTCS = new TaskCompletionSource<CoreServices.LoginResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
                
                return _loginTCS.Task;
            }

            CoreServices.LoginResponse response = new CoreServices.LoginResponse
            {
                Result = CoreServices.LoginStatus.Unknown,
                SessionId = -1,
                Username = credentials.Username,
            };

            return Task.FromResult(response);
        }

        #region Service Broker Events

        /**
         * Callback for the login response.
         */
        private void ServiceBroker_LoginResponse(object? sender, CoreServices.LoginResponse e)
        {
            if (_loginTCS != null)
            {
                _loginTCS.TrySetResult(e);
            }
        }

        #endregion
    }
}
