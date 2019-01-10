using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Http
{
    /// <summary>
    /// A bunch of convenient <see cref="AttachmentDispatcher"/> extensions.
    /// </summary>
    internal static class AttachmentDispatcherExtensions
    {
        /// <summary>
        /// Dispatches an <see cref="HttpRequest"/>.
        /// </summary>
        /// <param name="dispatcher">A dispatcher instance.</param>
        /// <param name="id">A correlation id.</param>
        /// <param name="payloadName">A payload name.</param>
        /// <param name="payloadValue">A HTTP request.</param>
        /// <remarks>This method may not break because it is called from EventSources.</remarks>
        public static void Dispatch(this AttachmentDispatcher dispatcher, AttachmentId id,
            string payloadName, HttpRequest payloadValue)
        {
            if (dispatcher != null && id != AttachmentId.Empty &&
                !string.IsNullOrWhiteSpace(payloadName) && payloadValue != null)
            {
                HttpRequestAttachment request = AttachmentFactory
                    .Create<HttpRequestAttachment, HttpRequest>(id, payloadName, payloadValue);

                dispatcher.Dispatch(request);

                if (payloadValue.Body != null && payloadValue.Body.Length > 0)
                {
                    HttpRequestBodyAttachment requestBody = AttachmentFactory
                        .Create<HttpRequestBodyAttachment>(id, payloadName,
                            payloadValue.Body);
                
                    dispatcher.Dispatch(requestBody);
                }
            }
        }

        /// <summary>
        /// Dispatches an <see cref="HttpResponse"/>.
        /// </summary>
        /// <param name="dispatcher">A dispatcher instance.</param>
        /// <param name="id">A correlation id.</param>
        /// <param name="payloadName">A payload name.</param>
        /// <param name="payloadValue">A HTTP response.</param>
        /// <remarks>This method may not break because it is called from EventSources.</remarks>
        public static void Dispatch(this AttachmentDispatcher dispatcher, AttachmentId id,
            string payloadName, HttpResponse payloadValue)
        {
            if (dispatcher != null && id != AttachmentId.Empty &&
                !string.IsNullOrWhiteSpace(payloadName) && payloadValue != null)
            {
                HttpResponseAttachment response = AttachmentFactory
                    .Create<HttpResponseAttachment, HttpResponse>(id, payloadName, payloadValue);

                dispatcher.Dispatch(response);

#pragma warning disable S125 // Sections of code should not be "commented out"
                //if (payloadValue.Body != null && payloadValue.Body.Length > 0)
                //{
                //    HttpResponseBodyAttachment responseBody = AttachmentFactory
                //        .Create<HttpResponseBodyAttachment>(id, payloadName,
                //            payloadValue.Body);
                //
                //    dispatcher.Dispatch(responseBody);
                //}
#pragma warning disable S125 // Sections of code should not be "commented out"
            }
        }
    }
}