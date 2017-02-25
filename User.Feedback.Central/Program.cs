using System;

namespace User.Feedback.Central
{
    class Program
    {
        static void Main(string[] args)
        {
            var entry = new UserFeedbackCentralEntry();
            entry.Initialize();

            Console.WriteLine("User.Feedback Central Application started.");
            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }
    }
}
