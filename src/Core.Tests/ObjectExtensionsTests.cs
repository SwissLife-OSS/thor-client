using System;
using System.Text;
using Xunit;

namespace Thor.Core.Tests
{
    public class ObjectExtensionsTests
    {
        [Fact(DisplayName = "SerializeAttachmentContent: Should throw an argument null exception for content")]
        public void SerializeAttachmentContent_ContentNull()
        {
            // arrange
            object content = null;

            // act
            Action validate = () => content.SerializeAttachmentContent();

            // assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(validate);
            Assert.Equal("content", exception.ParamName);
        }
        
        [Fact(DisplayName = "SerializeAttachmentContent: Should dispatch an attachment")]
        public void SerializeAttachmentContent_Success()
        {
            // arrange
            object content = "3453453";

            // act
            byte[] result = content.SerializeAttachmentContent();

            // assert
            Assert.Equal($"\"{content}\"", Encoding.UTF8.GetString(result));
        }
    }
}