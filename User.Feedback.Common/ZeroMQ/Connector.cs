using System;
using System.Threading;
using System.Threading.Tasks;

using User.Feedback.Common.Messages;

using ZeroMQ;

namespace User.Feedback.Common.ZeroMQ
{
    public class Connector : IConnector
    {
        private const string SubEndPoint = "tcp://localhost:5563";
        private const string PubEndPoint = "tcp://*:5563";

        private const string ReqEndPoint = "tcp://localhost:5564";
        private const string RepEndPoint = "tcp://*:5564";

        public void Publish<T>(T message) 
            where T : IUserFeedbackMessage
        {
            using (var context = new ZContext())
            {
                using (var publisher = new ZSocket(context, ZSocketType.PUB))
                {
                    publisher.Linger = TimeSpan.Zero;
                    publisher.Bind(PubEndPoint);

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


        public void Subscribe<T>(Action<T> callback) 
            where T : IUserFeedbackMessage
        {
            Task.Factory.StartNew(() =>
            {
                using (var context = new ZContext())
                {
                    using (var subscriber = new ZSocket(context, ZSocketType.SUB))
                    {
                        var messageType = GetMessageType<T>();

                        subscriber.Connect(SubEndPoint);
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

        public void Unsubscribe<T>() 
            where T : IUserFeedbackMessage
        {
            using (var context = new ZContext())
            {
                using (var subscriber = new ZSocket(context, ZSocketType.SUB))
                {
                    var messageType = GetMessageType<T>();

                    subscriber.Connect(SubEndPoint);
                    subscriber.Unsubscribe(messageType);
                }
            }
        }

        public Task<TResponse> RequestResponse<TRequest, TResponse>(TRequest requestMessage) 
            where TRequest : IUserFeedbackMessage 
            where TResponse : IUserFeedbackMessage
        {
            return Task.Factory.StartNew<TResponse>(() =>
            {
                using (var context = new ZContext())
                {
                    using (var requester = new ZSocket(context, ZSocketType.REQ))
                    {
                        requester.Connect(ReqEndPoint);
                        requester.Send(new ZFrame(requestMessage.ToByteArray()));

                        Console.Out.WriteLine("The request sent: {0}", requestMessage);

                        using (var frame = requester.ReceiveFrame())
                        {
                            Console.Out.WriteLine("The response for request received!");

                            return frame.Read().FromByteArray<TResponse>();
                        }
                    }
                }
            });
        }

        public void ResponseToRequest<TRequest, TResponse>(Func<TResponse> callback)
            where TRequest : IUserFeedbackMessage
            where TResponse : IUserFeedbackMessage
        {
            Task.Factory.StartNew(() =>
            {
                using (var context = new ZContext())
                {
                    using (var responder = new ZSocket(context, ZSocketType.REP))
                    {
                        responder.Bind(RepEndPoint);

                        while (true)
                        {
                            using (var frame = responder.ReceiveFrame())
                            {
                                var request = frame.Read().FromByteArray<TRequest>();

                                Console.Out.WriteLine("The request received: {0}", request);

                                if (request != null)
                                {
                                    var response = callback();
                                    responder.Send(new ZFrame(response.ToByteArray()));
                                }
                            }
                        }
                    }
                }
            });
        }

        private static string GetMessageType<T>()
            where T : IUserFeedbackMessage
        {
            return string.Format("{0}", typeof(T));
        }
    }
}