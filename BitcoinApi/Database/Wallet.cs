using System.ComponentModel.DataAnnotations;

namespace BitcoinApi.Database
{
    public class Wallet
    {
        public int Id { get; set; }
        [Required]
        public string Address { get; set; }

        public decimal? Balance { get; set; }
    }
}