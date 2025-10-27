using System;

namespace Core.Exceptions
{
    public class ArgumentException : Exception
    {
        public ArgumentException(string message) : base(message)
        {
        }
    }
}

