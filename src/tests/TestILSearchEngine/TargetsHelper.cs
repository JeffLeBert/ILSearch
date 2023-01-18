using Mono.Cecil;

namespace ILSearch.Engine;

internal static class TargetsHelper
{
    private readonly static AssemblyDefinition targetsAssemblyDefinition = LoadTargetsAssembly();

    public static TypeDefinition GetType(string @namespace, string name)
        => (from moduleDefinition in targetsAssemblyDefinition.Modules
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