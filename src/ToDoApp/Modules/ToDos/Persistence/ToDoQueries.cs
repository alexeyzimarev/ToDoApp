using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<ToDoApp.Modules.ToDos.Persistence.ToDoDocument>;
using static ToDoApp.Modules.ToDos.ToDoQueries;

namespace ToDoApp.Modules.ToDos.Persistence
{
    public static class ToDoQueries
    {
        public static async Task<V1.GetOne.Result> Run(this IMongoDatabase database, V1.GetOne query, CancellationToken cancellationToken)
        {
            var document = await database.ToDos().Find(Filter.Eq(x => x.Id, query.Id)).FirstOrDefaultAsync(cancellationToken);

            return document?.Map<V1.GetOne.Result>();
        }

        public static Task<V1.GetAll.Result> Run(this IMongoDatabase database, V1.GetAll _, CancellationToken cancellationToken)
            => database.QueryToResult<V1.GetAll.Result>(Filter.Empty, cancellationToken);

        public static Task<V1.GetOverdue.Result> Run(this IMongoDatabase database, V1.GetOverdue _, CancellationToken cancellationToken)
            => database.QueryToResult<V1.GetOverdue.Result>(Filter.Gt(x => x.DueAt, DateTime.Today), cancellationToken);

        public static Task<V1.GetNotDone.Result> Run(this IMongoDatabase database, V1.GetNotDone _, CancellationToken cancellationToken)
            => database.QueryToResult<V1.GetNotDone.Result>(Filter.Eq(x => x.Done, false), cancellationToken);

        static IMongoCollection<ToDoDocument> ToDos(this IMongoDatabase database)
            => database.GetCollection<ToDoDocument>(nameof(ToDoDocument).Replace("Document", ""));

        static async Task<T> QueryToResult<T>(
            this IMongoDatabase database, FilterDefinition<ToDoDocument> filter, CancellationToken cancellationToken
        )
            where T : List<V1.ToDoResult>, new()
        {
            var documents = await database.ToDos().Find(filter).ToListAsync(cancellationToken);

            return documents.Map<T>();
        }

        static T Map<T>(this ToDoDocument document) where T : V1.ToDoResult, new()
            => new T
            {
                Id       = document.Id,
                WhatToDo = document.WhatToDo,
                DueAt    = document.DueAt,
                Done     = document.Done
            };

        static T Map<T>(this IEnumerable<ToDoDocument> documents) where T : List<V1.ToDoResult>, new()
        {
            var result = new T();
            result.AddRange(documents.Select(x => x.Map<V1.ToDoResult>()));
            return result;
        }
    }
}