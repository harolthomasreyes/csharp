namespace MyPdfEditor.Core.Contracts.Services
{
    public interface IUndoRedoService
    {
        Task<bool> CanUndoAsync();
        Task<bool> CanRedoAsync();
        Task UndoAsync();
        Task RedoAsync();
        Task ClearHistoryAsync();
        Task AddOperationAsync(IOperation operation);
        Task<int> GetHistoryCountAsync();
    }

    public interface IOperation
    {
        string Description { get; }
        Task ExecuteAsync();
        Task RevertAsync();
    }
}
