using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ToDoApp.Plumbing.Persistence;

namespace ToDoApp.Infrastructure
{
    public class MongoStore<T> : IStore<T> where T : Document
    {
        readonly IMongoCollection<T> _collection;

        public MongoStore(IMongoDatabase database) => _collection = database.GetCollection<T>(typeof(T).Name.Replace("Document", ""));

        public Task<T> Load(string id, CancellationToken cancellationToken) 
            => _collection.AsQueryable().Where(x => x.Id == id).SingleOrDefaultAsync(cancellationToken);

        public Task Add(T document, CancellationToken cancellationToken) 
            => _collection.InsertOneAsync(document, new InsertOneOptions(), cancellationToken);
        
        public Task Update(string id, Action<UpdateBuilder<T>> update, CancellationToken cancellationToken)
        {
            var builder = new MongoUpdateBuilder();
            update(builder);
            return _collection.UpdateOneAsync(Builders<T>.Filter.Eq(x => x.Id, id), builder.Build(), new UpdateOptions(), cancellationToken);
        }

        class MongoUpdateBuilder : UpdateBuilder<T>
        {
            readonly UpdateDefinitionBuilder<T> _builder;
            UpdateDefinition<T> _update;

            internal MongoUpdateBuilder() => _builder = Builders<T>.Update;
            
            public override UpdateBuilder<T> Set<TField>(Expression<Func<T, TField>> field, TField value)
            {
                if (_update != null)
                {
                    _update.Set(field, value);
                }
                else
                {
                    _update = _builder.Set(field, value);
                }

                return this;
            }

            public UpdateDefinition<T> Build() => _update;
        }
    }
}