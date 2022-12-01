using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPSSL.BL.Exceptions
{
    public class OurUserFriendlyException : ApplicationException
    {
        public OurUserFriendlyException (string message) : base(message) { }
    }
}
