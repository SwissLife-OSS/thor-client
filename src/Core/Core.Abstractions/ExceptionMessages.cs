namespace Thor.Core.Abstractions
{
    internal static class ExceptionMessages
    {
        public const string ApplicationIdMustBeGreaterZero = "The application id must be greater than zero.";
        public const string CollectionIsEmpty = "Must contain at least one item";
        public const string ImmutableStackIsEmpty = "This operation does not apply to an empty instance.";
        public const string NoActivityIdFound = "No activity id found.";
    }
}
