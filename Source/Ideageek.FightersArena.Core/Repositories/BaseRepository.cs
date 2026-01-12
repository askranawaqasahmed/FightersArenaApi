using SqlKata;
using SqlKata.Execution;

namespace Ideageek.FightersArena.Core.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(int? page = null, int? pageSize = null);
    Task<T?> GetByIdAsync(Guid id);
    Task<Guid> InsertAsync(T entity);
    Task<int> UpdateAsync(Guid id, object data);
    Task<int> DeleteAsync(Guid id);
}

public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly QueryFactory Db;
    protected readonly string Table;
    protected readonly string IdColumn;

    protected BaseRepository(QueryFactory db, string table, string idColumn = "Id")
    {
        Db = db;
        Table = table;
        IdColumn = idColumn;
    }

    protected Query Query() => Db.Query(Table);

    public virtual async Task<IEnumerable<T>> GetAllAsync(int? page = null, int? pageSize = null)
    {
        var query = Query();
        if (page.HasValue && pageSize.HasValue)
        {
            query = query.ForPage(page.Value, pageSize.Value);
        }

        return await query.GetAsync<T>();
    }

    public virtual Task<T?> GetByIdAsync(Guid id) => Query().Where(IdColumn, id).FirstOrDefaultAsync<T>();

    public virtual async Task<Guid> InsertAsync(T entity)
    {
        var idProp = entity!.GetType().GetProperty(IdColumn);
        if (idProp is not null && idProp.PropertyType == typeof(Guid))
        {
            var current = (Guid)idProp.GetValue(entity)!;
            if (current == Guid.Empty)
            {
                idProp.SetValue(entity, Guid.NewGuid());
            }
        }

        await Query().InsertAsync(entity);
        return idProp is null ? Guid.Empty : (Guid)idProp.GetValue(entity)!;
    }

    public virtual Task<int> UpdateAsync(Guid id, object data) => Query().Where(IdColumn, id).UpdateAsync(data);

    public virtual Task<int> DeleteAsync(Guid id) => Query().Where(IdColumn, id).DeleteAsync();
}
