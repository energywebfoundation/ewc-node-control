using System;
using System.IO;
using FluentAssertions;
using src;
using Xunit;

namespace tests
{
    public class ConsoleLoggerTests
    {
        [Fact]
        public void ShouldWriteToConsole()
        {
            StringWriter sw = new StringWriter();
            
            Console.SetOut(sw);
            ConsoleLogger cl = new ConsoleLogger();
            cl.Log("this is a message");

            string cwLog = sw.ToString();
            cwLog.Should().Be("this is a message\n");
            
        }
        
        [Fact]
        public void ShouldRecordMessageToConsole()
        {
            using (StringWriter sw = new StringWriter())
            {
                
                Console.SetOut(sw);
                ConsoleLogger cms = new ConsoleLogger();
                cms.Error("test-subject", new Exception("Something went wrong").Message);
                sw.ToString().Should().Be($"[MSG | test-subject] Something went wrong{Environment.NewLine}");
            }
        }
    }
}