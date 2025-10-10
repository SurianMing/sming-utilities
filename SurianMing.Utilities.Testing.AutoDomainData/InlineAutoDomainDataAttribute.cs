namespace SurianMing.Utilities.Testing.AutoDomainData;

public class InlineAutoDomainDataAttribute : InlineAutoDataAttribute
{
    public InlineAutoDomainDataAttribute(params object[] objects)
        : base(new AutoDomainDataAttribute(), objects) { }

    public InlineAutoDomainDataAttribute(CompositeCustomization customization, params object[] objects)
        : base(new Fixture().Customize(customization), objects) { }
}