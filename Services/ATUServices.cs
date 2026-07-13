using CargaEmbarques.Models;
using Newtonsoft.Json; // O System.Text.Json, a tu elección. 
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CargaEmbarques.Services
{
    public class ATUServices
    {
        private readonly HttpClient _http;
        private const string ATU_URL = "http://192.168.123.244:83/auth";
        //private const string ATU_URL = "http://atu-web.int.mrlucky.com/auth";
        //private const string ATU_URL = "http://192.168.123.155:5002"; // Ajusta según tu entorno
        //private const string ATU_URL = "http://192.168.123.155:5049"; // Ajusta según tu entorno

        public ATUServices()
        {
            _http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        }

        /// <summary>
        /// 1. Operador de CargaEmbarques → levanta la solicitud de folio adelantado.
        /// POST /api/otp/request
        /// </summary>
        public async Task<bool> CrearSolicitudAsync(
            string embFolio,
            string reciboCap,      // FolioLeido
            string reciboSug,      // FolioAtrasado
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
                var body = new
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
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(body),
                    Encoding.UTF8,
                    "application/json");

                var resp = await _http.PostAsync($"{ATU_URL}/api/otp/request", content);
                return resp.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 2. Supervisor (ATU.CamaraFria) → genera OTP para un folio adelantado.
        /// POST /api/otp/generate-folio
        /// </summary>
        /// <returns>Resultado con el código OTP generado y metadatos.</returns>
        public async Task<AtuGenerateFolioResponse> GenerarOTPFolioAsync(
            string supervisorId,
            string embFolio,
            string deviceFingerprint)
        {
            try
            {
                var body = new
                {
                    supervisorId = supervisorId.Trim(),
                    embFolio = embFolio.Trim(),
                    deviceFingerprint = deviceFingerprint ?? string.Empty
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(body),
                    Encoding.UTF8,
                    "application/json");

                var httpResponse = await _http.PostAsync($"{ATU_URL}/api/otp/generate-folio", content);
                var json = await httpResponse.Content.ReadAsStringAsync();

                if (!httpResponse.IsSuccessStatusCode)
                {
                    return new AtuGenerateFolioResponse
                    {
                        Success = false,
                        Message = "Error de conexión con servidor ATU"
                    };
                }

                return JsonConvert.DeserializeObject<AtuGenerateFolioResponse>(json)
                       ?? new AtuGenerateFolioResponse
                       {
                           Success = false,
                           Message = "Respuesta inesperada del servidor"
                       };
            }
            catch (TaskCanceledException)
            {
                return new AtuGenerateFolioResponse
                {
                    Success = false,
                    Message = "Tiempo de espera agotado al contactar ATU"
                };
            }
            catch (Exception ex)
            {
                return new AtuGenerateFolioResponse
                {
                    Success = false,
                    Message = $"Error inesperado: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 3. Operador de CargaEmbarques → valida el OTP dictado por el supervisor.
        /// POST /api/otp/validate-folio
        /// </summary>
        /// <returns>Resultado detallado con status, mensaje y supervisorId.</returns>
        public async Task<AtuValidateFolioResponse> ValidarOTPAsync(
            string embFolio,
            string otp,
            string supervisorId,
            string prodClave,
            string reciboCap,
            string tarimaCap,
            string claimedProdClave,   // la del adelanto
            string claimedReciboSug,   // la del adelanto
            string claimedTarimaSug)
        {
            try
            {
                var body = new
                {
                    embFolio = embFolio.Trim(),
                    code = otp,
                    SupervisorId = supervisorId.Trim(),
                    actualProdClave = prodClave.Trim(),
                    actualRecibo = reciboCap.Trim(),
                    actualTarima = int.TryParse(tarimaCap.Trim(), out int t) ? t : 0,
                    claimedProdClave = claimedProdClave.Trim(),
                    claimedReciboSug = claimedReciboSug.Trim(),
                    claimedTarimaSug = int.TryParse(claimedTarimaSug.Trim(), out int ts) ? ts : 0
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(body),
                    Encoding.UTF8,
                    "application/json");

                var httpResponse = await _http.PostAsync($"{ATU_URL}/api/otp/validate-folio", content);
                var json = await httpResponse.Content.ReadAsStringAsync();

                if (!httpResponse.IsSuccessStatusCode)
                {
                    return new AtuValidateFolioResponse
                    {
                        Success = false,
                        Status = "Red",
                        Message = "Servidor ATU no disponible",
                        IsAuthorized = false
                    };
                }

                var result = JsonConvert.DeserializeObject<AtuValidateFolioResponse>(json);
                return result ?? new AtuValidateFolioResponse
                {
                    Success = false,
                    Status = "Red",
                    Message = "Respuesta inesperada del servidor",
                    IsAuthorized = false
                };
            }
            catch (TaskCanceledException)
            {
                return new AtuValidateFolioResponse
                {
                    Success = false,
                    Status = "Red",
                    Message = "Tiempo de espera agotado al contactar ATU",
                    IsAuthorized = false
                };
            }
            catch (Exception ex)
            {
                return new AtuValidateFolioResponse
                {
                    Success = false,
                    Status = "Red",
                    Message = $"Error inesperado: {ex.Message}",
                    IsAuthorized = false
                };
            }
        }

        /// <summary>
        /// Verifica si el servidor ATU está en línea.
        /// </summary>
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