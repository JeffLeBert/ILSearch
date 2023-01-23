using System.Diagnostics.CodeAnalysis;
using Mono.Cecil;

namespace ILSearch.Engine;

internal static class MetadataResolverExtensions
{
    public static bool TryResolve(
        this IMetadataResolver resolver,
        MethodReference method,
        [NotNullWhen(true)] out MethodDefinition? methodDefinition)
    {
        try
        {
            methodDefinition = resolver.Resolve(method);
            return methodDefinition is not null;
        }
        catch (AssemblyResolutionException)
        {
            methodDefinition = null;
            return false;
        }
    }
}