using Google.Protobuf.Examples.AddressBoook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorProcessorWpf.Interfaces
{
    public interface ISessionMessenger
    {
        #region Events

        /**
         * Event for when the login response is received.
         */
        public event EventHandler<Person> LoginResponse;

        #endregion
    }
}
