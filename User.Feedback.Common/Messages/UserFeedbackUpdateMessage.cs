
using ProtoBuf;

namespace User.Feedback.Common.Messages
{
    [ProtoContract]
    public class UserFeedbackUpdateMessage : IUserFeedbackMessage
    {
        [ProtoMember(1)]
        public UserFeedback UserFeedback { get; private set; }

        public UserFeedbackUpdateMessage()
            : this(new UserFeedback())
        {
        }
        
        public UserFeedbackUpdateMessage(UserFeedback userFeedback)
        {
            UserFeedback = userFeedback;
        }
    }
}
