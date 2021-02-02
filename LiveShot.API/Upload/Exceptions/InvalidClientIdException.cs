using System;
using LiveShot.API.Properties;

namespace LiveShot.API.Upload.Exceptions
{
    public class InvalidClientIdException : Exception
    {
        public InvalidClientIdException()
            : base(Resources.Imgur_InvalidClientIdException)
        {
        }
    }
}