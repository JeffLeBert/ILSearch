using System.Collections.Immutable;
using Mono.Cecil;

namespace ILSearch.Engine;

internal static class TargetsHelper
{
    public static SearchOptions BuildSearchOptions()
    {
        var directory = Path.GetDirectoryName(typeof(TargetsHelper).Assembly.Location)!;
        var searchOptions = new SearchOptions
        {
            TargetAssembly = TargetsAssemblyDefinition,
            Methods = ImmutableArray<MethodDefinition>.Empty,
            SearchDirectories = ImmutableArray<DirectoryInfo>.Empty.Add(new DirectoryInfo(directory))
        };

        return searchOptions;
    }

    public static AssemblyDefinition TargetsAssemblyDefinition => LoadTargetsAssembly();

    public static TypeDefinition GetType(string @namespace, string name)
        => (from moduleDefinition in TargetsAssemblyDefinition.Modules
            from typeDefinition in moduleDefinition.Types
            where typeDefinition.Name == name && typeDefinition.Namespace == @namespace
            select typeDefinition)
            .First();

    private static AssemblyDefinition LoadTargetsAssembly()
    {
        var assemblyFileName = typeof(TargetsHelper).Assembly.Location;

        var assemblyResolver = new DefaultAssemblyResolver();
        assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(assemblyFileName));

        var readerParameters = new ReaderParameters { AssemblyResolver = assemblyResolver };

        return AssemblyDefinition.ReadAssembly(assemblyFileName, readerParameters);
    }
}