using MongoDB.Driver;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Flow.Domain.Entities;
using Whatsapp.Flow.Services.Flow.Domain.Repositories;

namespace Whatsapp.Flow.Services.Flow.Infrastructure.Repositories
{
    public class MongoFlowStateRepository : IFlowStateRepository
    {
        private readonly IMongoCollection<FlowState> _collection;

        public MongoFlowStateRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<FlowState>("FlowStates");
        }

        public async Task<FlowState> CreateAsync(FlowState flowState)
        {
            await _collection.InsertOneAsync(flowState);
            return flowState;
        }

        public async Task<FlowState?> GetByUserPhoneAsync(string userPhoneNumber)
        {
            return await _collection.Find(fs => fs.UserPhoneNumber == userPhoneNumber).SingleOrDefaultAsync();
        }

        public async Task UpdateAsync(FlowState flowState)
        {
            var filter = Builders<FlowState>.Filter.Eq(fs => fs.Id, flowState.Id);
            await _collection.ReplaceOneAsync(filter, flowState);
        }
    }
} 