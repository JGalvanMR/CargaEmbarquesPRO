using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Android.Util;
using Android.OS;

namespace CargaEmbarques.Services
{
    public class SignalRService : IDisposable
    {
        private HubConnection _connection;
        private readonly string _hubUrl;
        private readonly Handler _mainHandler;

        public event EventHandler<FolioAutorizadoEventArgs> OnFolioAutorizado;
        public event EventHandler<string> OnConnectionStateChanged;

        // ✅ NUEVO: Clase interna nativa para que System.Text.Json no tenga problemas
        private class SignalRFolioDto
        {
            public string EmbFolio { get; set; }
            public string BatchId { get; set; }
            public string SupervisorId { get; set; }
            public DateTime AuthorizedAt { get; set; }
            public string Message { get; set; }
            public string Comments { get; set; }
        }

        public SignalRService(string baseUrl)
        {
            _hubUrl = $"{baseUrl.TrimEnd('/')}/audit-hub";
            _mainHandler = new Handler(Looper.MainLooper);
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

                // ✅ CORREGIDO: Escuchar usando la clase nativa en lugar de JObject
                _connection.On<SignalRFolioDto>("FolioAutorizado", data =>
                {
                    try
                    {
                        Log.Info("SignalR_Carga", $"🔥🔥🔥 MENSAJE CRUDO RECIBIDO. Folio: {data?.EmbFolio}, Supervisor: {data?.SupervisorId}");

                        var args = new FolioAutorizadoEventArgs
                        {
                            EmbFolio = data.EmbFolio ?? string.Empty,
                            BatchId = data.BatchId ?? string.Empty,
                            SupervisorId = data.SupervisorId ?? string.Empty,
                            AuthorizedAt = data.AuthorizedAt,
                            Message = data.Message ?? string.Empty,
                            Comments = data.Comments ?? string.Empty
                        };

                        _mainHandler.Post(() =>
                        {
                            Log.Info("SignalR_Carga", "✅ Disparando evento a la UI del diálogo...");
                            OnFolioAutorizado?.Invoke(this, args);
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Error("SignalR_Carga", $"Error procesando DTO: {ex.Message}");
                    }
                });

                // Para probar si el celular recibe OTROS eventos (como el del PWA)
                _connection.On<string>("AuditEvent", rawMessage =>
                {
                    Log.Info("SignalR_Carga", $"📡 Evento de Auditoría recibido (crudo): {rawMessage}");
                });

                _connection.Closed += (error) =>
                {
                    Log.Warn("SignalR_Carga", $"❌ Conexión cerrada. Error: {error?.Message}");
                    _mainHandler.Post(() => OnConnectionStateChanged?.Invoke(this, "Desconectado"));
                    return Task.CompletedTask;
                };

                _connection.Reconnecting += (error) =>
                {
                    _mainHandler.Post(() => OnConnectionStateChanged?.Invoke(this, "Reconectando..."));
                    return Task.CompletedTask;
                };

                _connection.Reconnected += (connectionId) =>
                {
                    _mainHandler.Post(() => OnConnectionStateChanged?.Invoke(this, "Conectado"));
                    return Task.CompletedTask;
                };

                await _connection.StartAsync();
                Log.Info("SignalR_Carga", "✅ Conectado al hub de auditoría");
                _mainHandler.Post(() => OnConnectionStateChanged?.Invoke(this, "Conectado"));
            }
            catch (Exception ex)
            {
                Log.Error("SignalR_Carga", $"Error al conectar: {ex.Message}");
                _mainHandler.Post(() => OnConnectionStateChanged?.Invoke(this, $"Error: {ex.Message}"));
                throw;
            }
        }

        public async Task StopAsync()
        {
            if (_connection != null && _connection.State == HubConnectionState.Connected)
            {
                await _connection.StopAsync();
            }
        }

        public bool IsConnected => _connection?.State == HubConnectionState.Connected;

        public void Dispose()
        {
            try { _connection?.DisposeAsync().AsTask().Wait(); } catch { }
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