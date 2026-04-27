using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.API.Extensions;
using CareSchedule.DTOs;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("outcome")]
    [Authorize(Roles = "Provider,Admin,Patient")]
    public class OutcomesController(
        IOutcomeService _outcomeservice,
        IAppointmentRepository _appointmentRepo,
        IWebHostEnvironment _hostEnvironment) : ControllerBase
    {
        private static readonly string[] AllowedExtensions = [".pdf", ".png", ".jpg", ".jpeg", ".webp"];

        private string PrescriptionFolder
            => Path.Combine(_hostEnvironment.ContentRootPath, "uploads", "prescriptions");

        private static string SafeFileName(string fileName)
        {
            var cleaned = Path.GetFileName(fileName ?? string.Empty).Trim();
            return Regex.Replace(cleaned, @"[^a-zA-Z0-9._-]", "_");
        }

        private string? FindPrescriptionPath(int appointmentId)
        {
            var prefix = $"appointment-{appointmentId}-";
            if (!Directory.Exists(PrescriptionFolder)) return null;
            return Directory
                .EnumerateFiles(PrescriptionFolder)
                .FirstOrDefault(path => Path.GetFileName(path).StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        [HttpGet("{appointmentId:int}")]
        public ActionResult<ApiResponse<OutcomeResponseDto?>> GetByAppointment(int appointmentId)
        {
            var appointment = _appointmentRepo.GetById(appointmentId);
            if (appointment == null)
                return NotFound(ApiResponse<OutcomeResponseDto?>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Appointment not found."));

            var role = User.GetRole();
            var userId = User.GetUserId();
            if (string.Equals(role, "Patient", StringComparison.OrdinalIgnoreCase) && appointment.PatientId != userId)
                return Forbid();
            if (string.Equals(role, "Provider", StringComparison.OrdinalIgnoreCase) && appointment.ProviderId != userId)
                return Forbid();

            var result = _outcomeservice.GetOutcomeByAppointment(appointmentId);
            if (result != null)
            {
                var prescriptionPath = FindPrescriptionPath(appointmentId);
                if (!string.IsNullOrWhiteSpace(prescriptionPath))
                {
                    result.HasPrescription = true;
                    result.PrescriptionFileName = Path.GetFileName(prescriptionPath);
                }
            }
            return ApiResponse<OutcomeResponseDto?>.Ok(result, result == null ? "Outcome not available yet." : "Outcome fetched.");
        }

        [HttpPost("{appointmentId:int}")]
        [Authorize(Roles = "Provider,Admin")]
        public ActionResult<ApiResponse<OutcomeResponseDto>> RecordOutcome(int appointmentId, [FromBody] RecordOutcomeRequestDto dto)
        {
            var result = _outcomeservice.RecordOutcome(appointmentId, dto);
            return ApiResponse<OutcomeResponseDto>.Ok(result, "Outcome recorded.");
        }

        [HttpPost("{appointmentId:int}/prescription")]
        [Authorize(Roles = "Provider,Admin")]
        [RequestSizeLimit(10_000_000)]
        public async Task<ActionResult<ApiResponse<object>>> UploadPrescription(int appointmentId, [FromForm] IFormFile file)
        {
            if (appointmentId <= 0) return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Invalid appointmentId."));
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Prescription file is required."));

            var outcome = _outcomeservice.GetOutcomeByAppointment(appointmentId);
            if (outcome == null)
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Record outcome first, then upload prescription."));

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Allowed file types: pdf, png, jpg, jpeg, webp."));

            Directory.CreateDirectory(PrescriptionFolder);
            var oldPath = FindPrescriptionPath(appointmentId);
            if (!string.IsNullOrWhiteSpace(oldPath) && System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);

            var safeOriginal = SafeFileName(file.FileName);
            var finalName = $"appointment-{appointmentId}-{DateTime.UtcNow:yyyyMMddHHmmss}-{safeOriginal}";
            var savePath = Path.Combine(PrescriptionFolder, finalName);

            await using (var stream = System.IO.File.Create(savePath))
            {
                await file.CopyToAsync(stream);
            }

            return ApiResponse<object>.Ok(
                new { fileName = finalName, size = file.Length },
                "Prescription uploaded.");
        }

        [HttpGet("{appointmentId:int}/prescription")]
        public ActionResult DownloadPrescription(int appointmentId)
        {
            var appointment = _appointmentRepo.GetById(appointmentId);
            if (appointment == null)
                return NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Appointment not found."));

            var role = User.GetRole();
            var userId = User.GetUserId();
            if (string.Equals(role, "Patient", StringComparison.OrdinalIgnoreCase) && appointment.PatientId != userId)
                return Forbid();
            if (string.Equals(role, "Provider", StringComparison.OrdinalIgnoreCase) && appointment.ProviderId != userId)
                return Forbid();

            var path = FindPrescriptionPath(appointmentId);
            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
                return NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Prescription not found."));

            var ext = Path.GetExtension(path).ToLowerInvariant();
            var contentType = ext switch
            {
                ".pdf" => "application/pdf",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
            var bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, contentType, Path.GetFileName(path));
        }
    }
}
