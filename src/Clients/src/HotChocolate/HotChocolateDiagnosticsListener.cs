using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DiagnosticAdapter;

namespace Thor.Extensions.HotChocolate
{
    public class HotChocolateDiagnosticsListener
        : IDiagnosticObserver
    {
        private readonly IRequestFormatter _formatter;

        public HotChocolateDiagnosticsListener(IRequestFormatter formatter)
        {
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        }

        [DiagnosticName(DiagnosticNames.Query)]
        public void QueryExecute()
        {
            // This method is required to enable recording "Query.Start" and
            // "Query.Stop" diagnostic events. Do not write code in here.
        }

        [DiagnosticName(DiagnosticNames.StartQuery)]
        public void BeginQueryExecute(IQueryContext context)
        {
            HotChocolateRequest request = _formatter.Serialize(context.Request);
            context.ContextData[nameof(HotChocolateRequest)] = request;

            HttpContext httpContext = context.GetHttpContext();
            var activity = HotChocolateActivity.Create(request);
            httpContext.Features.Set(activity);
        }

        [DiagnosticName(DiagnosticNames.StopQuery)]
        public void EndQueryExecute(
            IQueryContext context,
            IExecutionResult result)
        {
            context
                .GetHttpContext()
                .Features
                .Get<HotChocolateActivity>()
                ?.Dispose();
        }

        [DiagnosticName("HotChocolate.Execution.Query.Error")]
        public virtual void OnQueryError(
            IQueryContext context,
            Exception exception)
        {
            context
                .GetHttpContext()
                .Features
                .Get<HotChocolateActivity>()
                ?.HandleQueryError(exception);
        }

        [DiagnosticName("HotChocolate.Execution.Resolver.Error")]
        public virtual void OnResolverError(
            IResolverContext context,
            IEnumerable<IError> errors)
        {
            context
                .GetHttpContext()
                .Features
                .Get<HotChocolateActivity>()
                ?.HandlesResolverErrors(
                    TryGetRequest(context.ContextData),
                    errors.ToList());
        }

        [DiagnosticName("HotChocolate.Execution.Validation.Error")]
        public virtual void OnValidationError(
            IQueryContext context,
            IReadOnlyCollection<IError> errors)
        {
            context
                .GetHttpContext()
                .Features
                .Get<HotChocolateActivity>()
                ?.HandleValidationError(
                    TryGetRequest(context.ContextData),
                    errors.ToList());
        }

        private static HotChocolateRequest TryGetRequest(
            IDictionary<string, object> contextData)
        {
            if (contextData.TryGetValue(nameof(HotChocolateRequest),
                out object request)
                && request is HotChocolateRequest r)
            {
                return r;
            }
            return null;
        }
    }
}
