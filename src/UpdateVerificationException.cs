using System;
using System.Runtime.Serialization;

namespace src
{
    /// <summary>
    /// Exception that is thrown during verification of a received update
    /// </summary>
    [Serializable]
    public class UpdateVerificationException : Exception
    {
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