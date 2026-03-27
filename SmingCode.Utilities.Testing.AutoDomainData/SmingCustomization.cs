using AutoFixture.AutoNSubstitute;

namespace SmingCode.Utilities.Testing.AutoDomainData;

public class SmingCustomization : CompositeCustomization
{
    private static readonly List<ICustomization> _defaultCustomizations = [
        new AutoNSubstituteCustomization { ConfigureMembers = true },
        new AutoIncludedSpecimenBuildersCustomization()
    ];

    public SmingCustomization()
        : base(_defaultCustomizations) { }

    public SmingCustomization(params ICustomization[] additionalCustomizations)
        : base(_defaultCustomizations.Concat(additionalCustomizations)) { }
}