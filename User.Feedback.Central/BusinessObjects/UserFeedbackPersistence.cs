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
            //Connector.Subscribe<UserFeedbackMessage>(ProcessTellUserFeedbackMessage);

            //Receive<UserFeedbackMessage>(tellUserFeedback =>
            //{
            //    _userFeedbacks.Add(tellUserFeedback.UserFeedback);
            //    Sender.Tell(new UserFeedbackUpdateMessage(
            //        new UserFeedback(tellUserFeedback.UserFeedback.Message + "*", tellUserFeedback.UserFeedback.Created)));
            //});

            //Receive<RequestUserFeedbacksMessage>(request =>
            //{
            //    var result = new ResponseRequestUserFeedbacksMessage(_userFeedbacks);
            //    Sender.Tell(result, Self);
            //});
        }

        public void SaveUserFeedback(UserFeedback userFeedback)
        {
            _userFeedbacks.Add(userFeedback);
            Connector.Publish<UserFeedbackUpdateMessage>(new UserFeedbackUpdateMessage(new UserFeedback(userFeedback.Message + "*", userFeedback.Created)));
        }
    }
}
