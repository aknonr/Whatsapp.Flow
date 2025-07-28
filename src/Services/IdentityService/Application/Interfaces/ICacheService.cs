using System;
using System.Threading.Tasks;

namespace Whatsapp.Flow.Services.Identity.Application.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);
        Task<string> GetStringAsync(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task SetStringAsync(string key, string value, TimeSpan? expiry = null);
        Task<bool> ExistsAsync(string key);
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
    }
} 