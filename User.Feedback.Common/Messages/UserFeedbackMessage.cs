
using ProtoBuf;

namespace User.Feedback.Common.Messages
{
    [ProtoContract]
    public class UserFeedbackMessage : IUserFeedbackMessage
    {
        [ProtoMember(1)]
        public UserFeedback UserFeedback { get; private set; }

        public UserFeedbackMessage()
            : this(new UserFeedback())
        {
        }

        public UserFeedbackMessage(UserFeedback userFeedback)
        {
            UserFeedback = userFeedback;
        }
    }
}
