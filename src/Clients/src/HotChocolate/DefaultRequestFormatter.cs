using HotChocolate.Execution;

namespace Thor.Extensions.HotChocolate
{
    public class DefaultRequestFormatter : IRequestFormatter
    {
        public HotChocolateRequest Serialize(IQueryRequest request)
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
