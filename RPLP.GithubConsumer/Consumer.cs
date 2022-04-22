using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.GithubConsumer
{
    public class Consumer
    {
        private static HttpClient _httpClient;
        private static ManualResetEvent waitHandle = new ManualResetEvent(false);

        public Consumer(string p_token)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.github.com");
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("RPLP", "1.0"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", p_token);
        }

        public void StartUI()
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
            using (IConnection connexion = factory.CreateConnection())
            {
                using (IModel channel = connexion.CreateModel())
                {
                    channel.QueueDeclare(queue: "rplp-github", durable: false, exclusive: false,
                    autoDelete: false, arguments: null);

                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        byte[] donnees = ea.Body.ToArray();
                        string message = Encoding.UTF8.GetString(donnees);

                        string? translatedMessage = JsonConvert.DeserializeObject<string>(message);



                        channel.BasicAck(ea.DeliveryTag, false);
                    };
                    channel.BasicConsume(queue: "rplp-github",
                    autoAck: false,
                    consumer: consumer
                    );
                    waitHandle.WaitOne();
                }
            }

        }
    }
}
