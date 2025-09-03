// MyPdfEditor.Core/Services/Implementations/UndoRedoService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyPdfEditor.Core.Services.Interfaces;

namespace MyPdfEditor.Core.Services.Implementations
{
    public class UndoRedoService : IUndoRedoService
    {
        private readonly Stack<IUndoableOperation> _undoStack = new Stack<IUndoableOperation>();
        private readonly Stack<IUndoableOperation> _redoStack = new Stack<IUndoableOperation>();
        private readonly Stack<Transaction> _transactionStack = new Stack<Transaction>();

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;
        public bool IsTransactionActive => _transactionStack.Count > 0;
        public string CurrentTransactionName => _transactionStack.Count > 0 ? _transactionStack.Peek().Name : string.Empty;

        public event EventHandler<UndoRedoStateChangedEventArgs> StateChanged;
        public event EventHandler<OperationExecutedEventArgs> OperationExecuted;

        public async Task PushOperationAsync(IUndoableOperation operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            if (IsTransactionActive)
            {
                _transactionStack.Peek().Operations.Add(operation);
            }
            else
            {
                _undoStack.Push(operation);
                _redoStack.Clear();
                OnStateChanged();
            }

            await operation.ExecuteAsync();
            OnOperationExecuted(operation, false, false);
        }

        public async Task<bool> UndoAsync()
        {
            if (!CanUndo) return false;

            IUndoableOperation operation;
            if (IsTransactionActive && _transactionStack.Peek().Operations.Count > 0)
            {
                operation = _transactionStack.Peek().Operations.Last();
                _transactionStack.Peek().Operations.RemoveAt(_transactionStack.Peek().Operations.Count - 1);
            }
            else
            {
                operation = _undoStack.Pop();
            }

            await operation.UndoAsync();
            _redoStack.Push(operation);

            OnStateChanged();
            OnOperationExecuted(operation, true, false);

            return true;
        }

        public async Task<bool> RedoAsync()
        {
            if (!CanRedo) return false;

            var operation = _redoStack.Pop();
            await operation.RedoAsync();

            if (IsTransactionActive)
            {
                _transactionStack.Peek().Operations.Add(operation);
            }
            else
            {
                _undoStack.Push(operation);
            }

            OnStateChanged();
            OnOperationExecuted(operation, false, true);

            return true;
        }

        public async Task BeginTransactionAsync(string transactionName)
        {
            _transactionStack.Push(new Transaction(transactionName));
            OnStateChanged();
            await Task.CompletedTask;
        }

        public async Task CommitTransactionAsync()
        {
            if (!IsTransactionActive)
                throw new InvalidOperationException("No active transaction to commit");

            var transaction = _transactionStack.Pop();

            if (transaction.Operations.Count > 0)
            {
                var compositeOperation = new CompositeOperation(transaction.Name, transaction.Operations);
                _undoStack.Push(compositeOperation);
                _redoStack.Clear();
            }

            OnStateChanged();
            await Task.CompletedTask;
        }

        public async Task RollbackTransactionAsync()
        {
            if (!IsTransactionActive)
                throw new InvalidOperationException("No active transaction to rollback");

            var transaction = _transactionStack.Pop();

            // Undo all operations in reverse order
            for (int i = transaction.Operations.Count - 1; i >= 0; i--)
            {
                await transaction.Operations[i].UndoAsync();
            }

            OnStateChanged();
            await Task.CompletedTask;
        }

        public async Task ClearHistoryAsync()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            _transactionStack.Clear();

            OnStateChanged();
            await Task.CompletedTask;
        }

        public async Task<int> GetUndoCountAsync()
        {
            return await Task.FromResult(_undoStack.Count);
        }

        public async Task<int> GetRedoCountAsync()
        {
            return await Task.FromResult(_redoStack.Count);
        }

        public async Task<IEnumerable<string>> GetUndoStackDescriptionsAsync()
        {
            return await Task.FromResult(_undoStack.Select(op => op.Description));
        }

        public async Task<IEnumerable<string>> GetRedoStackDescriptionsAsync()
        {
            return await Task.FromResult(_redoStack.Select(op => op.Description));
        }

        protected virtual void OnStateChanged()
        {
            StateChanged?.Invoke(this, new UndoRedoStateChangedEventArgs
            {
                CanUndo = CanUndo,
                CanRedo = CanRedo,
                UndoCount = _undoStack.Count,
                RedoCount = _redoStack.Count
            });
        }

