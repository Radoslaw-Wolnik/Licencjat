using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Tests.Infrastructure.Services;

public class SignInServiceTests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly FakeSignInManager _signInManager;
    private readonly SignInService _service;

    public class FakeSignInManager : SignInManager<UserEntity>
    {
        public Func<UserEntity, string, bool, Task<SignInResult>>? CheckPasswordSignInAsyncBehavior { get; set; }

        public FakeSignInManager(
            UserManager<UserEntity> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<UserEntity> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<UserEntity>> logger,
            IAuthenticationSchemeProvider schemes)
            : base(
                userManager,
                contextAccessor,
                claimsFactory,
                optionsAccessor,
                logger,
                schemes)
        {
        }

        public override Task<SignInResult> CheckPasswordSignInAsync(
            UserEntity user,
            string password,
            bool lockoutOnFailure)
        {
            return CheckPasswordSignInAsyncBehavior?.Invoke(user, password, lockoutOnFailure)
                ?? base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
        }
    }


    public SignInServiceTests()
    {
        // Create minimal mocks for required dependencies
        var storeMock = new Mock<IUserStore<UserEntity>>();
        var options = Options.Create(new IdentityOptions());
        var errorDescriber = new IdentityErrorDescriber();
        var logger = Mock.Of<ILogger<UserManager<UserEntity>>>();
        
        _userManagerMock = new Mock<UserManager<UserEntity>>(
            storeMock.Object,
            options,
            null!,  // IPasswordHasher
            new List<IUserValidator<UserEntity>>(),
            new List<IPasswordValidator<UserEntity>>(),
            null!,  // ILookupNormalizer
            errorDescriber,
            null!,  // IServiceProvider
            logger
        );

        // Create FakeSignInManager with minimal dependencies
        _signInManager = new FakeSignInManager(
            userManager: _userManagerMock.Object,
            contextAccessor: Mock.Of<IHttpContextAccessor>(),
            claimsFactory: Mock.Of<IUserClaimsPrincipalFactory<UserEntity>>(),
            optionsAccessor: options,
            logger: Mock.Of<ILogger<SignInManager<UserEntity>>>(),
            schemes: Mock.Of<IAuthenticationSchemeProvider>()
        );

        _service = new SignInService(_signInManager, _userManagerMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var testUser = new UserEntity { Id = Guid.NewGuid(), UserName = "alice", Email = "alice@test.com" };
        
        _userManagerMock.Setup(m => m.FindByNameAsync("alice"))
            .ReturnsAsync(testUser);
            
        _userManagerMock.Setup(m => m.IsLockedOutAsync(testUser))
            .ReturnsAsync(false);
            
        _signInManager.CheckPasswordSignInAsyncBehavior = (user, password, _) => 
            Task.FromResult(password == "validpw" 
                ? SignInResult.Success 
                : SignInResult.Failed);

        // Act
        var result = await _service.LoginAsync("alice", "validpw", false);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(testUser.Id, result.Value);
    }

    [Fact]
    public async Task LoginAsync_AccountLocked_ReturnsFailure()
    {
        // Arrange
        var testUser = new UserEntity { UserName = "lockeduser" };
        
        _userManagerMock.Setup(m => m.FindByNameAsync("lockeduser"))
            .ReturnsAsync(testUser);
            
        _userManagerMock.Setup(m => m.IsLockedOutAsync(testUser))
            .ReturnsAsync(true);  // Account is locked

        // Act
        var result = await _service.LoginAsync("lockeduser", "anypw", false);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Account is temporarily locked", result.Errors[0].Message);
    }
}