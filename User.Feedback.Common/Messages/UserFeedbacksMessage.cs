using System.Collections.Generic;
using ProtoBuf;

namespace User.Feedback.Common.Messages
{
    [ProtoContract]
    public class UserFeedbacksMessage : IUserFeedbackMessage
    {
        [ProtoMember(1)]
        public IList<UserFeedback> UserFeedbacks { get; private set; }

        public UserFeedbacksMessage()
            : this (new List<UserFeedback>())
        {
        }

        public UserFeedbacksMessage(IList<UserFeedback> userFeedbacks)
        {
            UserFeedbacks = userFeedbacks;
        }
    }
}
