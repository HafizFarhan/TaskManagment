using EasyRepository.EFCore.Generic;
using Injazat.DataAccess.Models;
using Injazat.Presentation.Services.BidService;
using Injazat.Presentation.Services.LogDBService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task = Injazat.DataAccess.Models.Task;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class BidService : IBidService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<BidService> _logger;
    private readonly ILogDbService _logDbService;

    public BidService(
        IUnitOfWork unitOfWork,
        UserManager<User> userManager,
        ILogger<BidService> logger,
        ILogDbService logDbService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _logger = logger;
        _logDbService = logDbService;
    }

    public async Task<Bid> CreateBid(Bid bid)
    {
        if (bid == null) throw new ArgumentNullException(nameof(bid));

        bid.CreationDate = DateTime.UtcNow;
        bid.ModificationDate = DateTime.UtcNow;

        _unitOfWork.Repository.Add(bid);
        await _unitOfWork.Repository.CompleteAsync();

        var logEvent = new LogEvent
        {
            Action = "Bid Created",
            Description = $"Bid {bid.Id} created for task {bid.TaskId}",
            UserId = bid.SupplierId
        };
        _logDbService.AddLogEvent(logEvent);

        _logger.LogInformation($"Bid {bid.Id} created for task {bid.TaskId}");
        return bid;
    }

    public async Task<Bid> ApproveBid(Bid bid, int userId)
    {
        if (bid == null) throw new ArgumentNullException(nameof(bid));

        var task = await _unitOfWork.Repository.GetQueryable<Task>(t => t.Id == bid.TaskId).FirstOrDefaultAsync();
        if (task == null) throw new ArgumentException("Task not found");

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new ArgumentException("User not found");

        // Role check for user
        if (!await _userManager.IsInRoleAsync(user, "Vendor") || bid.Task.VendorId != user.Id)
        {
            throw new UnauthorizedAccessException("User not authorized to approve bids");
        }

        bid.IsApproved = true;
        bid.ModificationDate = DateTime.UtcNow;

        _unitOfWork.Repository.Update(bid);
        await _unitOfWork.Repository.CompleteAsync();

        var logEvent = new LogEvent
        {
            Action = "Bid Approved",
            Description = $"Bid {bid.Id} approved by user {userId} for task {bid.TaskId}",
            UserId = userId
        };
        _logDbService.AddLogEvent(logEvent);

        _logger.LogInformation($"Bid {bid.Id} approved by user {userId} for task {bid.TaskId}");
        return bid;
    }

    public async Task<Bid> SubmitBid(Bid bid)
    {
        if (bid == null) throw new ArgumentNullException(nameof(bid));

        var task = await _unitOfWork.Repository.GetQueryable<Task>(t => t.Id == bid.TaskId).FirstOrDefaultAsync();
        if (task == null) throw new ArgumentException("Task not found");

        if (!task.OpenForBid) throw new InvalidOperationException("Task is not open for bidding");

        bid.CreationDate = DateTime.UtcNow;
        bid.ModificationDate = DateTime.UtcNow;

        _unitOfWork.Repository.Add(bid);
        await _unitOfWork.Repository.CompleteAsync();

        var logEvent = new LogEvent
        {
            Action = "Bid Submitted",
            Description = $"Bid {bid.Id} submitted for task {task.Id} by supplier {bid.SupplierId}",
            UserId = bid.SupplierId
        };
        _logDbService.AddLogEvent(logEvent);

        _logger.LogInformation($"Bid {bid.Id} submitted for task {task.Id} by supplier {bid.SupplierId}");
        return bid;
    }

    public async Task<IEnumerable<Bid>> GetBidsByTaskId(int taskId)
    {
        return await _unitOfWork.Repository.GetQueryable<Bid>(b => b.TaskId == taskId).ToListAsync();
    }

    public
        async Task<Bid> DeleteBid(int bidId)
    {
        var bid = await _unitOfWork.Repository.GetQueryable<Bid>(b => b.Id == bidId).FirstOrDefaultAsync();
        if (bid == null) throw new ArgumentException("Bid not found");

        bid.ModificationDate = DateTime.UtcNow;
        bid.IsDeleted = true;
        bid.DeletionDate = DateTime.UtcNow;

        _unitOfWork.Repository.HardDelete(bid);
        await _unitOfWork.Repository.CompleteAsync();

        var logEvent = new LogEvent
        {
            Action = "Bid Deleted",
            Description = $"Bid {bid.Id} deleted",
            UserId = bid.SupplierId
        };
        _logDbService.AddLogEvent(logEvent);

        _logger.LogInformation($"Bid {bid.Id} deleted");
        return bid;
    }
}
