using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTWebSite.Models
{
    public class Message
    {
        public string Body { get; set; }
        public string SignedBody { get; set; }
        public string result { get; set; }
    }
}
