using System;

namespace ReviewApi.Domain.Exceptions
{
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException() : base("incorret password.")
        {

        }
    }
}
