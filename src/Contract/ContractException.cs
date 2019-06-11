using System;
using System.Runtime.Serialization;

namespace src.Contract
{
    /// <summary>
    /// Exception that is thrown when there is an issue during contract interaction
    /// </summary>
    [Serializable]
    public class ContractException : Exception
    {
        public ContractException(string message) : base(message)
        {
        }

        protected ContractException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}