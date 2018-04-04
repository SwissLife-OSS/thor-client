using System;
using System.Collections.Generic;
using Moq;
using Thor.Core.Abstractions;
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
        public void Dispatch_AttachmentNull()
        {
            // arrange
            IAttachment attachment = null;
            HashSet<Action<AttachmentDescriptor>> observers = new HashSet<Action<AttachmentDescriptor>>();
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(observers);

            // act
            Action validate = () => dispatcher.Dispatch(attachment);

            // assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(validate);
            Assert.Equal("attachment", exception.ParamName);
        }

        [Fact(DisplayName = "Dispatch: Should dispatch an attachment")]
        public void Dispatch_Success()
        {
            // arrange
            bool called = false;
            IAttachment attachment = new Mock<IAttachment>().Object;
            Action<AttachmentDescriptor> observer = a => { called = true; };
            HashSet<Action<AttachmentDescriptor>> observers = new HashSet<Action<AttachmentDescriptor>>()
            {
                observer
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(observers);

            // act
            dispatcher.Dispatch(attachment);

            // assert
            Assert.True(called);
        }

        #endregion
    }
}