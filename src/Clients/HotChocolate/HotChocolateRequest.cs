using System;
using System.Collections.Generic;

namespace Thor.Extensions.HotChocolate
{
    public class HotChocolateRequest
    {
        public HotChocolateRequest(
            string query,
            string operationName,
            IReadOnlyDictionary<string, object> variableValues)
        {
            Query = query
                ?? throw new ArgumentNullException(nameof(query));
            OperationName = operationName
                ?? throw new ArgumentNullException(nameof(operationName));
            VariableValues = variableValues
                ?? throw new ArgumentNullException(nameof(variableValues));
        }

        public string Query { get; }
        public string OperationName { get; }
        public IReadOnlyDictionary<string, object> VariableValues { get; }
    }
}
