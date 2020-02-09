using System;

namespace ToDoApp.Plumbing
{
    public enum MessageHandlingStatus { Success, Failure }

    public class MessageResponse
    {
        protected MessageResponse(Exception exception, object result) {
            Exception = exception;
            Result    = result;
        }

        public Exception Exception { get; }
        public object    Result    { get; }

        public MessageHandlingStatus Status
            => Exception is null ? MessageHandlingStatus.Success : MessageHandlingStatus.Failure;

        public static MessageResponse Success(object result)
            => new MessageResponse(null, result);

        public static MessageResponse Failure(Exception exception)
            => new MessageResponse(exception, default);
    }

    public class MessageResponse<TResult> : MessageResponse
    {
        MessageResponse(Exception exception, TResult result)
            : base(exception, result) { }

        public new TResult Result => (TResult) base.Result;

        public static MessageResponse<TResult> Success(TResult result)
            => new MessageResponse<TResult>(null, result);

        public new static MessageResponse<TResult> Failure(Exception exception)
            => new MessageResponse<TResult>(exception, default);
    }
}