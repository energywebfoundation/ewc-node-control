using System;
using System.Runtime.Serialization;

namespace src
{
    [Serializable]
    public class StateCompareException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public StateCompareException()
        {
        }

        public StateCompareException(string message) : base(message)
        {
        }

        public StateCompareException(string message, Exception inner) : base(message, inner)
        {
        }

        protected StateCompareException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}