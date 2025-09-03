namespace MyPdfEditor.Core.Contracts.Repositories
{
    public interface ISettingsRepository
    {
        Task SaveSettingAsync<T>(string key, T value);
        Task<T> GetSettingAsync<T>(string key, T defaultValue = default);
        Task<IDictionary<string, object>> GetAllSettingsAsync();
        Task DeleteSettingAsync(string key);
        Task<bool> SettingExistsAsync(string key);
    }
}
