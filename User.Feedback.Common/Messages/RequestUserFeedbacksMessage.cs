using ProtoBuf;

namespace User.Feedback.Common.Messages
{
    [ProtoContract]
    public class RequestUserFeedbacksMessage : IUserFeedbackMessage
    {
        public RequestUserFeedbacksMessage()
        {
        }
    }
}
