using System;
using System.Diagnostics.Contracts;

namespace ChilliCream.Logging.Abstractions
{
    /// <summary>
    /// An immutable stack.
    /// A minimum implementation of corefx <c>ImmutableStack{T}</c>.
    /// </summary>
    /// <typeparam name="TElement">The type of element stored by the stack.</typeparam>
    public class ImmutableStack<TElement>
    {
        private readonly TElement _head;
        private readonly ImmutableStack<TElement> _tail;

        private ImmutableStack() { }

        private ImmutableStack(TElement head, ImmutableStack<TElement> tail)
        {
            if (tail == null)
            {
                throw new ArgumentNullException(nameof(tail));
            }
            Contract.EndContractBlock();

            _head = head;
            _tail = tail;
        }

        /// <summary>
        /// Gets the empty stack, upon which all stacks are built.
        /// </summary>
        public static ImmutableStack<TElement> Empty => new ImmutableStack<TElement>();

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        public bool IsEmpty => _tail == null;

        /// <summary>
        /// Gets the element on the top of the stack.
        /// </summary>
        /// <returns>An element on the top of the stack.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the stack is empty.
        /// </exception>
        public TElement Peek()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("This operation does not apply to an empty instance.");
            }

            return _head;
        }

        /// <summary>
        /// Returns a stack that lacks the top element on this stack.
        /// </summary>
        /// <returns>A stack; never <c>null</c></returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the stack is empty.
        /// </exception>
        public ImmutableStack<TElement> Pop()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("This operation does not apply to an empty instance.");
            }

            return _tail;
        }

        /// <summary>
        /// Pushes an element onto a stack and returns the new stack.
        /// </summary>
        /// <param name="element">An element to push onto the stack.</param>
        /// <returns>An new stack.</returns>
        public ImmutableStack<TElement> Push(TElement element)
        {
            return new ImmutableStack<TElement>(element, this);
        }
    }
}