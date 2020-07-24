using System;

namespace ReviewApi.Domain.Exceptions
{
    public class UserNotConfirmedException : Exception
    {
        public UserNotConfirmedException() : base("email already registered, needs to be confirmed.") 
        {

        }
    }
}
