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
            Query = query;
            OperationName = operationName;
            VariableValues = variableValues;
        }

        public string Query { get; }
        public string OperationName { get; }
        public IReadOnlyDictionary<string, object> VariableValues { get; }
    }
}
