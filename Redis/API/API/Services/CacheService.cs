
using API.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace API.Services
{
    public class CacheService : ICacheService
    {
        private readonly IConnectionMultiplexer  _connectionMultiplexer;
        public CacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer =  connectionMultiplexer ?? throw new ArgumentException(nameof(connectionMultiplexer));
        }

        public async Task<string> GetData(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var db = _connectionMultiplexer.GetDatabase();
                return (await db.StringGetAsync(key));
            }
            return default;
        }
        public async Task<bool> SetData(string key, string value, DateTimeOffset expirationTime )
        {
            var db = _connectionMultiplexer.GetDatabase();
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = await db.StringSetAsync(key, JsonConvert.SerializeObject(value), expiryTime);
            return isSet;
        }
        public async Task<object> RemoveData(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            bool _isKeyExist = db.KeyExists(key);
            if (_isKeyExist == true)
            {
                return db.KeyDelete(key);
            }
            return false;
        }
    }
}
