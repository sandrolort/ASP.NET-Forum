using System.Security.Claims;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Tests.Mocks;

public static class UserManagerMock
{
    public static Mock<UserManager<User>> Instance() =>
        new(
            new Mock<IUserStore<User>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<User>>().Object,
            Array.Empty<IUserValidator<User>>(),
            Array.Empty<IPasswordValidator<User>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<User>>>().Object);
    
    
    public static UserManager<User> GetMockUserManager()
    {
        Mock<UserManager<User>> mockManager;
        
        mockManager = UserManagerMock.Instance();
        
        mockManager.Setup(userManager => userManager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        mockManager.Setup(userManager => userManager.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        
        mockManager.Setup(userManager => userManager.AddClaimAsync(It.IsAny<User>(), It.IsAny<Claim>()))
            .ReturnsAsync(IdentityResult.Success);
        
        mockManager.Setup(userManager => userManager.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new User()
            {
                UserName = "testUser",
                Email = "email@email.com",
            });

        mockManager.Setup(manager => manager.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new User()
            {
                UserName = "testUser",
                Email = "email@email.com",
            });
        
        mockManager.Setup(userManager => userManager.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        mockManager.Setup(userManager => userManager.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string> { "User", "Admin" });
        
        return mockManager.Object;
    }
}