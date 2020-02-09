using MongoDB.Driver;
using ToDoApp.Modules.ToDos.Persistence;
using ToDoApp.Plumbing.Carter;
using static ToDoApp.Modules.ToDos.ToDoQueries.V1;

namespace ToDoApp.Modules.ToDos
{
    public class ToDoQueryApi : CarterQueryModule<IMongoDatabase>
    {
        public ToDoQueryApi()
        {
            When<GetAll, GetAll.Result>("/todo", (query, database, context) => database.Run(query, context.RequestAborted));
            When<GetNotDone, GetNotDone.Result>("/todo/pending", (query, database, context) => database.Run(query, context.RequestAborted));
            When<GetOverdue, GetOverdue.Result>("/todo/overdue", (query, database, context) => database.Run(query, context.RequestAborted));
            When<GetOne, GetOne.Result>("/todo/{id}", (query, database, context) => database.Run(query, context.RequestAborted));
        }
    }
}