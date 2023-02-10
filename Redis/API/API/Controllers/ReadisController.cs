using API.Interfaces;
using API.Querys;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    public class RedisController : ControllerBase
    {
        private readonly ILogger<RedisController> _logger;
        private readonly ICacheService _cacheService;

        public RedisController(ILogger<RedisController> logger, ICacheService cacheService)
        {
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _cacheService = cacheService?? throw new ArgumentException(nameof(cacheService));
        }

        [HttpGet]
        public async Task<string> Get(RedisEntityGetQuery entity)
        {
            try
            {
                return await _cacheService.GetData(entity.Key);
            }
            catch (Exception e)
            {
                throw;
            }
          
        }

        [HttpPost]
        public async Task Post(RedisEntityPostQuery entity)
        {
            try
            {
                await _cacheService.SetData(entity.Key, entity.Key, DateTime.Now.AddMinutes(1));
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpDelete]
        public async Task Delete(RedisEntityDeleteQuery entity)
        {
            try
            {
                _cacheService.RemoveData(entity.Key);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}