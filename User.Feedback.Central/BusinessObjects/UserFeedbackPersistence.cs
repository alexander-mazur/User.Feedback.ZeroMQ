using System;
using System.Collections.Generic;

using User.Feedback.Common;
using User.Feedback.Common.Messages;
using User.Feedback.Common.ZeroMQ;

namespace User.Feedback.Central.BusinessObjects
{
    public class UserFeedbackPersistence 
    {
        private readonly IList<UserFeedback> _userFeedbacks = new List<UserFeedback>();

        private IConnector Connector { get; set; }

        public UserFeedbackPersistence(IConnector connector)
        {
            Connector = connector;
        }

        public void Initialize()
        {
            Connector.ResponseToRequest<RequestUserFeedbacksMessage, ResponseRequestUserFeedbacksMessage>(
                () => new ResponseRequestUserFeedbacksMessage(_userFeedbacks));
        }

        public void SaveUserFeedback(UserFeedback userFeedback)
        {
            _userFeedbacks.Add(userFeedback);
            Connector.Publish<UserFeedbackUpdateMessage>(new UserFeedbackUpdateMessage(new UserFeedback(userFeedback.Message + "*", userFeedback.Created)));
        }
    }
}
