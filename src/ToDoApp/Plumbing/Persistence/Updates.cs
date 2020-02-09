using System;
using System.Linq.Expressions;

namespace ToDoApp.Plumbing.Persistence
{
    public abstract class UpdateBuilder<T> where T : Document
    {
        public abstract UpdateBuilder<T> Set<TField>(Expression<Func<T, TField>> field, TField value);
    }
}