using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using Xunit;

namespace Thor.Core.Tests
{
    public class AttachmentDispatcherTests
    {
        #region Attach
        
        [Fact(DisplayName = "Attach: Should throw an argument null excption for observer")]
        public void Attach_ObserverNull()
        {
            // arrange
            Action<AttachmentDescriptor> observer = null;
            HashSet<Action<AttachmentDescriptor>> observers = new HashSet<Action<AttachmentDescriptor>>();
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(observers);

            // act
            Action validate = () => dispatcher.Attach(observer);

            // assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(validate);
            Assert.Equal("observer", exception.ParamName);
        }

        [Fact(DisplayName = "Attach: Should attach an observer")]
        public void Attach_Success()
        {
            // arrange
            Action<AttachmentDescriptor> observer = d => { };
            HashSet<Action<AttachmentDescriptor>> observers = new HashSet<Action<AttachmentDescriptor>>();
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(observers);

            // act
            dispatcher.Attach(observer);

            // assert
            Assert.Collection(observers,
                item => Assert.NotNull(item));
        }

        #endregion

        #region Detach

        [Fact(DisplayName = "Detach: Should throw an argument null excption for observer")]
        public void Detach_ObserverNull()
        {
            // arrange
            Action<AttachmentDescriptor> observer = null;
            HashSet<Action<AttachmentDescriptor>> observers = new HashSet<Action<AttachmentDescriptor>>();
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(observers);

            // act
            Action validate = () => dispatcher.Detach(observer);

            // assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(validate);
            Assert.Equal("observer", exception.ParamName);
        }

        [Fact(DisplayName = "Detach: Should detach an observer")]
        public void Detach_Success()
        {
            // arrange
            Action<AttachmentDescriptor> observer = d => { };
            HashSet<Action<AttachmentDescriptor>> observers = new HashSet<Action<AttachmentDescriptor>>()
            {
                observer
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(observers);

            // act
            dispatcher.Detach(observer);

            // assert
            Assert.Empty(observers);
        }

        #endregion

        #region Dispatch

        [Fact(DisplayName = "Dispatch: Should throw an argument null excption for attachment")]
        public void Dispatch_AttachmentsNull()
        {
            // arrange
            IAttachment[] attachments = null;
            HashSet<Action<AttachmentDescriptor>> observers = new HashSet<Action<AttachmentDescriptor>>();
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(observers);

            // act
            Action validate = () => dispatcher.Dispatch(attachments);

            // assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(validate);
            Assert.Equal("attachments", exception.ParamName);
        }

        [Fact(DisplayName = "Dispatch: Should throw an argument null excption for attachment")]
        public void Dispatch_AttachmentsEmpty()
        {
            // arrange
            IAttachment[] attachments = new IAttachment[0];
            HashSet<Action<AttachmentDescriptor>> observers = new HashSet<Action<AttachmentDescriptor>>();
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(observers);

            // act
            Action validate = () => dispatcher.Dispatch(attachments);

            // assert
            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(validate);
            Assert.Equal("attachments", exception.ParamName);
        }

        [Fact(DisplayName = "Dispatch: Should dispatch a single attachment")]
        public void Dispatch_Single_Success()
        {
            // arrange
            int callCount = 0;
            IAttachment attachment = new Mock<IAttachment>().Object;
            Action<AttachmentDescriptor> observer = a => { Interlocked.Increment(ref callCount); };
            HashSet<Action<AttachmentDescriptor>> observers = new HashSet<Action<AttachmentDescriptor>>()
            {
                observer
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(observers);

            // act
            dispatcher.Dispatch(attachment);

            // assert
            Assert.Equal(1, callCount);
        }

        [Fact(DisplayName = "Dispatch: Should dispatch multiple attachments")]
        public void Dispatch_Multiple_Success()
        {
            // arrange
            int callCount = 0;
            IAttachment[] attachments = new[]
            {
                new Mock<IAttachment>().Object,
                new Mock<IAttachment>().Object,
                new Mock<IAttachment>().Object
            };
            Action<AttachmentDescriptor> observer = a => { Interlocked.Increment(ref callCount); };
            HashSet<Action<AttachmentDescriptor>> observers = new HashSet<Action<AttachmentDescriptor>>()
            {
                observer
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(observers);

            // act
            dispatcher.Dispatch(attachments);

            // assert
            Assert.Equal(3, callCount);
        }

        #endregion
    }
}