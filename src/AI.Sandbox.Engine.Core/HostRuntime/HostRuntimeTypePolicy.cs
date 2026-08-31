namespace AI.Sandbox.Engine.Core.HostRuntime;

internal static class HostRuntimeTypePolicy
{
    internal static void EnsureExactCapability(IHostRuntimeCapability payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        var payloadType = payload.GetType();
        if (!typeof(IHostRuntimeCapability).IsAssignableFrom(payloadType) ||
            payloadType.IsInterface ||
            payloadType.IsAbstract ||
            payloadType.ContainsGenericParameters ||
            (!payloadType.IsValueType && !payloadType.IsSealed))
        {
            throw new ArgumentException(
                "Host-runtime capabilities must be exact value types or " +
                "sealed reference types.",
                nameof(payload));
        }
    }

    internal static void EnsureExactHealthDetail(
        IHostRuntimeHealthDetail detail)
    {
        EnsureExactContractValue(
            detail,
            typeof(IHostRuntimeHealthDetail),
            "Host-runtime health details must be exact value types or " +
                "sealed reference types.",
            nameof(detail));
    }

    internal static void EnsureExactRequest(IHostRuntimeRequest request)
    {
        EnsureExactContractValue(
            request,
            typeof(IHostRuntimeRequest),
            "Host-runtime requests must be exact value types or sealed " +
                "reference types.",
            nameof(request));
    }

    internal static void EnsureExactCancellationReason(
        IHostRuntimeCancellationReason reason)
    {
        EnsureExactContractValue(
            reason,
            typeof(IHostRuntimeCancellationReason),
            "Host-runtime cancellation reasons must be exact value types or " +
                "sealed reference types.",
            nameof(reason));
    }

    internal static void EnsureExactRetryReason(
        IHostRuntimeRetryReason reason)
    {
        EnsureExactContractValue(
            reason,
            typeof(IHostRuntimeRetryReason),
            "Host-runtime retry reasons must be exact value types or sealed " +
                "reference types.",
            nameof(reason));
    }

    internal static void EnsureExactCompletion(
        IHostRuntimeCompletion completion)
    {
        EnsureExactContractValue(
            completion,
            typeof(IHostRuntimeCompletion),
            "Host-runtime completions must be exact value types or sealed " +
                "reference types.",
            nameof(completion));
    }

    private static void EnsureExactContractValue(
        object value,
        Type contractType,
        string message,
        string parameterName)
    {
        ArgumentNullException.ThrowIfNull(value);

        var valueType = value.GetType();
        if (!contractType.IsAssignableFrom(valueType) ||
            valueType.IsInterface ||
            valueType.IsAbstract ||
            valueType.ContainsGenericParameters ||
            (!valueType.IsValueType && !valueType.IsSealed))
        {
            throw new ArgumentException(
                message,
                parameterName);
        }
    }
}
