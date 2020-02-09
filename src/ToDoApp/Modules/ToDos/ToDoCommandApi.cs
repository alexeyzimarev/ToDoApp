using ToDoApp.Plumbing.Carter;
using static ToDoApp.Modules.ToDos.ToDoCommands;

namespace ToDoApp.Modules.ToDos
{
    public class ToDoCommandApi : CarterCommandModule
    {
        public ToDoCommandApi(ExecuteCommand executeCommand) : base(executeCommand)
        {
            Post<V1.AddToDo>("/todo");
            Post<V1.Schedule>("/todo/schedule");
            Post<V1.MarkDone>("/todo/done");
            Post<V1.MarkNotDone>("/todo/undone");
        }
    }
}