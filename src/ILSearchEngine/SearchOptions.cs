using System.Collections.Immutable;
using Mono.Cecil;

namespace ILSearch.Engine;

public record SearchOptions
{
    public AssemblyDefinition TargetAssembly { get; init; }

    public ImmutableArray<MethodDefinition> Methods { get; init; }

    public ImmutableArray<PropertyDefinition> Properties { get; init; }

    public ImmutableArray<DirectoryInfo> SearchDirectories { get; init; }
}