using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace ToDoApp.Plumbing
{
    public class ValidationException : Exception
    {
        public string[] Errors { get; }
        
        public ValidationException(IList<ValidationFailure> errors) : base("Validation failed") => Errors = errors.Select(x => x.ErrorMessage).ToArray();
    }
}