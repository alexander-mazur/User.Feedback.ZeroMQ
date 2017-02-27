using System;
using System.Threading.Tasks;

using User.Feedback.Common.Messages;

namespace User.Feedback.Common.ZeroMQ
{
    public interface IConnector
    {
        void Publish<T>(T message) 
            where T : IUserFeedbackMessage;

        void Subscribe<T>(Action<T> callback) 
            where T : IUserFeedbackMessage;

        void Unsubscribe<T>()
            where T : IUserFeedbackMessage;

        Task<TResponse> RequestResponse<TRequest, TResponse>(TRequest requestMessage)
            where TResponse : IUserFeedbackMessage
            where TRequest : IUserFeedbackMessage;

        void ResponseToRequest<TRequest, TResponse>(Func<TResponse> callback)
            where TRequest : IUserFeedbackMessage
            where TResponse : IUserFeedbackMessage;
    }
}