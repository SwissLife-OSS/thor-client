using System;
using Xunit;

namespace Thor.Core.Abstractions.Tests
{
    public class AttachmentIdTests
    {
        [Fact(DisplayName = "Empty: Should be allways the same")]
        public void Empty()
        {
            // arrange
            string expectedResult = $"{default(DateTime):yyyyMMdd}-{default(Guid):N}";

            // act
            AttachmentId id = AttachmentId.Empty;

            // assert
            Assert.Equal(AttachmentId.Empty, id);
            Assert.Equal(expectedResult, id);
        }
        
        [Fact(DisplayName = "NewId: Should create always a new id")]
        public void NewId_Unique()
        {
            // act
            AttachmentId id = AttachmentId.NewId();

            // assert
            Assert.NotEqual(AttachmentId.NewId(), id);
        }
        
        [Fact(DisplayName = "NewId: Should create a new id")]
        public void NewId_Success()
        {
            // act
            AttachmentId id = AttachmentId.NewId();

            // assert
            Assert.NotEqual(AttachmentId.Empty, id);
        }
    }
}