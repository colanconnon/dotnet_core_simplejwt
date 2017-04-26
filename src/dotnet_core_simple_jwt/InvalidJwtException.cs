using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet_core_simple_jwt
{
    public class InvalidJwtException : Exception
    {
        public InvalidJwtException()
        {
        }

        public InvalidJwtException(string message)
            : base(message)
        {
        }

        public InvalidJwtException(string message, Exception inner)
            : base(message, inner)
        {
        }
        }
}