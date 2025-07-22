using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Flow.Domain.Entities;
using Whatsapp.Flow.Services.Flow.Domain.Repositories;

namespace Whatsapp.Flow.Services.Flow.Infrastructure.Repositories
{
    public class MongoFlowRepository : IFlowRepository
    {
        private readonly IMongoCollection<Domain.Entities.Flow> _flowsCollection;

        static MongoFlowRepository()
        {
            // FlowNode'un alt sınıflarını MongoDB'ye tanıtıyoruz.
            // Bu, polymorphism'in (çok biçimlilik) doğru çalışmasını sağlar.
            BsonClassMap.RegisterClassMap<SendMessageNode>();
            BsonClassMap.RegisterClassMap<DecisionNode>();
            BsonClassMap.RegisterClassMap<ListMenuNode>();
            BsonClassMap.RegisterClassMap<ButtonNode>();
            BsonClassMap.RegisterClassMap<WaitNode>();
            BsonClassMap.RegisterClassMap<WebhookNode>();
            BsonClassMap.RegisterClassMap<AskQuestionNode>();
        }

        public MongoFlowRepository(IMongoDatabase database)
        {
            _flowsCollection = database.GetCollection<Domain.Entities.Flow>("Flows");
        }

        public async Task<Domain.Entities.Flow> AddNewAsync(Domain.Entities.Flow flow)
        {
            // Bu metod her zaman yeni bir kayıt ekler, Id'yi sıfırlar.
            flow.Id = null;
            await _flowsCollection.InsertOneAsync(flow);
            return flow;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _flowsCollection.DeleteOneAsync(f => f.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<Domain.Entities.Flow?> GetByIdAsync(string id)
        {
            return await _flowsCollection.Find(f => f.Id == id).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Flow>> GetByTenantIdAsync(string tenantId)
        {
            return await _flowsCollection.Find(f => f.TenantId == tenantId).ToListAsync();
        }

        public async Task<Domain.Entities.Flow> GetActiveFlowByTenantIdAsync(string tenantId)
        {
            return await _flowsCollection.Find(f => f.TenantId == tenantId && f.IsActive).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(string id, Domain.Entities.Flow flow)
        {
            flow.Id = id;
            flow.UpdatedAt = DateTime.UtcNow;
            var result = await _flowsCollection.ReplaceOneAsync(
                f => f.Id == id,
                flow);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<IEnumerable<Domain.Entities.Flow>> GetActiveFlowsByTenantIdAsync(string tenantId)
        {
            return await _flowsCollection.Find(f => f.TenantId == tenantId && f.IsActive).ToListAsync();
        }

        public async Task<Domain.Entities.Flow> GetFlowByTriggerKeywordAsync(string tenantId, string keyword)
        {
            var filter = Builders<Domain.Entities.Flow>.Filter.And(
                Builders<Domain.Entities.Flow>.Filter.Eq(f => f.TenantId, tenantId),
                Builders<Domain.Entities.Flow>.Filter.Eq(f => f.IsActive, true),
                Builders<Domain.Entities.Flow>.Filter.AnyIn("Settings.TriggerKeywords", new[] { keyword })
            );
            
            return await _flowsCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Flow>> GetFlowsByCategoryAsync(string tenantId, string category)
        {
            return await _flowsCollection.Find(f => f.TenantId == tenantId && f.Category == category).ToListAsync();
        }

        public async Task<bool> SetActiveStatusAsync(string id, bool isActive)
        {
            var update = Builders<Domain.Entities.Flow>.Update
                .Set(f => f.IsActive, isActive)
                .Set(f => f.UpdatedAt, DateTime.UtcNow);
                
            var result = await _flowsCollection.UpdateOneAsync(f => f.Id == id, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<int> GetFlowCountByTenantAsync(string tenantId)
        {
            return (int)await _flowsCollection.CountDocumentsAsync(f => f.TenantId == tenantId);
        }
    }
} 