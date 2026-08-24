namespace UserManagementApi.Models
{
    public record ErrorDetails(int StatusCode, string Message, string? Detail = null);
}