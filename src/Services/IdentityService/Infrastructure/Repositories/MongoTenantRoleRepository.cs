using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Domain.Entities;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;
using System.Linq;

namespace Whatsapp.Flow.Services.Identity.Infrastructure.Repositories
{
    public class MongoTenantRoleRepository : ITenantRoleRepository
    {
        private readonly IMongoCollection<TenantRole> _roles;
        private readonly IUserRepository _userRepository;

        public MongoTenantRoleRepository(IMongoDatabase database, IUserRepository userRepository)
        {
            _roles = database.GetCollection<TenantRole>("TenantRoles");
            _userRepository = userRepository;
        }

        public async Task<TenantRole> GetByIdAsync(string id)
        {
            return await _roles.Find(r => r.Id == id).SingleOrDefaultAsync();
        }

        public async Task<List<TenantRole>> GetByTenantIdAsync(string tenantId)
        {
            return await _roles.Find(r => r.TenantId == tenantId).ToListAsync();
        }
        
        public async Task<TenantRole> GetByNameAsync(string tenantId, string roleName)
        {
            return await _roles.Find(r => r.TenantId == tenantId && r.RoleName == roleName).SingleOrDefaultAsync();
        }

        public async Task<TenantRole> AddAsync(TenantRole role)
        {
            await _roles.InsertOneAsync(role);
            return role;
        }

        public async Task UpdateAsync(TenantRole role)
        {
            await _roles.ReplaceOneAsync(r => r.Id == role.Id, role);
        }

        public async Task DeleteAsync(string id)
        {
            await _roles.DeleteOneAsync(r => r.Id == id);
        }

        public async Task<List<Permission>> GetUserPermissionsAsync(string userId, string tenantId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.TenantId != tenantId)
            {
                return new List<Permission>();
            }

            var roles = await _roles.Find(r => user.TenantRoleIds.Contains(r.Id)).ToListAsync();
            
            var permissions = roles
                .SelectMany(r => r.Permissions)
                .GroupBy(p => p.Resource)
                .Select(g => new Permission
                {
                    Resource = g.Key,
                    Actions = g.SelectMany(p => p.Actions).Distinct().ToList()
                })
                .ToList();

            return permissions;
        }
    }
} 