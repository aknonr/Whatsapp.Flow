using MongoDB.Driver;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Flow.Domain.Entities;
using Whatsapp.Flow.Services.Flow.Domain.Repositories;

namespace Whatsapp.Flow.Services.Flow.Infrastructure.Repositories
{
    public class MongoTenantRepository : ITenantRepository
    {
        private readonly IMongoCollection<Tenant> _collection;

        public MongoTenantRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Tenant>("Tenants");
        }

        public async Task<Tenant> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _collection.Find(t => t.PhoneNumber == phoneNumber).SingleOrDefaultAsync();
        }

        public async Task UpsertAsync(Tenant tenant)
        {
            var filter = Builders<Tenant>.Filter.Eq(t => t.Id, tenant.Id);
            var options = new ReplaceOptions { IsUpsert = true };
            await _collection.ReplaceOneAsync(filter, tenant, options);
        }
    }
} 