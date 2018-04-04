using System;
using System.IO;
using Thor.Core.Abstractions;
using Xunit;

namespace Thor.Core.Tests
{
    public class StreamExtensionsTests
    {
        [Fact(DisplayName = "SetToStart: Should throw an argument null exception for stream")]
        public void SetToStart_StreamNull()
        {
            // arrange
            Stream stream = null;

            // act
            Action validate = () => stream.SetToStart();

            // assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(validate);
            Assert.Equal("stream", exception.ParamName);
        }
        
        [Fact(DisplayName = "SetToStart: Should not throw any exception")]
        public void SSetToStart_StreamNotNull()
        {
            // arrange
            Stream stream = new MemoryStream();

            // act
            Action validate = () => stream.SetToStart();

            // assert
            Assert.Null(Record.Exception(validate));
        }

        [Fact(DisplayName = "SetToStart: Should successfully set to position 0")]
        public void SetToStart_Success()
        {
            // arrange
            Stream stream = new MemoryStream(new byte[] { 1, 255 });

            // Sets the position to position 1
            stream.ReadByte();

            // act
            stream.SetToStart();

            // assert
            Assert.Equal(0, stream.Position);
        }
    }
}