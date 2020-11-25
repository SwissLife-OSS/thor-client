using HotChocolate.Execution;

namespace Thor.Extensions.HotChocolate
{
    public interface IRequestFormatter
    {
        HotChocolateRequest Serialize(IQueryRequest request);
    }
}
