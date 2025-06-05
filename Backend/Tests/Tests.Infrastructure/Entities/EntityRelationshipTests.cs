using Backend.Domain.Enums;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.ComponentModel.DataAnnotations;
using Tests.Infrastructure;
using Xunit;

namespace Tests.Infrastructure.Entities;
/*
// the is not possible - go through the root 
public class EntityRelationshipTests : TestContainersBase, IAsyncLifetime
{
    private ApplicationDbContext _context = null!;

    protected override async Task OnTestInitializedAsync()
    {
        _context = CreateDbContext();
        await ResetDatabase();
    }

    public new async Task DisposeAsync() => await base.DisposeAsync();

    // Helper method to create valid user
    private UserEntity CreateValidUser() => new UserEntity
    {
        UserName = "test",
        Email = "test@example.com",
        FirstName = "Barry", LastName = "Alan", City = "London", Country = "GB", Reputation = 4.5f
    };

    [Fact]
    public async Task Bookmark_Requires_UserBook()
    {
        // Arrange
        var bookmark = new BookmarkEntity 
        {
            Colour = BookmarkColours.red,
            Page = 42,
            // Missing required UserBook
        };

        // Act & Assert
        _context.Bookmarks.Add(bookmark);
        await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync());
    }

    [Fact]
    public async Task Feedback_Stars_RangeConstraint_Enforced()
    {
        // Arrange
        var user = CreateValidUser();
        var subSwap = new SubSwapEntity { User = user };
        
        var feedback = new FeedbackEntity 
        {
            Stars = 6, // Invalid
            Recommend = true,
            User = user,
            SubSwap = subSwap
        };

        // Act & Assert
        _context.Feedbacks.Add(feedback);
        await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync());
    }

    [Fact]
    public async Task Swap_Requires_TwoSubSwaps()
    {
        // Arrange
        var swap = new SwapEntity();
        swap.SubSwaps.Add(new SubSwapEntity { User = CreateValidUser() }); // Only one SubSwap

        // Act & Assert
        _context.Swaps.Add(swap);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync());
        exception.InnerException.Should().BeOfType<PostgresException>()
            .Which.ConstraintName.Should().Be("FK_Swaps_SubSwaps_SubSwapAcceptingId");
    }

    [Fact]
    public async Task DeleteUser_Cascades_ToDependents()
    {
        // Arrange
        var user = CreateValidUser();
        user.SocialMediaLinks.Add(new SocialMediaLinkEntity { Platform = SocialMediaPlatform.Messenger, Url = "fb.com" });
        user.Wishlist.Add(new UserWishlistEntity { GeneralBook = new GeneralBookEntity() });
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        // Assert
        (await _context.SocialMediaLinks.CountAsync()).Should().Be(0);
        (await _context.UserWishlists.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task SubSwap_CanExist_Without_OptionalRelations()
    {
        // Arrange
        var subSwap = new SubSwapEntity 
        {
            User = CreateValidUser(),
            Swap = new SwapEntity
            {
                SubSwapRequesting = new SubSwapEntity { User = CreateValidUser() },
                SubSwapAccepting = new SubSwapEntity { User = CreateValidUser() }
            }
        };

        // Act
        _context.SubSwaps.Add(subSwap);
        var exception = await Record.ExceptionAsync(() => _context.SaveChangesAsync());

        // Assert
        exception.Should().BeNull();
        (await _context.SubSwaps.FindAsync(subSwap.Id)).Should().NotBeNull();
    }

    [Fact]
    public async Task Meetup_Status_DefaultValue_Correct()
    {
        // Arrange
        var meetup = new MeetupEntity
        {
            Location_X = 10.5,
            Location_Y = 20.3f,
            User = CreateValidUser(),
            Swap = new SwapEntity
            {
                SubSwapRequesting = new SubSwapEntity { User = CreateValidUser() },
                SubSwapAccepting = new SubSwapEntity { User = CreateValidUser() }
            }
        };

        // Act
        _context.Meetups.Add(meetup);
        await _context.SaveChangesAsync();

        // Assert
        var loaded = await _context.Meetups.FirstAsync();
        loaded.Status.Should().Be(MeetupStatus.Proposed);
    }

    [Fact]
    public async Task Timeline_CreatedAt_AutomaticallySet()
    {
        // Arrange
        var timeline = new TimelineEntity
        {
            Description = "Test event",
            Status = TimelineStatus.Created,
            User = CreateValidUser(),
            Swap = new SwapEntity
            {
                SubSwapRequesting = new SubSwapEntity { User = CreateValidUser() },
                SubSwapAccepting = new SubSwapEntity { User = CreateValidUser() }
            }
        };

        // Act
        _context.Timelines.Add(timeline);
        await _context.SaveChangesAsync();

        // Assert
        var loaded = await _context.Timelines.FirstAsync();
        loaded.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task UserBook_Bookmarks_CollectionWorks()
    {
        // Arrange
        var userBook = new UserBookEntity();
        userBook.Bookmarks.AddRange(new[]
        {
            new BookmarkEntity { Colour = BookmarkColours.blue, Page = 10 },
            new BookmarkEntity { Colour = BookmarkColours.green, Page = 20 }
        });

        // Act
        _context.UserBooks.Add(userBook);
        await _context.SaveChangesAsync();

        // Assert
        var loaded = await _context.UserBooks
            .Include(ub => ub.Bookmarks)
            .FirstAsync();
        
        loaded.Bookmarks.Should().HaveCount(2);
        loaded.Bookmarks.Should().Contain(b => b.Page == 10);
    }
}

*/