using System;
using Thor.Core.Abstractions;
using Xunit;

namespace Thor.Core.Transmission.Abstractions.Tests
{
    public class AttachmentExtensionsTests
    {
        #region IAttachment.GetTypeName
        
        [Fact(DisplayName = "GetTypeName: Should throw an argument null exception for attachment")]
        public void GetTypeName_Attachment_AttachmentNull()
        {
            // arrange
            IAttachment attachment = null;

            // act
            Action validate = () => attachment.GetTypeName();

            // assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(validate);
            Assert.Equal("attachment", exception.ParamName);
        }
        
        [Fact(DisplayName = "GetTypeName: Should return a correct type name")]
        public void GetTypeName_Attachment_Success()
        {
            // arrange
            IAttachment attachment = new ExceptionAttachment();

            // act
            string typeName = attachment.GetTypeName();

            // assert
            Assert.Equal("Exception", typeName);
        }

        #endregion

        #region Type.GetTypeName
        
        [Fact(DisplayName = "GetTypeName: Should throw an argument null exception for type")]
        public void GetTypeName_Type_AttachmentNull()
        {
            // arrange
            Type type = null;

            // act
            Action validate = () => type.GetTypeName();

            // assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(validate);
            Assert.Equal("type", exception.ParamName);
        }
        
        [Fact(DisplayName = "GetTypeName: Should return a correct type name")]
        public void GetTypeName_Type_Success()
        {
            // arrange
            IAttachment attachment = new ObjectAttachment();

            // act
            string typeName = attachment.GetTypeName();

            // assert
            Assert.Equal("Object", typeName);
        }

        #endregion
    }
}