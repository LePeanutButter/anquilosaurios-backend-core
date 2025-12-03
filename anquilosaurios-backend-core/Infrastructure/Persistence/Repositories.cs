using aquilosaurios_backend_core.API;
using aquilosaurios_backend_core.Domain.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace aquilosaurios_backend_core.Infrastructure.Persistence
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetByFiltersAsync(UserFiltersDTO filters);
        Task CreateAsync(User user);
        Task CreateManyAsync(IEnumerable<User> users);
        Task UpdateAsync(User user);
        Task DeleteByIdAsync(Guid id);
        Task DeleteByEmailAsync(string email);
        Task<User?> GetByIdentifierAsync(string identifier);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _collection;
        public UserRepository(IMongoDatabase database) => _collection = database.GetCollection<User>("Users");
        public UserRepository(IMongoCollection<User> collection) => _collection = collection;
        protected virtual Task<List<User>> ExecuteFindToListAsync(FilterDefinition<User> filter) =>
            _collection.Find(filter).ToListAsync();
        protected virtual Task<User> ExecuteFindFirstOrDefaultAsync(FilterDefinition<User> filter) =>
            _collection.Find(filter).FirstOrDefaultAsync();

        public UserRepository()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("AquilosauriosDB");
            _collection = database.GetCollection<User>("Users");
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var result = await ExecuteFindToListAsync(Builders<User>.Filter.Empty);
            return result;
        }

        public async Task<IEnumerable<User>> GetByFiltersAsync(UserFiltersDTO filters)
        {
            var builder = Builders<User>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrEmpty(filters.Email))
                filter &= builder.Eq(u => u.Email, filters.Email);

            if (!string.IsNullOrEmpty(filters.Name))
                filter &= builder.Regex(u => u.Name, new BsonRegularExpression(filters.Name, "i"));

            var result = await ExecuteFindToListAsync(filter);
            return result;
        }

        public async Task<User?> GetByIdentifierAsync(string identifier)
        {
            var filter = Builders<User>.Filter.Or(
                Builders<User>.Filter.Eq(u => u.Email, identifier),
                Builders<User>.Filter.Eq(u => u.Username, identifier)
            );

            return await ExecuteFindFirstOrDefaultAsync(filter);
        }

        public async Task CreateAsync(User user)
        {
            await _collection.InsertOneAsync(user);
        }

        public async Task CreateManyAsync(IEnumerable<User> users)
        {
            await _collection.InsertManyAsync(users);
        }

        public async Task UpdateAsync(User user)
        {
            await _collection.ReplaceOneAsync(
                u => u.Id == user.Id,
                user
            );
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            await _collection.DeleteOneAsync(u => u.Id == id);
        }

        public async Task DeleteByEmailAsync(string email)
        {
            await _collection.DeleteOneAsync(u => u.Email == email);
        }
    }
}