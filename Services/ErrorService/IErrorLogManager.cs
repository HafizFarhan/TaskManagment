namespace Injazat.Presentation.Services.ErrorService
{
    public interface IErrorLogManager
    {
        Task<string> LogExceptionToDatabaseAsync(Exception exception, int? userId = null);
    }
}
