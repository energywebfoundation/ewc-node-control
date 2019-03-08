using System;
using System.IO;
using src;
using Xunit;

namespace tests
{
    public class ConfigurationFileHandlerTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void ShouldThrowOnEmptyPath(string path)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                _ = new ConfigurationFileHandler(path);
            });
        }
        
        [Theory]
        [InlineData("/foobar")]
        [InlineData("./foobar")]
        [InlineData("some-file.foo")]
        public void ShouldThrowOnNonExistingFile(string path)
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                _ = new ConfigurationFileHandler(path);
            });
        }
    }
}