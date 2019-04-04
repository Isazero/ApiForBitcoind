using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitcoinApi.Models
{
    //in case of errors while saving to database will return inner error to the methods
    public class Response
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }

    }
}