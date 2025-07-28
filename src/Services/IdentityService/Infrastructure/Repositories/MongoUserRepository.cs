using MongoDB.Driver;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Domain.Entities;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Infrastructure.Repositories
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _usersCollection;

        public MongoUserRepository(IMongoDatabase database)
        {
            _usersCollection = database.GetCollection<User>("Users");
        }

        public async Task AddAsync(User user)
        {
            await _usersCollection.InsertOneAsync(user);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _usersCollection.Find(u => u.Email == email).SingleOrDefaultAsync();
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _usersCollection.Find(u => u.Id == id).SingleOrDefaultAsync();
        }

        public async Task UpdateAsync(User user)
        {
            await _usersCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
        }

        public async Task DeleteAsync(string id)
        {
            await _usersCollection.DeleteOneAsync(u => u.Id == id);
        }
    }
} 