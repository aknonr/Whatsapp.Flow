using MongoDB.Driver;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Domain.Entities;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Infrastructure.Repositories
{
    public class MongoSubscriptionRepository : ISubscriptionRepository
    {
        private readonly IMongoCollection<Subscription> _subscriptions;

        public MongoSubscriptionRepository(IMongoDatabase database)
        {
            _subscriptions = database.GetCollection<Subscription>("Subscriptions");
        }

        public async Task<Subscription> GetByIdAsync(string id)
        {
            return await _subscriptions.Find(s => s.Id == id).SingleOrDefaultAsync();
        }

        public async Task<Subscription> GetByTenantIdAsync(string tenantId)
        {
            return await _subscriptions.Find(s => s.TenantId == tenantId).SingleOrDefaultAsync();
        }

        public async Task<Subscription> AddAsync(Subscription subscription)
        {
            await _subscriptions.InsertOneAsync(subscription);
            return subscription;
        }

        public async Task UpdateAsync(Subscription subscription)
        {
            await _subscriptions.ReplaceOneAsync(s => s.Id == subscription.Id, subscription);
        }

        public async Task<bool> CheckLimitAsync(string tenantId, string limitType)
        {
            var subscription = await GetByTenantIdAsync(tenantId);
            if (subscription == null) return false;

            return limitType.ToLower() switch
            {
                "users" => subscription.CurrentUsers < subscription.MaxUsers,
                "flows" => subscription.CurrentFlows < subscription.MaxFlows,
                "messages" => subscription.MessagesThisMonth < subscription.MaxMessagesPerMonth,
                "phonenumbers" => subscription.CurrentPhoneNumbers < subscription.MaxPhoneNumbers,
                _ => false
            };
        }

        public async Task IncrementUsageAsync(string tenantId, string usageType, int amount = 1)
        {
            var filter = Builders<Subscription>.Filter.Eq(s => s.TenantId, tenantId);
            var update = Builders<Subscription>.Update.Inc(usageType, amount);
            
            await _subscriptions.UpdateOneAsync(filter, update);
        }
    }
} 