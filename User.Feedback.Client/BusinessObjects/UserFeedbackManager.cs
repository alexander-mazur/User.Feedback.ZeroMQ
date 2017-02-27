using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

using User.Feedback.Common;
using User.Feedback.Common.Messages;
using User.Feedback.Common.ZeroMQ;

namespace User.Feedback.Client.BusinessObjects
{
    public class UserFeedbackManager : IUserFeedbackManager
    {
        public static UserFeedbackManager Instance = new UserFeedbackManager(new Connector());

        private UserFeedbackManager(IConnector connector)
        {
            _connector = connector;
        }

        private readonly IConnector _connector;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private string _lastUserFeedbackMessage = string.Empty;

        public void TellUserFeedback(UserFeedback userFeedback)
        {
            _connector.Publish<UserFeedbackMessage>(new UserFeedbackMessage(userFeedback));
        }

        public void TellBatchOfUserFeedbacks(UserFeedback userFeedback, int count)
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            IList<UserFeedback> userFeedbacks = new List<UserFeedback>();

            for (var index = 0; index < count; index++)
            {
                var newUserFeedback = new UserFeedback(string.Format("{0}:{1}", index + 1, userFeedback.Message), userFeedback.Created);

                if (index == count - 1)
                {
                    _lastUserFeedbackMessage = newUserFeedback.Message;
                }

                userFeedbacks.Add(newUserFeedback);
            }

            _connector.Publish<UserFeedbacksMessage>(new UserFeedbacksMessage(userFeedbacks));

        }

        public Task<ResponseRequestUserFeedbacksMessage> AskUserFeedbackCollection()
        {
            return _connector.RequestResponse<RequestUserFeedbacksMessage, ResponseRequestUserFeedbacksMessage>(new RequestUserFeedbacksMessage());
        }

        public void SubscribeToUserFeedbackUpdates()
        {
            _connector.Subscribe<UserFeedbackUpdateMessage>(RaiseUserFeedbackUpdate);
        }

        public void UnsubscribeFromUserFeedbackUpdates()
        {
            _connector.Unsubscribe<UserFeedbackUpdateMessage>();
        }

        private void RaiseUserFeedbackUpdate(UserFeedbackUpdateMessage obj)
        {
            RaiseUserFeedbackUpdate(obj.UserFeedback);
        }

        public void RaiseUserFeedbackUpdate(UserFeedback userFeedback)
        {
            if (UserFeedbackUpdated != null)
            {
                UserFeedbackUpdated.Invoke(this, userFeedback);
            }

            if (_stopwatch.IsRunning && userFeedback.Message.StartsWith(_lastUserFeedbackMessage))
            {
                _stopwatch.Stop();

                MessageBox.Show(string.Format("The process time of batch messages is {0} ms.", _stopwatch.ElapsedMilliseconds));
            }
        }

        public event EventHandler<UserFeedback> UserFeedbackUpdated;
    }
}