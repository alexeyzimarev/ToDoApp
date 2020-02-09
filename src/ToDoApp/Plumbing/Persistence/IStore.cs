using System;
using System.Threading;
using System.Threading.Tasks;

namespace ToDoApp.Plumbing.Persistence
{
    public interface IStore<T> where T : Document
    {
        Task<T> Load(string id, CancellationToken cancellationToken);
        Task Add(T document, CancellationToken cancellationToken);
        Task Update(string id, Action<UpdateBuilder<T>> update, CancellationToken cancellationToken);
    }
}