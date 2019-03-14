using System.Diagnostics.CodeAnalysis;
using src.Models;
using Xunit;

namespace tests
{
    /// <summary>
    /// Test the Models for correct behaviour
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LogArgsModelTests
    {
        /// <summary>
        /// Test that the models instantiates with the correct defaults
        /// </summary>
        [Theory]
        [InlineData("test")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("    ")]
        public void ShouldHaveCorrectMessage(string msg)
        {
            LogEventArgs la = new LogEventArgs(msg);
            Assert.Equal(msg, la.Message);
        }
        
    }
}