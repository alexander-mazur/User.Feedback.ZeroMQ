using System.IO;

using ProtoBuf;

using User.Feedback.Common.Messages;

namespace User.Feedback.Common
{
    public static class UserFeedbackExtensions
    {
        public static byte[] ToByteArray<T>(this T @object)
            where T : IUserFeedbackMessage
        {
            if (@object == null)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, @object);
                return memoryStream.ToArray();
            }
        }

        public static T FromByteArray<T>(this byte[] bytes)
            where T : IUserFeedbackMessage
        {
            if (bytes == null)
            {
                return default(T);
            }

            using (var memoryStream = new MemoryStream(bytes))
            {
                return Serializer.Deserialize<T>(memoryStream);
            }
        }
    }
}

