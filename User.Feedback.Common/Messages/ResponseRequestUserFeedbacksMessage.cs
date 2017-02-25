using System.Collections.Generic;
using ProtoBuf;

namespace User.Feedback.Common.Messages
{
    [ProtoContract]
    public class ResponseRequestUserFeedbacksMessage : IUserFeedbackMessage
    {
        [ProtoMember(1)]
        public IList<UserFeedback> UserFeedbacks { get; private set; }

        public ResponseRequestUserFeedbacksMessage()
            : this (new List<UserFeedback>())
        {
        }

        public ResponseRequestUserFeedbacksMessage(IList<UserFeedback> userFeedbacks)
        {
            UserFeedbacks = userFeedbacks;
        }
    }
}
