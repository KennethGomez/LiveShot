using System;
using LiveShot.API.Properties;

namespace LiveShot.API.Upload.Exceptions
{
    public class InvalidUploadTypeException : Exception
    {
        public InvalidUploadTypeException() : base(Resources.Upload_InvalidUpload_Type)
        {
        }
    }
}