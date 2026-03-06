using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CargaEmbarques
{
    public class TeamsNotifier
    {
        private readonly string webhookUrl;
        private static readonly HttpClient client = new HttpClient();

        public TeamsNotifier(string url)
        {
            webhookUrl = url;
        }

        // Método sincrónico sin bloquear la UI
        public void SendAdaptiveCard(string cardJson)
        {
            Task.Run(async () => await SendMessageInternal(cardJson)).Wait();
        }

        private async Task SendMessageInternal(string cardJson)
        {
            try
            {
                var payload = new { type = "message", attachments = new[] { new { contentType = "application/vnd.microsoft.card.adaptive", content = JsonConvert.DeserializeObject(cardJson) } } };
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(webhookUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al enviar mensaje: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al enviar mensaje: {ex.Message}");
            }
        }
    }
}
