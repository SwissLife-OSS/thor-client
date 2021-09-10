using System;
using System.Collections.Generic;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Resolvers;

namespace Thor.Extensions.HotChocolate
{
    internal class HotChocolateDiagnosticsListener : DiagnosticEventListener
    {
        private readonly IRequestFormatter _formatter;

        public HotChocolateDiagnosticsListener(IRequestFormatter formatter)
        {
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        }

        public override IDisposable ExecuteRequest(IRequestContext context)
        {
            var activity = HotChocolateActivity.Create(_formatter.Serialize(context.Request));
            context.SetActivity(activity);
            return activity;
        }

        public override void RequestError(IRequestContext context, Exception exception)
        {
            context.GetActivity().HandleQueryError(exception);
        }

        public override void ValidationErrors(IRequestContext context, IReadOnlyList<IError> errors)
        {
            context.GetActivity().HandleValidationErrors(context.GetRequest(), errors);
        }

        public override void ResolverError(IMiddlewareContext context, IError error)
        {
            context.GetActivity().HandlesResolverError(context.GetRequest(), error);
        }
    }
}
