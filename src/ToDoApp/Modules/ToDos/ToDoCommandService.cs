using System;
using System.Threading;
using System.Threading.Tasks;
using ToDoApp.Modules.ToDos.Persistence;
using ToDoApp.Plumbing;
using ToDoApp.Plumbing.Persistence;
using static ToDoApp.Modules.ToDos.ToDoCommands.V1;

namespace ToDoApp.Modules.ToDos
{
    public class ToDoCommandService
    {
        readonly IStore<ToDoDocument> _store;

        public ToDoCommandService(IStore<ToDoDocument> store) => _store = store;

        public async Task<MessageResponse> Handle(object command, CancellationToken cancellationToken)
        {
            var task = command switch
            {
                AddToDo cmd => _store.Add(
                    new ToDoDocument
                    {
                        Id       = cmd.Id,
                        WhatToDo = cmd.WhatToDo,
                        DueAt    = Parse(cmd.DueAt)
                    },
                    cancellationToken
                ),
                Schedule cmd    => _store.Update(cmd.Id, update => update.Set(x => x.DueAt, Parse(cmd.NewDueAt)), cancellationToken),
                MarkDone cmd    => _store.Update(cmd.Id, update => update.Set(x => x.Done, true), cancellationToken),
                MarkNotDone cmd => _store.Update(cmd.Id, update => update.Set(x => x.Done, false), cancellationToken),
                _               => default
            };

            if (task != default)
                await task;

            return MessageResponse.Success(new { });

            static DateTime Parse(string dateTime) => DateTime.TryParse(dateTime, out var dt) ? dt : default;
        }
    }
}