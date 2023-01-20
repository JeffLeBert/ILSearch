using Mono.Cecil;

namespace ILSearch.Engine;

public record SearchResult(MethodDefinition CallTarget, MethodDefinition CallSource);
