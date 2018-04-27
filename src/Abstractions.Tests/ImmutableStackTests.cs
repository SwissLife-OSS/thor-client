using System;
using Xunit;

namespace Thor.Core.Abstractions.Tests
{
    public class ImmutableStackTests
    {
        #region Empty

        [Fact(DisplayName = "Empty: Should return an empty stack")]
        public void Empty()
        {
            // act
            ImmutableStack<string> stack = ImmutableStack<string>.Empty;

            // assert
            Assert.NotNull(stack);
        }

        #endregion

        #region IsEmpty

        [Fact(DisplayName = "IsEmpty: Should return true if empty")]
        public void IsEmpty_Empty()
        {
            // arrange
            ImmutableStack<string> stack = ImmutableStack<string>.Empty;

            // act
            bool isEmpty = stack.IsEmpty;

            // assert
            Assert.True(isEmpty);
        }

        [Fact(DisplayName = "IsEmpty: Should return false if not empty")]
        public void IsEmpty_NotEmpty()
        {
            // arrange
            ImmutableStack<string> stack = ImmutableStack<string>.Empty.Push("Test 53");

            // act
            bool isEmpty = stack.IsEmpty;

            // assert
            Assert.False(isEmpty);
        }

        #endregion

        #region Peek

        [Fact(DisplayName = "Peek: Should throw an invalid operation exception if stack is empty")]
        public void Peek_Empty()
        {
            // arrange
            ImmutableStack<string> stack = ImmutableStack<string>.Empty;

            // act
            Action validate = () => stack.Peek();

            // assert
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(validate);
            Assert.Equal(ExceptionMessages.ImmutableStackIsEmpty, exception.Message);
        }

        [Fact(DisplayName = "Peek: Should return the top element if stack is not empty")]
        public void Peek_NotEmpty()
        {
            // arrange
            ImmutableStack<string> stack = ImmutableStack<string>.Empty.Push("Bottom").Push("Top");

            // act
            string result = stack.Peek();

            // assert
            Assert.Equal("Top", result);
        }

        #endregion

        #region Pop

        [Fact(DisplayName = "Pop: Should throw an invalid operation exception if stack is empty")]
        public void Pop_Empty()
        {
            // arrange
            ImmutableStack<string> stack = ImmutableStack<string>.Empty;

            // act
            Action validate = () => stack.Pop();

            // assert
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(validate);
            Assert.Equal(ExceptionMessages.ImmutableStackIsEmpty, exception.Message);
        }

        [Fact(DisplayName = "Pop: Should return a stack that lacks the top element if stack is not empty")]
        public void Pop_NotEmpty()
        {
            // arrange
            ImmutableStack<string> stack = ImmutableStack<string>.Empty.Push("Bottom").Push("Top");

            // act
            ImmutableStack<string> result = stack.Pop();

            // assert
            Assert.Equal("Bottom", result.Peek());
        }

        [Fact(DisplayName = "Pop: Should return an empty stack if the stack had only one element before")]
        public void Pop_OnlyOneElement()
        {
            // arrange
            ImmutableStack<string> stack = ImmutableStack<string>.Empty.Push("Top");

            // act
            ImmutableStack<string> result = stack.Pop();

            // assert
            Assert.True(result.IsEmpty);
        }

        #endregion
    }
}