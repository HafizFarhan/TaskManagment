using Injazat.DataAccess.Models;

namespace Injazat.Presentation.Services.LogDBService
{
    public interface ILogDbService
    {
        public void AddLogEvent(LogEvent logEvent);
        public void AddLogException(LogException logException);
    }
}
