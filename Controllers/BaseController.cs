using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Injazat.Presentation.Services.ErrorService;
using Microsoft.AspNetCore.Mvc.Filters;
namespace Injazat.Presentation.Controllers
{
    public class BaseController : IAsyncExceptionFilter
    {
        private readonly IErrorLogManager _errorLogManager;
        private readonly ILogger<BaseController> _logger;
        public BaseController(IErrorLogManager errorLogManager, ILogger<BaseController> logger)
        {
            //_errorLogManager = errorLogManager;
            _errorLogManager = errorLogManager ?? throw new ArgumentNullException(nameof(errorLogManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Unhandled exception occurred.");


            // Log exception to database
            await _errorLogManager.LogExceptionToDatabaseAsync(context.Exception, null);

            context.ExceptionHandled = true;
            context.Result = new StatusCodeResult(500);
        }
    }
}
