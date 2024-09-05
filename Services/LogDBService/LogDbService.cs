using EasyRepository.EFCore.Generic;
using Injazat.DataAccess.Models;

namespace Injazat.Presentation.Services.LogDBService
{
    public class LogDbService : ILogDbService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LogDbService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async void AddLogEvent(DataAccess.Models.LogEvent logEvent)
        {
            logEvent.CreationDate = DateTime.UtcNow;

            _unitOfWork.Repository.Add(logEvent);
            await _unitOfWork.Repository.CompleteAsync();
        }

        public async void AddLogException(LogException logException)
        {
            _unitOfWork.Repository.Add(logException);
            logException.CreationDate = DateTime.Now;
            await _unitOfWork.Repository.CompleteAsync();
        }
    }
}
