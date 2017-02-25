using User.Feedback.Central.BusinessObjects;
using User.Feedback.Common.ZeroMQ;

namespace User.Feedback.Central
{
    public class UserFeedbackCentralEntry
    {
        public IConnector Connector { get; set; }

        public UserFeedbackProcessor UserFeedbackProcessor { get; set; }

        public UserFeedbackPersistence UserFeedbackPersistence { get; set; }

        public void Initialize()
        {
            Connector = new Connector();

            UserFeedbackPersistence = new UserFeedbackPersistence(Connector);
            UserFeedbackPersistence.Initialize();

            UserFeedbackProcessor = new UserFeedbackProcessor(Connector, UserFeedbackPersistence);
            UserFeedbackProcessor.Initialize();
        }
    }
}