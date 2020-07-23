using System;

namespace ReviewApi.Domain.Exceptions
{
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException(string entityName) : base($"{entityName} already exists.")
        {

        }
    }
}
