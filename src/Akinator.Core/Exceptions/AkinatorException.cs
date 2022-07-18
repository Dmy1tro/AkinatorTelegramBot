using System;

namespace Akinator.Core.Exceptions
{
    public class AkinatorException : Exception
    {
        public AkinatorException() : base()
        {

        }

        public AkinatorException(string message) : base(message)
        {

        }
    }
}
