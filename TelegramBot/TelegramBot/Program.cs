using System;

namespace TelegramBot
{
    class Program
    {
        static void Main(string[] args)
        {
            //5095334683:AAHIJMdN6BDLG7VpyKa-RRRe1ndb2Pq3du4
            try
            {
                TelegramBotHelper hlp = new TelegramBotHelper(token: "5095334683:AAGtzLXy6s-5u7Wlgin0HEcieKaxh8KfqGc");
                hlp.GetUpdates();
            }catch(Exception ex)
            {
                Console.WriteLine("Error: "+ex.Message);
            }
        }
    }
}
