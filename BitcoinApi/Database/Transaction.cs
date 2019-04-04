using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitcoinApi.Database
{
    public class Transaction
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int OperationType { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        [ForeignKey("Wallet")]
        public int IdWallet { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public int Confirmations { get; set; }

        public Wallet Wallet { get; set; }
    }
}