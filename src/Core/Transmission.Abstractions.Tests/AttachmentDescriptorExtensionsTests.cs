using System;
using Xunit;

namespace Thor.Core.Transmission.Abstractions.Tests
{
    public class AttachmentDescriptorExtensionsTests
    {
        #region GetFilepath
        
        [Fact(DisplayName = "GetTypeName: Should throw an argument null exception for descriptor")]
        public void GetFilepath_DescriptorNull()
        {
            // arrange
            AttachmentDescriptor descriptor = null;

            // act
            Action validate = () => descriptor.GetFilepath();

            // assert
            Assert.Throws<ArgumentNullException>("descriptor", validate);
        }
        
        [Fact(DisplayName = "GetTypeName: Should return a unique filepath")]
        public void GetFilepath_Success()
        {
            // arrange
            AttachmentDescriptor descriptor = new AttachmentDescriptor
            {
                Id = "UNIQUE",
                Name = "561"
            };

            // act
            string filepath = descriptor.GetFilepath();

            // assert
            Assert.Equal("UNIQUE\\561", filepath);
        }

        #endregion
    }
}