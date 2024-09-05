using EasyRepository.EFCore.Generic;
using Injazat.DataAccess.Models;
using Injazat.Presentation.Services.LogDBService;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection.Metadata;
namespace Injazat.Presentation.Services.ErrorService
{
    public class ErrorLogManager : IErrorLogManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ErrorLogManager> _logger;
        public ErrorLogManager(IUnitOfWork unitOfWork, ILogger<ErrorLogManager> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<string> LogExceptionToDatabaseAsync(Exception exception, int? userId )
        {
            var errorLog = new LogException
            {
                ExceptionMessage = exception.Message,
                StackTrace = exception.StackTrace,
                UserId = userId,
                CreationDate= DateTime.Now,
            };
            _unitOfWork.Repository.Add(errorLog);
            await _unitOfWork.Repository.CompleteAsync();

            LogExceptionToFile(exception.Message, exception.StackTrace);

            // Return the error ID
            return errorLog.Id.ToString();
        }

        public void LogExceptionToFile(string message, string source)
        {
            try
            {
                var logMessage = $"Message: {message}\nSource: {source}\nTime: {DateTime.UtcNow}\n";

                var logPath = Path.GetDirectoryName(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "ErrorLog.txt"));
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "ErrorLog.txt"), logMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logging exception to file.");
            }
        }
    }
}

