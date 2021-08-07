using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IBMSPaymentGateway.Models
{
    public class Gw
    {
        public string visa { get; set; }
        public string master { get; set; }
        public string amex { get; set; }
        public string othercards { get; set; }
        public string internetbanking { get; set; }
        public string mobilebanking { get; set; }
    }
}
