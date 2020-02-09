using FluentValidation;

namespace ToDoApp.Modules.ToDos
{
    public static class Validations
    {
        public static class V1
        {
            public class AddToDoValidator : AbstractValidator<ToDoCommands.V1.AddToDo>
            {
                public AddToDoValidator()
                {
                    RuleFor(x => x.Id).NotEmpty();
                    RuleFor(x => x.WhatToDo).NotEmpty();
                }
            }
        }
    }
}