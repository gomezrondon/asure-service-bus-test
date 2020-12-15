using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace user_send_microservice
{
    class Program
    {
        static ITopicClient topicClient;
        
        
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            string ServiceBusConnectionString = "<user-send Primary Connection String>";
            string TopicName = "<Topic name>";

            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);
            
            //send messages
            await SendUserMessageAsync();

            Console.ReadKey();

            await topicClient.CloseAsync();
            
            
        }
 

        static async Task SendUserMessageAsync()
        {
            List<User> users = GetDummyDataUser();
            var serializeUser = JsonConvert.SerializeObject(users);

            string messageType = "userData";
            string messageId = Guid.NewGuid().ToString();
            var message = new ServiceBusMessage
            {
                Id = messageId,
                Type = messageType,
                Content = serializeUser
            };

            var serializeBody = JsonConvert.SerializeObject(message);
            
            /// send data to bus
            var busMessage = new Message(Encoding.UTF8.GetBytes(serializeBody));
            busMessage.UserProperties.Add("Type", messageType);
            busMessage.MessageId = messageId;

            await topicClient.SendAsync(busMessage);
            
            Console.WriteLine("message has been sent");
            
        }


        public class User
        {
            public int Id { get; set; }
            public string name { get; set; }

        }

        public class ServiceBusMessage
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string Content { get; set; }
            
        }
        
        private static List<User> GetDummyDataUser()
        {
            var user = new User();
            List<User> lstUser = new List<User>();
            for (int i = 0; i < 3; i++)
            {
                user = new User();
                user.Id = i;
                user.name = "fulanito " + i;
                lstUser.Add(user);
            };

            return lstUser;
        }
        
        
    }
}
