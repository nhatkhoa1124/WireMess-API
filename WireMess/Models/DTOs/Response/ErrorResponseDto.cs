namespace WireMess.Models.DTOs.Response
{
    public class ErrorResponseDto
    {
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string Code { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string RequestId { get; set; }
        public string Path { get; set; }

        public static ErrorResponseDto ValidationError(List<string> errors)
        {
            return new ErrorResponseDto
            {
                Message = "Validation failed",
                Errors = errors,
                Code = "VALIDATION_ERROR"
            };
        }

        public static ErrorResponseDto NotFound(string resource)
        {
            return new ErrorResponseDto
            {
                Message = $"{resource} not found",
                Code = "NOT_FOUND"
            };
        }

        public static ErrorResponseDto Unauthorized(string message = "Authentication required")
        {
            return new ErrorResponseDto 
            { 
                Message = message,
                Code = "UNAUTHORIZED"
            };
        }

        public static ErrorResponseDto Forbidden(string message = "Insufficient permissions")
        {
            return new ErrorResponseDto
            {
                Message = message,
                Code = "FORBIDDEN"
            };
        }

        public static ErrorResponseDto InternalError(string message = "An internal error occurred")
        {
            return new ErrorResponseDto
            {
                Message = message,
                Code = "INTERNAL_SERVER_ERROR"
            };
        }
    }
}
