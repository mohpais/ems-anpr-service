using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Lonsum.Services.ANPR.Application.Common.Exceptions
{
    /// <summary>
    /// Exception type for domain exceptions
    /// </summary>
    public class RequestDomainException : Exception
    {
        public RequestDomainException()
        { }

        public RequestDomainException(string message)
            : base(message)
        { }

        public RequestDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
