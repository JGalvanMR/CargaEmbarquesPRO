using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CargaEmbarques.Models
{
    // Respuesta genérica de casi todos los endpoints
    public class AtuApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    // Respuesta de /api/otp/request (ya la manejas, pero la dejamos explícita)
    public class AtuRequestResponse : AtuApiResponse
    {
    }

    // Respuesta de /api/otp/generate-folio
    public class AtuGenerateFolioResponse : AtuApiResponse
    {
        public AtuGenerateFolioData? Data { get; set; }
    }

    public class AtuGenerateFolioData
    {
        public string Code { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int SecondsRemaining { get; set; }
        public string BatchId { get; set; } = string.Empty;
        public Guid TransactionId { get; set; }
    }

    // Respuesta de /api/otp/validate-folio (la más compleja)
    public class AtuValidateFolioResponse
    {
        public bool Success { get; set; }
        public string Status { get; set; } = string.Empty; // Green, Yellow, Red
        public string Message { get; set; } = string.Empty;
        public bool IsAuthorized { get; set; }
        public string? SupervisorId { get; set; }
    }
}