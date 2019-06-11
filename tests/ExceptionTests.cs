using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using src;
using src.Contract;
using Xunit;

namespace tests
{
    public class ExceptionTests
    {
        [Fact]
        public void StateCompareExceptionShouldThrowCorrectly()
        {
            StateCompareException ex = new StateCompareException("Test message");
            Action exThrow = () => throw ex;
            exThrow.Should().Throw<StateCompareException>()
                .WithMessage("Test Message");

            // should serialize
            MemoryStream mem = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();

            Action serialize = () => b.Serialize(mem, ex);
            serialize.Should().NotThrow();

            // deserialize
            mem.Seek(0, SeekOrigin.Begin);
            StateCompareException deserializedEx = null;
            Action deserialize = () => deserializedEx = (StateCompareException) b.Deserialize(mem);

            deserialize.Should().NotThrow();
            deserializedEx.Message.Should().Be(ex.Message);
        }

        [Fact]
        public void ContractExceptionShouldThrowCorrectly()
        {
            ContractException ex = new ContractException("Test message");
            Action exThrow = () => throw ex;
            exThrow.Should().Throw<ContractException>()
                .WithMessage("Test Message");

            // should serailize
            MemoryStream mem = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();

            Action serialize = () => b.Serialize(mem, ex);
            serialize.Should().NotThrow();

            // deserialize
            mem.Seek(0, SeekOrigin.Begin);
            ContractException deserializedEx = null;
            Action deserialize = () => deserializedEx = (ContractException) b.Deserialize(mem);

            deserialize.Should().NotThrow();
            deserializedEx.Message.Should().Be(ex.Message);
        }

        [Fact]
        public void UpdateVerificationExceptionShouldThrowCorrectly()
        {
            // test w/o inner
            UpdateVerificationException ex = new UpdateVerificationException("Test message");
            Action exThrow = () => throw ex;
            exThrow.Should().Throw<UpdateVerificationException>()
                .WithMessage("Test Message");

            // test with inner
            UpdateVerificationException exWithInner = new UpdateVerificationException("Test message",new Exception("this is inner"));
            Action exThrowWithInner = () => throw exWithInner;
            exThrowWithInner.Should().Throw<UpdateVerificationException>()
                .WithMessage("Test Message")
                .WithInnerException<Exception>().Which.Message.Should().Be("this is inner");


            // should serailize
            MemoryStream mem = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();

            Action serialize = () => b.Serialize(mem, ex);
            serialize.Should().NotThrow();

            // deserialize
            mem.Seek(0, SeekOrigin.Begin);
            UpdateVerificationException deserializedEx = null;
            Action deserialize = () => deserializedEx = (UpdateVerificationException) b.Deserialize(mem);

            deserialize.Should().NotThrow();
            deserializedEx.Message.Should().Be(ex.Message);
        }
    }
}