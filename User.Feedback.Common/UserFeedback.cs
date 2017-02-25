using System;
using ProtoBuf;

namespace User.Feedback.Common
{
    [ProtoContract]
    public class UserFeedback
    {
        [ProtoMember(1)]
        public string Message { get; private set; }

        [ProtoMember(2)]
        public DateTime Created { get; private set; }

        public UserFeedback()
            : this(string.Empty, DateTime.Now)
        {
        }

        public UserFeedback(string message, DateTime createDateTime)
        {
            Message = message;
            Created = createDateTime;
        }
    }
}

