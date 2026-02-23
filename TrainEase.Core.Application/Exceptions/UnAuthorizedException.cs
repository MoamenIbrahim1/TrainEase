using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Exceptions
{
    public class UnAuthorizedException : ApplicationException
    {
        public UnAuthorizedException(string message)
            : base(message)
        {
            
        }
    }
}
