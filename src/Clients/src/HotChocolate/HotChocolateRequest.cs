using System;
using System.Collections.Generic;

#nullable enable

namespace Thor.Extensions.HotChocolate
{
    public class HotChocolateRequest
    {
        public HotChocolateRequest(
            string? id,
            string? query,
            string? operationName,
            IReadOnlyDictionary<string, object>? variableValues)
        {
            Id = id;
            Query = query;
            OperationName = operationName;
            VariableValues = variableValues;
        }

        public string? Id { get; }

        public string? Query { get; }

        public string? OperationName { get; }

        public IReadOnlyDictionary<string, object>? VariableValues { get; }

        public DateTime Started { get; } = DateTime.UtcNow;
    }
}