        protected virtual void OnOperationExecuted(IUndoableOperation operation, bool wasUndo, bool wasRedo)
        {
            OperationExecuted?.Invoke(this, new OperationExecutedEventArgs
            {
                Operation = operation,
                WasUndo = wasUndo,
                WasRedo = wasRedo
            });
        }

        private class Transaction
        {
            public string Name { get; }
            public List<IUndoableOperation> Operations { get; } = new List<IUndoableOperation>();

            public Transaction(string name)
            {
                Name = name;
            }
        }

        private class CompositeOperation : IUndoableOperation
        {
            private readonly List<IUndoableOperation> _operations;

            public string Description { get; }

            public CompositeOperation(string description, IEnumerable<IUndoableOperation> operations)
            {
                Description = description;
                _operations = operations.ToList();
            }

            public async Task ExecuteAsync()
            {
                foreach (var operation in _operations)
                {
                    await operation.ExecuteAsync();
                }
            }

            public async Task UndoAsync()
            {
                for (int i = _operations.Count - 1; i >= 0; i--)
                {
                    await _operations[i].UndoAsync();
                }
            }

            public async Task RedoAsync()
            {
                foreach (var operation in _operations)
                {
                    await operation.RedoAsync();
                }
            }
        }
    }

    // Implementación base para operaciones comunes
    public abstract class UndoableOperation : IUndoableOperation
    {
        public string Description { get; protected set; }

        protected UndoableOperation(string description)
        {
            Description = description;
        }

        public abstract Task ExecuteAsync();
        public abstract Task UndoAsync();
        public abstract Task RedoAsync();
    }

    // Operación para cambios de propiedades
    public class PropertyChangeOperation<T> : UndoableOperation
    {
        private readonly Action<T> _setter;
        private readonly T _oldValue;
        private readonly T _newValue;

        public PropertyChangeOperation(string description, Action<T> setter, T oldValue, T newValue)
            : base(description)
        {
            _setter = setter;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override async Task ExecuteAsync()
        {
            _setter(_newValue);
            await Task.CompletedTask;
        }

        public override async Task UndoAsync()
        {
            _setter(_oldValue);
            await Task.CompletedTask;
        }

        public override async Task RedoAsync()
        {
            _setter(_newValue);
            await Task.CompletedTask;
        }
    }

    // Operación para agregar elementos
    public class AddOperation<T> : UndoableOperation
    {
        private readonly IList<T> _collection;
        private readonly T _item;
        private readonly Action<T> _onAdd;
        private readonly Action<T> _onRemove;

        public AddOperation(string description, IList<T> collection, T item, Action<T> onAdd = null, Action<T> onRemove = null)
            : base(description)
        {
            _collection = collection;
            _item = item;
            _onAdd = onAdd;
            _onRemove = onRemove;
        }

        public override async Task ExecuteAsync()
        {
            _collection.Add(_item);
            _onAdd?.Invoke(_item);
            await Task.CompletedTask;
        }

        public override async Task UndoAsync()
        {
            _collection.Remove(_item);
            _onRemove?.Invoke(_item);
            await Task.CompletedTask;
        }

        public override async Task RedoAsync()
        {
            await ExecuteAsync();
        }
    }

    // Operación para remover elementos
    public class RemoveOperation<T> : UndoableOperation
    {
        private readonly IList<T> _collection;
        private readonly T _item;
        private readonly int _index;
        private readonly Action<T> _onAdd;
        private readonly Action<T> _onRemove;

        public RemoveOperation(string description, IList<T> collection, T item, Action<T> onAdd = null, Action<T> onRemove = null)
            : base(description)
        {
            _collection = collection;
            _item = item;
            _index = collection.IndexOf(item);
            _onAdd = onAdd;
            _onRemove = onRemove;
        }

        public override async Task ExecuteAsync()
        {
            _collection.Remove(_item);
            _onRemove?.Invoke(_item);
            await Task.CompletedTask;
        }

        public override async Task UndoAsync()
        {
            if (_index >= 0 && _index <= _collection.Count)
            {
                _collection.Insert(_index, _item);
            }
            else
            {
                _collection.Add(_item);
            }
            _onAdd?.Invoke(_item);
            await Task.CompletedTask;
        }

        public override async Task RedoAsync()
        {
            await ExecuteAsync();
        }
    }
}