using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BitcoinApi.Database
{
    public class User
    {
        [Required]
        public int Id { get; set; }

        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
        [Required] public byte[] Salt { get; set; }
    }
}