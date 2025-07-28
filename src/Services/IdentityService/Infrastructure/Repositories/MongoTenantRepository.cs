using MongoDB.Driver;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Domain.Entities;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Infrastructure.Repositories
{
    public class MongoTenantRepository : ITenantRepository
    {
        private readonly IMongoCollection<Tenant> _collection;

        public MongoTenantRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Tenant>("Tenants");
        }

        public async Task AddAsync(Tenant tenant)
        {
            await _collection.InsertOneAsync(tenant);
        }

        public async Task<Tenant> GetByIdAsync(string id)
        {
            return await _collection.Find(t => t.Id == id).SingleOrDefaultAsync();
        }

        public async Task SoftDeleteAsync(string id)
        {
            var filter = Builders<Tenant>.Filter.Eq(t => t.Id, id);
            var update = Builders<Tenant>.Update
                                         .Set(t => t.IsDeleted, true)
                                         .Set(t => t.DeletedAt, DateTime.UtcNow);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateAsync(Tenant tenant)
        {
            await _collection.ReplaceOneAsync(t => t.Id == tenant.Id, tenant);
        }

       
    }
} 