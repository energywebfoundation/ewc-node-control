using System;
using System.Runtime.Serialization;

namespace src
{
    /// <summary>
    /// Exception that is thrown by StateCompare
    /// </summary>
    [Serializable]
    public class StateCompareException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public StateCompareException(string message) : base(message)
        {
        }

        protected StateCompareException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}