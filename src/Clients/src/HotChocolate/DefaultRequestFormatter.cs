using HotChocolate.Execution;

namespace Thor.Extensions.HotChocolate
{
    public class DefaultRequestFormatter
        : IRequestFormatter
    {
        public HotChocolateRequest Serialize(IReadOnlyQueryRequest request)
        {
            return new HotChocolateRequest
            (
                request.QueryId,
                request.Query?.ToString(),
                request.OperationName,
                request.VariableValues
            );
        }
    }
}
