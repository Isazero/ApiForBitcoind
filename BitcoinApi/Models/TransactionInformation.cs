using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitcoinApi.Models
{
    public class TransactionInformation
    {
        public DateTime Date { get; set; }
        public string Address { get; set; }
        public decimal Amount { get; set; }
        public int Confirmations { get; set; }
    }
}