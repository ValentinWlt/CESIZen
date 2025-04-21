using CESIZen.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;

namespace CESIZen.Tests.Helpers
{
    public static class TestHelpers
    {
        public static Mock<UserManager<Utilisateur>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<Utilisateur>>();
            var mgr = new Mock<UserManager<Utilisateur>>(
                store.Object,
                null, null, null, null, null, null, null, null
            );

            return mgr;
        }

        public static Mock<SignInManager<Utilisateur>> GetMockSignInManager()
        {
            var userManager = GetMockUserManager();

            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<Utilisateur>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var logger = new Mock<ILogger<SignInManager<Utilisateur>>>();
            var schemes = new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
            var userConfirmation = new Mock<IUserConfirmation<Utilisateur>>();

            var signInManager = new Mock<SignInManager<Utilisateur>>(
                userManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                options.Object,
                logger.Object,
                schemes.Object,
                userConfirmation.Object
            );

            return signInManager;
        }

        public static Mock<RoleManager<IdentityRole<int>>> GetMockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole<int>>>();
            var roleMgr = new Mock<RoleManager<IdentityRole<int>>>(
                store.Object,
                new List<IRoleValidator<IdentityRole<int>>>(),
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<IdentityRole<int>>>>().Object
            );

            return roleMgr;
        }
    }
}
