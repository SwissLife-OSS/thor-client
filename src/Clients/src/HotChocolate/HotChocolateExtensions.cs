using System;
using HotChocolate;

namespace Thor.Extensions.HotChocolate
{
    internal static class HotChocolateExtensions
    {
        public static void SetActivity(this IHasContextData context, HotChocolateActivity activity)
        {
            context.ContextData[nameof(HotChocolateActivity)] = activity;
        }

        public static HotChocolateActivity GetActivity(this IHasContextData context)
        {
            if (context.ContextData.TryGetValue(nameof(HotChocolateActivity), out object value) &&
                value is HotChocolateActivity activity)
            {
                return activity;
            }
            throw new InvalidOperationException();
        }

        public static HotChocolateRequest GetRequest(this IHasContextData context) =>
            context.GetActivity().Request;
    }
}
