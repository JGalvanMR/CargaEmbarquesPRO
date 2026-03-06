using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


using System.IO;
using System.Net;

namespace CargaEmbarques
{
    public class WebhookServer
    {
        private readonly HttpListener _listener = new HttpListener();

        public WebhookServer()
        {
            _listener.Prefixes.Add("http://0.0.0.0:5000/");
        }

        public async void Start()
        {
            _listener.Start();
            Console.WriteLine("🚀 Servidor HTTP iniciado en http://0.0.0.0:5000/");

            while (true)
            {
                var context = await _listener.GetContextAsync();
                var request = context.Request;

                using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                string requestBody = await reader.ReadToEndAsync();
                Console.WriteLine($"📩 Mensaje recibido: {requestBody}");

                var response = Encoding.UTF8.GetBytes("{\"text\": \"Mensaje recibido correctamente\"}");
                context.Response.OutputStream.Write(response, 0, response.Length);
                context.Response.Close();
            }
        }

        public void Stop()
        {
            _listener.Stop();
        }
    }
}