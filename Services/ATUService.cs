// CargaEmbarques/Services/ATUService.cs
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CargaEmbarques.Services
{

    //380410 tarima 4 15 cajas
    public class ATUService
    {
        private readonly HttpClient _http;
        //private const string ATU_URL = "http://192.168.123.155:5059"; // Tu IP del servidor ATU
        private const string ATU_URL = "http://192.168.123.155:5059"; // Tu IP del servidor ATU

        public ATUService()
        {
            _http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        }

        /// <summary>
        /// Crea la solicitud de autorización en el backend ATU.
        /// El backend notifica automáticamente a ATU.CamaraFria via SignalR.
        /// </summary>
        public async Task<bool> CrearSolicitudAsync(
            string embFolio,
            string reciboCap,      // FolioLeido  — el que FIFO dice que debe salir
            string reciboSug,      // FolioAtrasado — el adelantado
            string fechaRecCap,    // FechaLeido
            string fechaRecSug,    // FechaAtrasada
            string prodClave,      // Productocve
            string producto,       // Producto
            string cantidad,       // CajasDisp
            string tarimaCap,      // TarimaLeido
            string tarimaSug,      // TarimaAtrasada
            string responsable,
            string motivo,
            string imei)
        {
            try
            {
                var body = System.Text.Json.JsonSerializer.Serialize(new
                {
                    embFolio,
                    reciboCap,
                    reciboSug,
                    fechaRecCap,
                    fechaRecSug,
                    prodClave,
                    producto,
                    cantidad,
                    tarimaCap,
                    tarimaSug,
                    responsable,
                    motivo,
                    imei
                });

                var resp = await _http.PostAsync(
                    $"{ATU_URL}/api/otp/request",
                    new StringContent(body, Encoding.UTF8, "application/json"));

                return resp.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Valida el OTP que dictó el supervisor de cámaras frías.
        /// Devuelve (isValid, supervisorId, mensaje).
        /// </summary>
        public async Task<(bool IsValid, string SupervisorId, string Mensaje)> ValidarOTPAsync(
            string otp,
            string embFolio,
            string prodClave,
            string reciboSug,
            string tarimaSug,
            string supervisorId)
        {
            try
            {
                var batchId = $"{prodClave.Trim()}-{reciboSug.Trim()}-{tarimaSug.Trim()}";

                var body = System.Text.Json.JsonSerializer.Serialize(new
                {
                    embFolio,
                    code = otp,
                    supervisorId,
                    actualProdClave = prodClave.Trim(),
                    actualRecibo = reciboSug.Trim(),
                    actualTarima = int.TryParse(tarimaSug.Trim(), out int t) ? t : 0
                });

                var resp = await _http.PostAsync(
                    $"{ATU_URL}/api/otp/validate",
                    new StringContent(body, Encoding.UTF8, "application/json"));

                if (!resp.IsSuccessStatusCode)
                    return (false, "", "Error de conexión con servidor ATU");

                var json = await resp.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                bool isAuth = root.TryGetProperty("isAuthorized", out var a) && a.GetBoolean();
                string msg = root.TryGetProperty("message", out var m) ? m.GetString() ?? "" : "";
                string supId = root.TryGetProperty("supervisorId", out var s) ? s.GetString() ?? "" : supervisorId;

                return (isAuth, supId, msg);
            }
            catch (Exception ex)
            {
                return (false, "", $"Error: {ex.Message}");
            }
        }

        public async Task<bool> ServidorDisponibleAsync()
        {
            try
            {
                var resp = await _http.GetAsync($"{ATU_URL}/health");
                return resp.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}