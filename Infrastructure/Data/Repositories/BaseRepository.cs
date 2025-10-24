using Domain.Common;
using MongoDB.Driver;

namespace Infrastructure.Data.Repositories
{
    public abstract class BaseRepository<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;

        protected BaseRepository(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public virtual async Task<T?> GetByIdAsync(string id)
        {
            var filterBuilder = Builders<T>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq(x => x.Id, id),
                filterBuilder.Or(
                    filterBuilder.Eq(x => x.IsDeleted, false),
                    filterBuilder.Exists(x => x.IsDeleted, false)
                )
            );
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var filterBuilder = Builders<T>.Filter;
            var filter = filterBuilder.Or(
                filterBuilder.Eq(x => x.IsDeleted, false),
                filterBuilder.Exists(x => x.IsDeleted, false)
            );
            return await _collection.Find(filter).ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            return entity;
        }

        public virtual async Task DeleteAsync(string id)
        {
            var update = Builders<T>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            await _collection.UpdateOneAsync(x => x.Id == id, update);
        }

        public virtual async Task<bool> ExistsAsync(string id)
        {
            var filterBuilder = Builders<T>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq(x => x.Id, id),
                filterBuilder.Or(
                    filterBuilder.Eq(x => x.IsDeleted, false),
                    filterBuilder.Exists(x => x.IsDeleted, false)
                )
            );
            return await _collection.CountDocumentsAsync(filter) > 0;
        }
    }
}
