using System;
using System.Runtime.Serialization;

namespace src
{
    [Serializable]
    public class UpdateVerificationException : Exception
    {
        public UpdateVerificationException()
        {
        }

        public UpdateVerificationException(string message) : base(message)
        {
        }

        public UpdateVerificationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UpdateVerificationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}