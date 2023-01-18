using Mono.Cecil;
using Xunit;

namespace ILSearch.Engine;

public class When_searching_for_properties
{
    private static readonly TypeDefinition targetsTypeDef = TargetsHelper.GetType("ILSearch.Engine", nameof(Targets));

    [Fact]
    public void Can_find_a_public_instance_property_getter()
    {
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