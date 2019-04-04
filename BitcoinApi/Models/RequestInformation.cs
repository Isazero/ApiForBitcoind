using System.Security;

namespace BitcoinApi.Models
{
    public class RequestInformation
    {
        public string Address { get; set; }
        public decimal Amount { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}