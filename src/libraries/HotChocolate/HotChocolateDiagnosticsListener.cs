using System;
using System.Collections.Generic;
using HotChocolate;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DiagnosticAdapter;
using Thor.Core.Abstractions;

namespace Thor.HotChocolate
{
    internal class HotChocolateDiagnosticsListener
        : IDiagnosticsListener
    {
        public string Name { get; } = "HotChocolate";

        [DiagnosticName("Query")]
        public virtual void OnQuery()
        {
        }

        [DiagnosticName("Query.Start")]
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

        [DiagnosticName("Query.Stop")]
        public virtual void OnQueryStop(
            IReadOnlyQueryRequest request)
        {
            request
                .GetHttpContext()
                .Features
                .Get<HotChocolateActivity>()
                ?.Dispose();
        }

        [DiagnosticName("QueryError")]
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

        [DiagnosticName("ValidationError")]
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