using ILSearch.Engine;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILSearch.Search;

public class Searcher
{
    private readonly SearchOptions options;

    public Searcher(SearchOptions options)
    {
        this.options = options;
    }

    public void Search()
    {
        var assemblyResolver = this.BuildAssemblyResolver();
        var readerParameters = new ReaderParameters { AssemblyResolver = assemblyResolver };

        foreach (var assemblyFileInfo in EnumerateAssemblies(this.options))
        {
            this.Search(readerParameters, assemblyFileInfo);
        }
    }

    private void Search(ReaderParameters readerParameters, FileInfo assemblyFileInfo)
    {
        // Ignore bad assemblies. Probably non .NET.
        AssemblyDefinition assemblyDefinition;
        try
        {
            assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyFileInfo.FullName, readerParameters);
        }
        catch (BadImageFormatException)
        {
            return;
        }

        if (this.IsSearchAssembly(assemblyDefinition) || this.AssemblyReferencesTargetAssembly(assemblyDefinition))
        {
            Console.WriteLine($"Searching {assemblyDefinition.FullName}");
            this.Search(assemblyDefinition);
        }
        else
        {
            //Console.WriteLine($"Skipping {assemblyDefinition.FullName}");
        }
    }

    private bool IsSearchAssembly(AssemblyDefinition assemblyDefinition)
        => assemblyDefinition.FullName == this.options.TargetAssembly.FullName;

    private bool AssemblyReferencesTargetAssembly(AssemblyDefinition assemblyDefinition)
    {
        foreach (var module in assemblyDefinition.Modules)
        {
            foreach (var reference in module.AssemblyReferences)
            {
                if (reference.FullName == this.options.TargetAssembly.FullName)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void Search(AssemblyDefinition assemblyDefinition)
    {
        foreach (var methodDefinition in EnumerateMethodsWithBodies(this.options, assemblyDefinition))
        {
            Search(assemblyDefinition, methodDefinition);
        }
    }

    private void Search(AssemblyDefinition assemblyDefinition, MethodDefinition methodDefinition)
    {
        //Mono.Cecil.Rocks.ILParser.Parse(method, visitor);

        var methodBody = methodDefinition.Body;
        foreach (var instruction in methodBody.Instructions)
        {
            Search(assemblyDefinition, methodBody, instruction);
        }
    }

    private void Search(AssemblyDefinition assemblyDefinition, MethodBody methodBody, Instruction instruction)
    {
        if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Calli || instruction.OpCode == OpCodes.Callvirt)
        {
            var methodReference = (MethodReference)instruction.Operand;
            if (methodBody.Method.Module.MetadataResolver.TryResolve(methodReference, out var resolvedMethodReference))
            {
                if (resolvedMethodReference.DeclaringType.Module.Assembly.FullName == this.options.TargetAssembly.FullName)
                {
                    var methodDefinition = methodReference.Resolve();
                    if (methodDefinition.DeclaringType.Module.Assembly.FullName == this.options.TargetAssembly.FullName)
                    {
                        foreach (var propertyDefinition in this.options.Properties)
                        {
                            if (propertyDefinition.DeclaringType.FullName == methodDefinition.DeclaringType.FullName)
                            {
                                if (propertyDefinition.GetMethod.FullName == methodDefinition.FullName)
                                {
                                    Console.WriteLine($"{methodBody.Method.FullName} called Get on {propertyDefinition.FullName}");
                                }
                                else if (propertyDefinition.SetMethod.FullName == methodDefinition.FullName)
                                {
                                    Console.WriteLine($"{methodBody.Method.FullName} called Set on {propertyDefinition.FullName}");
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private DefaultAssemblyResolver BuildAssemblyResolver()
    {
        var assemblyResolver = new DefaultAssemblyResolver();
        foreach (var searchDirectory in this.options.SearchDirectories)
        {
            assemblyResolver.AddSearchDirectory(searchDirectory.FullName);
        }

        return assemblyResolver;
    }

    private static IEnumerable<FileInfo> EnumerateAssemblies(SearchOptions options)
    {
        foreach (var searchDirectory in options.SearchDirectories)
        {
            var allAssemblies = searchDirectory.EnumerateFiles("*.dll").Concat(searchDirectory.EnumerateFiles("*.exe"));
            foreach (var assemblyFileName in allAssemblies)
            {
                yield return assemblyFileName;
            }
        }
    }

    private static IEnumerable<MethodDefinition> EnumerateMethodsWithBodies(SearchOptions options, AssemblyDefinition assemblyDefinition)
    {
        foreach (var moduleDefinition in assemblyDefinition.Modules)
        {
            foreach (var typeDefinition in moduleDefinition.Types)
            {
                foreach (var methodDefinition in typeDefinition.Methods)
                {
                    if (methodDefinition.HasBody)
                    {
                        yield return methodDefinition;
                    }
                }
            }
        }
    }
}