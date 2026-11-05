using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Service
{
    public class ServiceException : Exception
    {
        public ServiceException(string message) : base(message)
        {
        }
    }
}
