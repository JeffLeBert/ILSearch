using System.Collections.Immutable;
using ILSearch.Search;
using Mono.Cecil;
using Xunit;

namespace ILSearch.Engine;

public class When_searching_for_properties
{
    private static readonly TypeDefinition targetsTypeDef = TargetsHelper.GetType("ILSearch.Engine", nameof(Targets));

    [Fact]
    public void Can_find_a_public_instance_property_getter()
    {
        var propertyGetter = targetsTypeDef.Properties
            .First(def => def.Name == nameof(Targets.PublicInstanceProperty))
            .GetMethod;
        var options = TargetsHelper.BuildSearchOptions()
            with { Methods = ImmutableArray<MethodDefinition>.Empty.Add(propertyGetter) };

        var searcher = new Searcher(options);
        var targetMethod = targetsTypeDef.Methods.First(def => def.Name == nameof(Targets.PublicInstanceProperty_calls_getter));
        var results = searcher.Search(targetMethod).ToList();

        var result = Assert.Single(results);
        Assert.Same(propertyGetter, result.CallTarget);
        Assert.Same(targetMethod, result.CallSource);
    }
}

public class Targets
{
    // SEE:
    // https://github.com/jbevain/cecil/blob/master/rocks/Mono.Cecil.Rocks/ILParser.cs

    public void PublicInstanceProperty_calls_getter()
    {
        _ = this.PublicInstanceProperty;
    }

    public bool PublicInstanceProperty { get; set; }
}