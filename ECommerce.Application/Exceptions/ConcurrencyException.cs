using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Exceptions
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(string message, Exception? inner = null)
            : base(message, inner) { }
    }
}
