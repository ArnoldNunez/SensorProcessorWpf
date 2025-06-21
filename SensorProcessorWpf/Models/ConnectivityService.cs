using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorProcessorWpf.Models
{
    /**
     * Service handling connectivity reporting for various
     * system and server components.
     */
    public class ConnectivityService : Microservice
    {
        /**
         * Constructor
         */
        public ConnectivityService(ServiceBroker serviceBroker) : base(serviceBroker)
        {
        }
    }
}
