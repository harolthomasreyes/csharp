using StackExchange.Redis;

namespace API.Interfaces
{
    public interface ICacheService
    {

        Task<string> GetData(string key);

        Task<bool> SetData(string key, string value, DateTimeOffset expirationTime);

        Task<object> RemoveData(string key);
    }
}
