using System;
using ToDoApp.Plumbing.Persistence;

namespace ToDoApp.Modules.ToDos.Persistence
{
    public class ToDoDocument : Document
    {
        public string   WhatToDo    { get; set; }
        public DateTime DueAt       { get; set; }
        public bool     Done        { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}