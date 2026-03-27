using System;
using System.Collections.Generic;
using System.Text;

namespace HelpDesk.Domain.Exceptions
{
    internal class UnauthorizedDomainException : Exception
    {
        public UnauthorizedDomainException(string message) : base(message)
        {
            
        }
    }
}
