namespace Thor.Extensions.HotChocolate
{
    internal class HotChocolateError
    {
        public string Message { get; set; }
        public string Code { get; set; }
        public IReadOnlyList<object> Path { get; set; }
    }
}