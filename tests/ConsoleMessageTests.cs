using System;
using System.IO;
using FluentAssertions;
using src;
using src.Models;
using Xunit;

namespace tests
{
    public class ConsoleMessageTests
    {
        [Fact]
        public void ShouldRecordMessageToConsole()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                ConsoleMessageService cms = new ConsoleMessageService();
                cms.SendErrorMessage("test-subject", new Exception("Something went wrong").Message, new NodeState());
                sw.ToString().Should().Be($"[MSG | test-subject] Something went wrong{Environment.NewLine}");
            }
        }
    }
}