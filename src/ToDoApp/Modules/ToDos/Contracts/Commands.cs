using System;

namespace ToDoApp.Modules.ToDos
{
    public static class ToDoCommands
    {
        public static class V1
        {
            public class AddToDo
            {
                public string Id       { get; set; }
                public string WhatToDo { get; set; }
                public string DueAt    { get; set; }
            }

            public class Schedule
            {
                public string Id       { get; set; }
                public string NewDueAt { get; set; }
            }

            public class MarkDone
            {
                public string Id { get; set; }
            }

            public class MarkNotDone
            {
                public string Id { get; set; }
            }
        }
    }
}