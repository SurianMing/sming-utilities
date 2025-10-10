using System.Diagnostics.CodeAnalysis;

namespace SurianMing.Utilities.Testing.AutoDomainData;

public abstract class AutoIncludedSpecimenBuilder<T> : ISpecimenBuilder
{
    private readonly Type SpecimenType = typeof(T);

    [return: NotNull]
    protected abstract T CreateSpecimen();

    public object Create(object request, ISpecimenContext context)
    {
        var parameterType = request as Type;

        if (parameterType != SpecimenType)
            return new NoSpecimen();

        return CreateSpecimen();
    }
}