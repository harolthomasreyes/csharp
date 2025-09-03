// MyPdfEditor.Core/Services/Interfaces/IUndoRedoService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyPdfEditor.Core.Services.Interfaces
{
    public interface IUndoRedoService
    {
        // Operation management
        Task PushOperationAsync(IUndoableOperation operation);
        Task<bool> UndoAsync();
        Task<bool> RedoAsync();

        // History management
        Task ClearHistoryAsync();
        Task<int> GetUndoCountAsync();
        Task<int> GetRedoCountAsync();
        Task<IEnumerable<string>> GetUndoStackDescriptionsAsync();
        Task<IEnumerable<string>> GetRedoStackDescriptionsAsync();

        // Batch operations
        Task BeginTransactionAsync(string transactionName);
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        // State information
        bool CanUndo { get; }
        bool CanRedo { get; }
        bool IsTransactionActive { get; }
        string CurrentTransactionName { get; }

        // Events
        event EventHandler<UndoRedoStateChangedEventArgs> StateChanged;
        event EventHandler<OperationExecutedEventArgs> OperationExecuted;
    }

    public interface IUndoableOperation
    {
        string Description { get; }
        Task ExecuteAsync();
        Task UndoAsync();
        Task RedoAsync();
    }

    public class UndoRedoStateChangedEventArgs : EventArgs
    {
        public bool CanUndo { get; set; }
        public bool CanRedo { get; set; }
        public int UndoCount { get; set; }
        public int RedoCount { get; set; }
    }

    public class OperationExecutedEventArgs : EventArgs
    {
        public IUndoableOperation Operation { get; set; }
        public bool WasUndo { get; set; }
        public bool WasRedo { get; set; }
    }
}