namespace SmingCode.Utilities.Data;

public static class Injection
{
    public static IServiceCollection AddDbContext(
        this IServiceCollection services
    )
    {
        var tvpEntityDefinitions = Assembly.GetCallingAssembly()
            .GetTypes()
            .Where(
                type => !type.IsAbstract
                && typeof(ITableValuedParameter).IsAssignableFrom(type)
            )
            .ToList();
            
        services.AddSingleton(new TableValuedParameterFactory(tvpEntityDefinitions));
        services.AddSingleton(new EntityKeyParametersFactory(tvpEntityDefinitions));
        services.AddSingleton<IDbContext, DbContext>();
        services.AddSingleton(typeof(ICrudHandler<,,>), typeof(CrudHandler<,,>));

        return services;
    }
}