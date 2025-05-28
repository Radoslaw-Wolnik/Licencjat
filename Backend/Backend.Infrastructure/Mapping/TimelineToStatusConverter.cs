using AutoMapper;
using Backend.Domain.Enums;
using System;

namespace Backend.Infrastructure.Mapping;

public class TimelineStatusToSwapStatusConverter
    : ITypeConverter<TimelineStatus, SwapStatus>
{
    public SwapStatus Convert(
        TimelineStatus source,
        SwapStatus destination,
        ResolutionContext context)
    {
        switch (source)
        {
            case TimelineStatus.Requested:
            case TimelineStatus.Accepted:
            case TimelineStatus.Declined:
                return SwapStatus.Requested;

            case TimelineStatus.Canceled:
            case TimelineStatus.MeetingUp:
            case TimelineStatus.ReadingBooks:
            case TimelineStatus.FinishedBooks:
            case TimelineStatus.WaitingForFinish:
            case TimelineStatus.RequestedFinish:
                return SwapStatus.Ongoing;

            case TimelineStatus.Finished:
            case TimelineStatus.Resolved:
                return SwapStatus.Finished;

            case TimelineStatus.Disputed:
                return SwapStatus.Disputed;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(source), source, "Unknown TimelineStatus");
        }
    }
}
