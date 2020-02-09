using System;
using System.Collections.Generic;

namespace ToDoApp.Modules.ToDos
{
    public static class ToDoQueries
    {
        public static class V1
        {
            public class GetOne
            {
                public string Id { get; set; }
                
                public class Result : ToDoResult { }
            }

            public class GetAll
            {
                public class Result : List<ToDoResult> { }
            }

            public class GetNotDone
            {
                public class Result : List<ToDoResult> { }
            }

            public class GetOverdue
            {
                public class Result : List<ToDoResult> { }
            }

            public class ToDoResult
            {
                public string   Id       { get; set; }
                public string   WhatToDo { get; set; }
                public DateTime DueAt    { get; set; }
                public bool     Done     { get; set; }
            }
        }
    }
}