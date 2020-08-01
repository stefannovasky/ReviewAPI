using System;

namespace ReviewApi.Domain.Exceptions
{
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException(string alreadyExistsResourceName) : base($"{alreadyExistsResourceName} already exists.")
        {

        }
    }
}
