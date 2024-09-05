using Injazat.DataAccess.Models;

namespace Injazat.Presentation.Services.BidService
{
    public interface IBidService
    {
        Task<Bid> CreateBid(Bid bid);
        Task<Bid> ApproveBid(Bid bid, int userId);
        Task<Bid> SubmitBid(Bid bid);
        Task<IEnumerable<Bid>> GetBidsByTaskId(int taskId);
    }
}
