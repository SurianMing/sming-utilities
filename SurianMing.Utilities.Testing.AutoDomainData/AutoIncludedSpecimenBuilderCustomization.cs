namespace SurianMing.Utilities.Testing.AutoDomainData;

public class AutoIncludedSpecimenBuildersCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        var baseTypeName = typeof(AutoIncludedSpecimenBuilder<>).Name;

        var autoIncludedSpecimenBuilders = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName is not null && a.FullName.Contains("Tests"))
            .SelectMany(a => a.GetTypes())
            .Where(t => t.BaseType != null
                    && t.BaseType.Name.Equals(baseTypeName, StringComparison.Ordinal)
                    && !t.IsAbstract
            )
            .Select(Activator.CreateInstance)
            .OfType<ISpecimenBuilder>()
            .ToArray();

        foreach (var specimenBuilder in autoIncludedSpecimenBuilders)
        {
            fixture.Customizations.Add(specimenBuilder);
        }
    }
}