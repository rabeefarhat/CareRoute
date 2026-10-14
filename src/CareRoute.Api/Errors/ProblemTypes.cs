// src/CareRoute.Api/Errors/ProblemTypes.cs   (Api layer · cross-cutting)
namespace CareRoute.Api.Errors;

public static class ProblemTypes
{
    public const string BaseUri = "https://careroute.example/problems/";               // ①

    public const string PatientNotFound = BaseUri + "patient-not-found";

    public static class Codes                                                            // ②
    {
        public const string PatientNotFound = "patient_not_found";
    }

    public static (string Type, string Code) ForStatus(int status) => status switch      // ③
    {
        400 => (BaseUri + "validation-failed", "validation_failed"),
        404 => (BaseUri + "not-found", "not_found"),
        405 => (BaseUri + "method-not-allowed", "method_not_allowed"),
        409 => (BaseUri + "conflict", "conflict"),
        412 => (BaseUri + "precondition-failed", "precondition_failed"),
        415 => (BaseUri + "unsupported-media-type", "unsupported_media_type"),
        >= 500 => (BaseUri + "internal-error", "internal_error"),
        _ => (BaseUri + "http-" + status, "http_" + status)
    };
}