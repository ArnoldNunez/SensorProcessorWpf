using SensorProcessorWpf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorProcessorWpf.Interfaces
{
    /**
     * Interface for a service that handle session related
     * messaging
     */
    public interface ISessionService
    {
        /**
         * Method to let the user login to the processing server.
         */
        public Task<UserCredentials> Login(UserCredentials credentials, int timeout);
    }
}
