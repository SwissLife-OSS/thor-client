namespace ChilliCream.Logging.Abstractions
{
    /// <summary>
    /// A bunch of convenient <see cref="string"/> extension methods.
    /// </summary>
    public static class StringExtensions
    {
        private const int _messageLength = 20480;
        private const string _messageTooLong = " ... The message is too long ...";

        /// <summary>
        /// Ensures a string value is not <c>null</c>.
        /// If <c>null</c>, it will be set to <c>string.Empty</c>.
        /// </summary>
        /// <param name="value">A string value.</param>
        /// <remarks>Only for <c>unsafe</c> code scenarios.</remarks>
        public static void SetToEmptyIfNull(ref string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }
        }

        /// <summary>
        /// Ensures a string does not exceed the specified length of 20480.
        /// </summary>
        /// <param name="value">A string value.</param>
        /// <returns>The origin string or the truncated version of the string.</returns>
        public static string TruncateIfExceedLength(this string value) =>
            value.TruncateIfExceedLength(_messageLength);

        /// <summary>
        /// Ensures a string does not exceed the specified <paramref name="length"/>.
        /// </summary>
        /// <param name="value">A string value.</param>
        /// <param name="length">A length which may not be exceeded.</param>
        /// <returns>The origin string or the truncated version of the string.</returns>
        public static string TruncateIfExceedLength(this string value, int length)
        {
            if (value != null && value.Length > length)
            {
                return value.Substring(0, length) +
                    _messageTooLong;
            }

            return value;
        }
    }
}
