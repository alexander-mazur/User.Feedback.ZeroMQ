using User.Feedback.Common.Messages;
using User.Feedback.Common.ZeroMQ;

namespace User.Feedback.Central.BusinessObjects
{
    public class UserFeedbackProcessor 
    {
        private IConnector Connector { get; set; }
        private UserFeedbackPersistence UserFeedbackPersistence { get; set; }

        public UserFeedbackProcessor(IConnector connector, UserFeedbackPersistence userFeedbackPersistence)
        {
            Connector = connector;
            UserFeedbackPersistence = userFeedbackPersistence;
        }

        public void Initialize()
        {
            Connector.Subscribe<UserFeedbackMessage>(ProcessTellUserFeedbackMessage);
        }

        private void ProcessTellUserFeedbackMessage(UserFeedbackMessage userFeedbackMessage)
        {
            UserFeedbackPersistence.SaveUserFeedback(userFeedbackMessage.UserFeedback);
        }
    }
}
