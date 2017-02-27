using System;
using System.Threading;
using System.Threading.Tasks;

using User.Feedback.Common.Messages;

using ZeroMQ;

namespace User.Feedback.Common.ZeroMQ
{
    public class Connector : IConnector
    {
        private const string ConnectorEndPoint = "tcp://localhost:5563";
        private const string PublisherEndPoint = "tcp://*:5563";

        public void Publish<T>(T message) where T : IUserFeedbackMessage
        {
            using (var context = new ZContext())
            {
                using (var publisher = new ZSocket(context, ZSocketType.PUB))
                {
                    publisher.Linger = TimeSpan.Zero;
                    publisher.Bind(PublisherEndPoint);

                    using (var zMessage = new ZMessage())
                    {
                        var messageType = GetMessageType<T>();

                        zMessage.Add(new ZFrame(messageType));
                        zMessage.Add(new ZFrame(message.ToByteArray()));
                        
                        Thread.Sleep(500); // This is a dirty hack to make it work :(

                        publisher.SendMessage(zMessage);

                        Console.Out.WriteLine("The message sent: {0}", messageType);
                    }
                }
            }
        }


        public void Subscribe<T>(Action<T> callback) where T : IUserFeedbackMessage
        {
            Task.Factory.StartNew(() =>
            {
                using (var context = new ZContext())
                {
                    using (var subscriber = new ZSocket(context, ZSocketType.SUB))
                    {
                        var messageType = GetMessageType<T>();

                        subscriber.Connect(ConnectorEndPoint);
                        subscriber.Subscribe(messageType);

                        while (true)
                        {
                            using (var message = subscriber.ReceiveMessage())
                            {
                                if (messageType == message[0].ReadString())
                                {
                                    Console.Out.WriteLine("The message received: {0}", messageType);

                                    callback(message[1].Read().FromByteArray<T>());
                                }
                            }
                        }
                    }
                }
            });
        }

        public void Unsubscribe<T>() where T : IUserFeedbackMessage
        {
            using (var context = new ZContext())
            {
                using (var subscriber = new ZSocket(context, ZSocketType.SUB))
                {
                    var messageType = GetMessageType<T>();

                    subscriber.Connect(ConnectorEndPoint);
                    subscriber.Unsubscribe(messageType);
                }
            }
        }

        public Task<TResponse> RequestResponse<TRequest, TResponse>(TRequest requestMessage) where TRequest : IUserFeedbackMessage where TResponse : IUserFeedbackMessage
        {
            using (var requester = new ZSocket(ZSocketType.REQ))
            {
                requester.Connect(ConnectorEndPoint);
                requester.Send(new ZFrame(requestMessage.ToByteArray()));

                return ReceiveResponse<TResponse>(requester);
            }
        }

        public void Response<T>(T message) where T : IUserFeedbackMessage
        {
            using (var requester = new ZSocket(ZSocketType.REP))
            {
                requester.Connect(ConnectorEndPoint);
                requester.Send(new ZFrame(message.ToByteArray()));
            }
        }

        private Task<TResponse> ReceiveResponse<TResponse>(ZSocket requester) where TResponse : IUserFeedbackMessage
        {
            return Task.Factory.StartNew<TResponse>(() =>
            {
                using (var frame = requester.ReceiveFrame())
                {
                    return frame.Read().FromByteArray<TResponse>();
                }
            });
        }

        private string GetMessageType<T>()
            where T : IUserFeedbackMessage
        {
            return string.Format("{0}", typeof(T));
        }
    }
}