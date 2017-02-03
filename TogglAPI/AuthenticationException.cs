using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TogglAPI
{
    public class AuthenticationException : Exception
    {
        public int ResposeCode { get; set; }

        public AuthenticationException(string message) : base(message) 
        {

        }

        public AuthenticationException(string message, int code) : base(message)
        {
            this.ResposeCode = code;
        }
    }
}
