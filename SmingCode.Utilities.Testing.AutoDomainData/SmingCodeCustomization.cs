using AutoFixture.AutoNSubstitute;

namespace SmingCode.Utilities.Testing.AutoDomainData;

public class SmingCodeCustomization : CompositeCustomization
{
    private static readonly List<ICustomization> _defaultCustomizations = [
        new AutoNSubstituteCustomization { ConfigureMembers = true },
        new AutoIncludedSpecimenBuildersCustomization()
    ];

    public SmingCodeCustomization()
        : base(_defaultCustomizations) { }

    public SmingCodeCustomization(params ICustomization[] additionalCustomizations)
        : base(_defaultCustomizations.Concat(additionalCustomizations)) { }
}