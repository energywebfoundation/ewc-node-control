using System;
using System.Runtime.Serialization;

namespace tests
{
    [Serializable]
    public class TestFailureException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public TestFailureException()
        {
        }

        public TestFailureException(string message) : base(message)
        {
        }

        public TestFailureException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TestFailureException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}