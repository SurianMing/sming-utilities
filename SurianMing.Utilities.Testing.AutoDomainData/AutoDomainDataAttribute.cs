﻿namespace SurianMing.Utilities.Testing.AutoDomainData;

public class AutoDomainDataAttribute : AutoDataAttribute
{
    public AutoDomainDataAttribute()
        : base(() => new Fixture().Customize(new SmingCustomization()))
    { }

    public AutoDomainDataAttribute(CompositeCustomization customization)
        : base(() => new Fixture().Customize(customization))
    { }

    public AutoDomainDataAttribute(Type customizationType)
        : base(() => new Fixture().Customize(new SmingCustomization(
            Activator.CreateInstance(customizationType) is ICustomization instance
                ? instance
                : throw new NotSupportedException("Passed customization does not implement ICustomization")
        )))
    { }
}
