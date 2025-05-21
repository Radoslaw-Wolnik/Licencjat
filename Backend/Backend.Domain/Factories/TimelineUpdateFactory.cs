using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Factories
{
    // Factory for creating TimelineUpdate entities for different stages of the swap lifecycle.
    public static class TimelineUpdateFactory
    {

        // Creates the initial "Requested" update when a user initiates a swap.
        public static Result<TimelineUpdate> CreateRequested(Guid userId, Guid swapId)
        {
            var id = Guid.NewGuid();
            var status = TimelineStatus.Requested;
            var description = "Swap requested by user.";
            return TimelineUpdate.Create(id, userId, swapId, status, description);
        }

        // Creates an "Accepted" or "Declined" update when the counterparty responds.
        public static Result<TimelineUpdate> CreateResponse(Guid userId, Guid swapId, bool accepted)
        {
            var id = Guid.NewGuid();
            var status = accepted ? TimelineStatus.Accepted : TimelineStatus.Declined;
            var description = accepted
                ? "Swap request accepted."
                : "Swap request declined.";
            return TimelineUpdate.Create(id, userId, swapId, status, description);
        }

        // Creates a "MeetingUp" update when users agree to meet.
        public static Result<TimelineUpdate> CreateMeetingUp(Guid userId, Guid swapId)
        {
            var id = Guid.NewGuid();
            var status = TimelineStatus.MeetingUp;
            var description = "Users agreed to meet in person.";
            return TimelineUpdate.Create(id, userId, swapId, status, description);
        }

        // Creates an update for reading progress (e.g. page number).
        public static Result<TimelineUpdate> CreateReadingProgress(Guid userId, Guid swapId, int currentPage)
        {
            var id = Guid.NewGuid();
            var status = TimelineStatus.ReadingBooks;
            var description = $"Reading progress: current page {currentPage}.";
            return TimelineUpdate.Create(id, userId, swapId, status, description);
        }

        // Creates a "WaitingForFinish" update when one person finisheed the book.
        public static Result<TimelineUpdate> CreateWaitingForFinish(Guid userId, Guid swapId)
        {
            var id = Guid.NewGuid();
            var status = TimelineStatus.WaitingForFinish;
            var description = "One of you finished reading their book";
            return TimelineUpdate.Create(id, userId, swapId, status, description);
        }

        // Creates a "FinishedBooks" update when reading is done.
        public static Result<TimelineUpdate> CreateFinishedReading(Guid userId, Guid swapId)
        {
            var id = Guid.NewGuid();
            var status = TimelineStatus.FinishedBooks;
            var description = "Finished reading books.";
            return TimelineUpdate.Create(id, userId, swapId, status, description);
        }

        // Creates a "Finished" update when the overall swap is completed.
        public static Result<TimelineUpdate> CreateCompleted(Guid userId, Guid swapId)
        {
            var id = Guid.NewGuid();
            var status = TimelineStatus.Finished;
            var description = "Swap completed successfully. User gave their feedback";
            return TimelineUpdate.Create(id, userId, swapId, status, description);
        }

        // Creates a "Disputed" update when an issue is raised.
        public static Result<TimelineUpdate> CreateDispute(Guid swapId, Guid userId, string issueDetails)
        {
            var id = Guid.NewGuid();
            var status = TimelineStatus.Disputed;
            var description = $"Dispute raised: {issueDetails}";
            return TimelineUpdate.Create(id, userId, swapId, status, description);
        }
        
        // Creates a "Disputed" update when an issue is raised.
        public static Result<TimelineUpdate> CreateResolved(Guid swapId, Guid userId, string resolutionDetails)
        {
            var id = Guid.NewGuid();
            var status = TimelineStatus.Resolved;
            var description = $"Dispute resolved: {resolutionDetails}";
            return TimelineUpdate.Create(id, userId, swapId, status, description);
        }
    }
}
