using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorProcessorWpf.Models
{
    /**
     * Base class for microservices
     */
    public class Microservice
    {
        /**
         * Reference to a broker that handles lower level service communication concerns.
         */
        protected ServiceBroker ServiceBroker { get; }

        /**
         * Constructor
         */
        public Microservice(ServiceBroker serviceBroker)
        {
            ServiceBroker = serviceBroker;
        }
    }
}
