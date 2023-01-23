using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace ILSearch.Engine;

internal class ILVisitor : IILVisitor
{
    private readonly MethodDefinition methodDefinition;
    private readonly SearchOptions options;
    private readonly IList<SearchResult> searchResults = new List<SearchResult>();

    public ILVisitor(MethodDefinition methodDefinition, SearchOptions options)
    {
        this.methodDefinition = methodDefinition;
        this.options = options;
    }

    public IEnumerable<SearchResult> SearchResults => this.searchResults;

    public void OnInlineArgument(OpCode opcode, ParameterDefinition parameter)
    {
    }

    public void OnInlineBranch(OpCode opcode, int offset)
    {
    }

    public void OnInlineByte(OpCode opcode, byte value)
    {
    }

    public void OnInlineDouble(OpCode opcode, double value)
    {
    }

    public void OnInlineField(OpCode opcode, FieldReference field)
    {
    }

    public void OnInlineInt32(OpCode opcode, int value)
    {
    }

    public void OnInlineInt64(OpCode opcode, long value)
    {
    }

    public void OnInlineMethod(OpCode opcode, MethodReference methodReference)
    {
        if (this.methodDefinition.Module.MetadataResolver.TryResolve(methodReference, out var resolvedMethodReference))
        {
            if (resolvedMethodReference.DeclaringType.Module.Assembly.FullName == this.options.TargetAssembly.FullName)
            {
                var methodDefinition = methodReference.Resolve();
                if (methodDefinition.DeclaringType.Module.Assembly.FullName == this.options.TargetAssembly.FullName)
                {
                    foreach (var thisMethod in this.options.Methods)
                    {
                        if (thisMethod.DeclaringType.FullName == methodDefinition.DeclaringType.FullName)
                        {
                            if (thisMethod.FullName == methodDefinition.FullName)
                            {
                                this.searchResults.Add(new SearchResult(thisMethod, this.methodDefinition));
                            }
                        }
                    }
                }
            }
        }
    }

    public void OnInlineNone(OpCode opcode)
    {
    }

    public void OnInlineSByte(OpCode opcode, sbyte value)
    {
    }

    public void OnInlineSignature(OpCode opcode, CallSite callSite)
    {
    }

    public void OnInlineSingle(OpCode opcode, float value)
    {
    }

    public void OnInlineString(OpCode opcode, string value)
    {
    }

    public void OnInlineSwitch(OpCode opcode, int[] offsets)
    {
    }

    public void OnInlineType(OpCode opcode, TypeReference type)
    {
    }

    public void OnInlineVariable(OpCode opcode, VariableDefinition variable)
    {
    }
}