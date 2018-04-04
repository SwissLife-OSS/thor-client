using System;
using System.Collections.Generic;
using Thor.Core.Abstractions;
using Xunit;

namespace Thor.Core.Tests
{
    public class AttachmentDispatcherExtensionsTests
    {
        #region Dispatch Exception

        [Fact(DisplayName = "Dispatch: Should not throw an argument null exception for dispatcher")]
        public void Dispatch_Exception_DispatcherNull()
        {
            // arrange
            AttachmentId correlationId = AttachmentId.NewId();
            Exception content = new ArgumentNullException("Test34");
            AttachmentDispatcher dispatcher = null;

            // act
            Action validate = () => dispatcher.Dispatch(correlationId, content);

            // assert
            Assert.Null(Record.Exception(validate));
        }

        [Fact(DisplayName = "Dispatch: Should not throw an argument exception for empty correlationId")]
        public void Dispatch_Exception_CorrelationIdNull()
        {
            // arrange
            bool called = false;
            AttachmentId correlationId = AttachmentId.Empty;
            Exception content = new ArgumentNullException("Test76");
            Action<AttachmentDescriptor> observer = d => { called = true; };
            HashSet<Action<AttachmentDescriptor>> observers = new HashSet<Action<AttachmentDescriptor>>()
            {
                observer
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(observers);

            // act
            dispatcher.Dispatch(correlationId, content);

            // assert
            Assert.False(called);
        }

        [Fact(DisplayName = "Dispatch: Should not throw an argument null exception for content")]
        public void Dispatch_Exception_ContentNull()
        {
            // arrange
            bool called = false;
            AttachmentId correlationId = AttachmentId.NewId();
            Exception content = null;
            Action<AttachmentDescriptor> observer = d => { called = true; };
            HashSet<Action<AttachmentDescriptor>> observers = new HashSet<Action<AttachmentDescriptor>>()
            {
                observer
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(observers);

            // act
            dispatcher.Dispatch(correlationId, content);

            // assert
            Assert.False(called);
        }

        [Fact(DisplayName = "Dispatch: Should dispatch an attachment")]
        public void Dispatch_Exception_Success()
        {
            // arrange
            bool called = false;
            AttachmentId correlationId = AttachmentId.NewId();
            Exception content = new ArgumentNullException("Test11");
            Action<AttachmentDescriptor> observer = d =>
            {
                called = d.TypeName == "Exception" && d.Id == correlationId.ToString();
            };
            HashSet<Action<AttachmentDescriptor>> observers = new HashSet<Action<AttachmentDescriptor>>()
            {
                observer
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(observers);

            // act
            dispatcher.Dispatch(correlationId, content);

            // assert
            Assert.True(called);
        }

        #endregion

        #region Dispatch Object

        [Fact(DisplayName = "Dispatch: Should not throw an argument null exception for dispatcher")]
        public void Dispatch_Object_DispatcherNull()
        {
            // arrange
            AttachmentId correlationId = AttachmentId.NewId();
            object content = "Test56";
            AttachmentDispatcher dispatcher = null;

            // act
            Action validate = () => dispatcher.Dispatch(correlationId, content);

            // assert
            Assert.Null(Record.Exception(validate));
        }

        [Fact(DisplayName = "Dispatch: Should not throw an argument exception for empty correlationId")]
        public void Dispatch_Object_CorrelationIdNull()
        {
            // arrange
            bool called = false;
            AttachmentId correlationId = AttachmentId.Empty;
            object content = "Test01";
            Action<AttachmentDescriptor> observer = d => { called = true; };
            HashSet<Action<AttachmentDescriptor>> observers = new HashSet<Action<AttachmentDescriptor>>()
            {
                observer
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(observers);

            // act
            dispatcher.Dispatch(correlationId, content);

            // assert
            Assert.False(called);
        }

        [Fact(DisplayName = "Dispatch: Should not throw an argument null exception for content")]
        public void Dispatch_Object_ContentNull()
        {
            // arrange
            bool called = false;
            AttachmentId correlationId = AttachmentId.NewId();
            object content = null;
            Action<AttachmentDescriptor> observer = d => { called = true; };
            HashSet<Action<AttachmentDescriptor>> observers = new HashSet<Action<AttachmentDescriptor>>()
            {
                observer
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(observers);

            // act
            dispatcher.Dispatch(correlationId, content);

            // assert
            Assert.False(called);
        }
        
        [Fact(DisplayName = "Dispatch: Should dispatch an attachment")]
        public void Dispatch_Object_Success()
        {
            // arrange
            bool called = false;
            AttachmentId correlationId = AttachmentId.NewId();
            object content = "Test13";
            Action<AttachmentDescriptor> observer = d =>
            {
                called = d.TypeName == "Object" && d.Id == correlationId.ToString();
            };
            HashSet<Action<AttachmentDescriptor>> observers = new HashSet<Action<AttachmentDescriptor>>()
            {
                observer
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(observers);

            // act
            dispatcher.Dispatch(correlationId, content);

            // assert
            Assert.True(called);
        }

        #endregion
    }
}