using System;
using System.Collections.Generic;
using HotChocolate;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DiagnosticAdapter;

namespace Thor.Extensions.HotChocolate
{
    internal class HotChocolateDiagnosticsListener
    {
        [DiagnosticName("HotChocolate.Execution.Query")]
        public virtual void OnQuery()
        {
        }

        [DiagnosticName("HotChocolate.Execution.Query.Start")]
        public virtual void OnQueryStart(
            IReadOnlyQueryRequest request)
        {
            HttpContext httpContext = request.GetHttpContext();
            HotChocolateActivity activity = HotChocolateActivity
                .Create(
                    new HotChocolateRequest
                    {
                        Query = request.Query,
                        OperationName = request.OperationName,
                        VariableValues = request.VariableValues
                    });

            httpContext.Features.Set(activity);
        }

        [DiagnosticName("HotChocolate.Execution.Query.Stop")]
        public virtual void OnQueryStop(
            IReadOnlyQueryRequest request)
        {
            request
                .GetHttpContext()
                .Features
                .Get<HotChocolateActivity>()
                ?.Dispose();
        }

        [DiagnosticName("HotChocolate.Execution.Query.Error")]
        public virtual void OnQueryError(
            IReadOnlyQueryRequest request,
            Exception exception)
        {
            request
                .GetHttpContext()
                .Features
                .Get<HotChocolateActivity>()
                ?.HandleQueryError(exception);
        }

        [DiagnosticName("HotChocolate.Execution.Validation.Error")]
        public virtual void OnValidationError(
            IReadOnlyQueryRequest request,
            IReadOnlyCollection<IError> errors)
        {
            request
                .GetHttpContext()
                .Features
                .Get<HotChocolateActivity>()
                ?.HandleValidationError(errors);
        }
    }
}