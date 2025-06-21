using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Printing.OptionDetails;

namespace SensorProcessorWpf.Models
{
    /**
     * Data structure for storing User credentials.
     */
    public class UserCredentials
    {
        /**
         * Username belonging to the user.
         */
        public string Username { get; set; }

        /**
         * Unique Id of the user.
         */
        public string UserId { get; set; }

        /**
         * Flag indicating whether the user credentials are accepted.
         */
        public bool Accepted { get; set; }


        /**
         * Constructor
         */
        public UserCredentials()
        {
        }
    }
}
