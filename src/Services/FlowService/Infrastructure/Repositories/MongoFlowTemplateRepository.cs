using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Flow.Domain.Entities;
using Whatsapp.Flow.Services.Flow.Domain.Repositories;

namespace Whatsapp.Flow.Services.Flow.Infrastructure.Repositories
{
    public class MongoFlowTemplateRepository : IFlowTemplateRepository
    {
        private readonly IMongoCollection<FlowTemplate> _collection;

        public MongoFlowTemplateRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<FlowTemplate>("FlowTemplates");
        }

        public async Task<FlowTemplate> CreateAsync(FlowTemplate template)
        {
            await _collection.InsertOneAsync(template);
            return template;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(t => t.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<IEnumerable<FlowTemplate>> GetAllPublicAsync()
        {
            return await _collection.Find(t => t.IsPublic).ToListAsync();
        }

        public async Task<IEnumerable<FlowTemplate>> GetByCategoryAsync(string category)
        {
            return await _collection.Find(t => t.Category == category && t.IsPublic).ToListAsync();
        }

        public async Task<FlowTemplate> GetByIdAsync(string id)
        {
            return await _collection.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task IncrementUsageCountAsync(string templateId)
        {
            var update = Builders<FlowTemplate>.Update.Inc(t => t.UsageCount, 1);
            await _collection.UpdateOneAsync(t => t.Id == templateId, update);
        }

        public async Task<IEnumerable<FlowTemplate>> SearchAsync(string searchTerm)
        {
            var filter = Builders<FlowTemplate>.Filter.And(
                Builders<FlowTemplate>.Filter.Eq(t => t.IsPublic, true),
                Builders<FlowTemplate>.Filter.Or(
                    Builders<FlowTemplate>.Filter.Regex(t => t.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    Builders<FlowTemplate>.Filter.Regex(t => t.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    Builders<FlowTemplate>.Filter.AnyIn(t => t.Tags, new[] { searchTerm })
                )
            );

            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<bool> UpdateAsync(string id, FlowTemplate template)
        {
            template.Id = id;
            var result = await _collection.ReplaceOneAsync(t => t.Id == id, template);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
} 