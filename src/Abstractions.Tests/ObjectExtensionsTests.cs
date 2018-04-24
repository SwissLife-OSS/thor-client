using System;
using System.Text;
using Xunit;

namespace Thor.Core.Abstractions.Tests
{
    public class ObjectExtensionsTests
    {
        [Fact(DisplayName = "Deserialize: Should throw an argument null exception for source")]
        public void Deserialize_SourceNull()
        {
            // arrange
            byte[] source = null;

            // act
            Action validate = () => source.Deserialize<string>();

            // assert
            Assert.Throws<ArgumentNullException>("source", validate);
        }

        [Fact(DisplayName = "Deserialize: Should serialize an abitrary object into a UTF8 byte array")]
        public void Deserialize_Success()
        {
            // arrange
            string expectedDestination = "823838348";
            byte[] source = Encoding.UTF8.GetBytes($"\"{expectedDestination}\"");

            // act
            string destination = source.Deserialize<string>();

            // assert
            Assert.Equal(expectedDestination, destination);
        }

        [Fact(DisplayName = "Serialize: Should throw an argument null exception for source")]
        public void Serialize_SourceNull()
        {
            // arrange
            object source = null;

            // act
            Action validate = () => source.Serialize();

            // assert
            Assert.Throws<ArgumentNullException>("source", validate);
        }
        
        [Fact(DisplayName = "Serialize: Should serialize an object into a UTF8 byte array")]
        public void Serialize_Success()
        {
            // arrange
            object source = "3453453";

            // act
            byte[] result = source.Serialize();

            // assert
            Assert.Equal($"\"{source}\"", Encoding.UTF8.GetString(result));
        }
    }
}