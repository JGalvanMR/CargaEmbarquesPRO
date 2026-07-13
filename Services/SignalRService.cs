using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Android.Util;
using Newtonsoft.Json.Linq; // <-- AGREGADO: Necesario para usar JObject en lugar de dynamic

namespace CargaEmbarques.Services
{
    public class SignalRService : IDisposable
    {
        private HubConnection _connection;
        private readonly string _hubUrl;
        private bool _isStarted = false;

        // Evento que se dispara cuando se recibe una autorización remota
        public event EventHandler<FolioAutorizadoEventArgs> OnFolioAutorizado;

        // Evento opcional para cambios de estado de conexión
        public event EventHandler<string> OnConnectionStateChanged;

        public SignalRService(string baseUrl)
        {
            // baseUrl debe ser la misma que usas en ATUServices (ej. "http://192.168.123.244:83/auth")
            _hubUrl = $"{baseUrl.TrimEnd('/')}/audit-hub";
        }

        public async Task StartAsync()
        {
            if (_connection != null && _connection.State == HubConnectionState.Connected)
                return;

            try
            {
                _connection = new HubConnectionBuilder()
                    .WithUrl(_hubUrl)
                    .WithAutomaticReconnect(new[]
                    {
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(30)
                    })
                    .Build();

                // ✅ CORREGIDO: Cambiado de 'dynamic' a 'JObject'
                _connection.On<JObject>("FolioAutorizado", data =>
                {
                    var args = new FolioAutorizadoEventArgs
                    {
                        // ✅ CORREGIDO: Acceder a las propiedades mediante indexadores de JObject
                        EmbFolio = data["embFolio"]?.ToString() ?? string.Empty,
                        BatchId = data["batchId"]?.ToString() ?? string.Empty,
                        SupervisorId = data["supervisorId"]?.ToString() ?? string.Empty,
                        AuthorizedAt = DateTime.TryParse(data["authorizedAt"]?.ToString(), out DateTime dt) ? dt : DateTime.Now,
                        Message = data["message"]?.ToString() ?? string.Empty,
                        Comments = data["comments"]?.ToString() ?? string.Empty
                    };

                    // Disparar evento en el hilo UI
                    Android.App.Application.SynchronizationContext.Post(_ =>
                    {
                        OnFolioAutorizado?.Invoke(this, args);
                    }, null);
                });

                // Manejar eventos de conexión
                _connection.Closed += (error) =>
                {
                    OnConnectionStateChanged?.Invoke(this, "Desconectado");
                    return Task.CompletedTask;
                };

                _connection.Reconnecting += (error) =>
                {
                    OnConnectionStateChanged?.Invoke(this, "Reconectando...");
                    return Task.CompletedTask;
                };

                _connection.Reconnected += (connectionId) =>
                {
                    OnConnectionStateChanged?.Invoke(this, "Conectado");
                    return Task.CompletedTask;
                };

                await _connection.StartAsync();
                _isStarted = true;
                Log.Info("SignalR", "Conectado al hub de auditoría");
                OnConnectionStateChanged?.Invoke(this, "Conectado");
            }
            catch (Exception ex)
            {
                Log.Error("SignalR", $"Error al conectar: {ex.Message}");
                throw;
            }
        }

        public async Task StopAsync()
        {
            if (_connection != null && _connection.State == HubConnectionState.Connected)
            {
                await _connection.StopAsync();
                _isStarted = false;
            }
        }

        public bool IsConnected => _connection?.State == HubConnectionState.Connected;

        public void Dispose()
        {
            _connection?.DisposeAsync().AsTask().Wait();
        }
    }

    public class FolioAutorizadoEventArgs : EventArgs
    {
        public string EmbFolio { get; set; }
        public string BatchId { get; set; }
        public string SupervisorId { get; set; }
        public DateTime AuthorizedAt { get; set; }
        public string Message { get; set; }
        public string Comments { get; set; }
    }
}