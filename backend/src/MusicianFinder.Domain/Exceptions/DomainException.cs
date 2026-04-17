using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Domain.Exceptions
{
    /// <summary>
    /// Исключение, сигнализирующее о нарушении бизнес-правил домена.
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
        public DomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}
