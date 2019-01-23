using System.Collections.Generic;

namespace Thor.Extensions.HotChocolate
{
    internal class HotChocolateRequest
    {
        public string Query { get; set; }
        public string OperationName { get; set; }
        public IReadOnlyDictionary<string, object> VariableValues { get; set; }
    }
}