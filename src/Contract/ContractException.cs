using System;
using System.Runtime.Serialization;

namespace src.Contract
{
    [Serializable]
    public class ContractException : Exception
    {
        public ContractException()
        {
        }

        public ContractException(string message) : base(message)
        {
        }

        public ContractException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ContractException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}