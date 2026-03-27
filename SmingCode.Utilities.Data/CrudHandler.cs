namespace SmingCode.Utilities.Data;

internal class CrudHandler<TExtendedEntity, TEntity, TKey>(
    IDbContext _dbContext,
    TableValuedParameterFactory _tableValuedParameterFactory,
    EntityKeyParametersFactory _keyParametersFactory
)
   : ICrudHandler<TExtendedEntity, TEntity, TKey>
        where TExtendedEntity : TEntity, IEntity<TKey>
        where TEntity : IEntity
        where TKey : notnull
{
    private readonly string _sprocRoot = typeof(TEntity).Name.Replace("Entity", string.Empty);

    public async Task<List<TExtendedEntity>> GetAllEntities()
        => await _dbContext.ExecuteManyOrNoResult<TExtendedEntity>(
            $"{_sprocRoot}_GetAll",
            null
        );

    public async Task<TExtendedEntity> GetEntityById(TKey id) =>
        await _dbContext.ExecuteSingleResult<TExtendedEntity>(
            $"{_sprocRoot}_GetById",
            _keyParametersFactory.GetKeyParameters<TExtendedEntity, TKey>(id)
        );

    public async Task SaveNewEntities(TEntity[] newEntities) =>
        await _dbContext.ExecuteWithMultiRowTvp(
            $"{_sprocRoot}_CreateAll",
            null,
            $"newEntities",
            newEntities
        );

    public async Task<TKey> SaveNewEntity(TEntity newEntity)
    {
        var parameterSet = _tableValuedParameterFactory.GetParameterSetWithTvp(
            null,
            "newEntity",
            newEntity
        );

        var result = await _dbContext.Execute(
            connection => connection.QuerySingleAsync<TKey>(
                $"{_sprocRoot}_CreateSingle",
                parameterSet,
                commandType: CommandType.StoredProcedure
            )
        );

        return result;
    }

    public async Task SoftDeleteEntityById(TKey id) =>
        await _dbContext.ExecuteNoResult(
            $"{_sprocRoot}_SoftDeleteSingle",
            _keyParametersFactory.GetKeyParameters<TExtendedEntity, TKey>(id)
        );

    public async Task HardDeleteEntityById(TKey id) =>
        await _dbContext.ExecuteNoResult(
            $"{_sprocRoot}_HardDeleteSingle",
            _keyParametersFactory.GetKeyParameters<TExtendedEntity, TKey>(id)
        );

    public async Task UpdateEntityMatchedById(
        TKey id,
        TEntity updatedEntity
    )
    {
        var parameterSet = _tableValuedParameterFactory.GetParameterSetWithTvp(
            _keyParametersFactory.GetKeyParameters<TExtendedEntity, TKey>(id),
            "updatedEntity",
            updatedEntity
        );

        await _dbContext.Execute(
            connection => connection.ExecuteAsync(
                $"{_sprocRoot}_UpdateMatchedById",
                parameterSet,
                commandType: CommandType.StoredProcedure
            )
        );
    }

    public IDbContext Context => _dbContext;
}
