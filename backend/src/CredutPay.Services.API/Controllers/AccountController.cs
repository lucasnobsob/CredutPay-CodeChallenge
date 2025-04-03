using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CredutPay.Domain.Core.Interfaces;
using CredutPay.Domain.Core.Notifications;
using CredutPay.Domain.Interfaces;
using CredutPay.Domain.Models;
using CredutPay.Infra.CrossCutting.Identity.Models;
using CredutPay.Infra.CrossCutting.Identity.Models.AccountViewModels;
using CredutPay.Infra.CrossCutting.Identity.Services;
using CredutPay.Infra.Data.Context;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CredutPay.Services.Api.Controllers
{
    [Authorize]
    public class AccountController : ApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUser _user;
        private readonly IJwtFactory _jwtFactory;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext,
            IUser user,
            IJwtFactory jwtFactory,
            ILoggerFactory loggerFactory,
            INotificationHandler<DomainNotification> notifications,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _user = user;
            _jwtFactory = jwtFactory;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response();
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Usuário ou senha inválidos.");
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);
            if (!signInResult.Succeeded)
            {
                NotifyError(signInResult.ToString(), "Usuário ou senha inválidos.");
                return Response();
            }

            var appUser = await _userManager.FindByEmailAsync(model.Email);

            _logger.LogInformation(1, "Usuário logado com sucesso!");
            return Response(await GenerateToken(appUser));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response();
            }

            var appUser = new ApplicationUser { UserName = model.UserName, Email = model.Email };
            var identityResult = await _userManager.CreateAsync(appUser, model.Password);
            if (!identityResult.Succeeded)
            {
                AddIdentityErrors(identityResult);
                return Response();
            }

            identityResult = await _userManager.AddToRoleAsync(appUser, "Admin");
            if (!identityResult.Succeeded)
            {
                AddIdentityErrors(identityResult);
                return Response();
            }

            var userClaims = new List<Claim>
            {
                new Claim("Wallet_Write", "Write"),
                new Claim("Wallet_Remove", "Remove"),
                new Claim("Transaction_Write", "Write")
            };
            await _userManager.AddClaimsAsync(appUser, userClaims);

            _logger.LogInformation(3, "Novo usuário registrado!");
            return Response();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("refresh")]
        public async Task<IActionResult> Refresh(TokenViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response();
            }

            // Get current RefreshToken
            var refreshTokenCurrent = _dbContext.RefreshTokens.SingleOrDefault
                (x => x.Token == model.RefreshToken && !x.Used && !x.Invalidated);
            if (refreshTokenCurrent is null)
            {
                NotifyError("RefreshToken", "O refresh token não existe.");
                return Response();
            }
            if (refreshTokenCurrent.ExpiryDate < DateTime.UtcNow)
            {
                // Update current RefreshToken
                refreshTokenCurrent.Invalidated = true;
                await _dbContext.SaveChangesAsync();
                NotifyError("RefreshToken", "Refresh token invalido.");
                return Response();
            }

            // Get User
            var appUser = await _userManager.FindByIdAsync(refreshTokenCurrent.UserId);
            if (appUser is null)
            {
                NotifyError("User", "Usuário não existe.");
                return Response();
            }

            refreshTokenCurrent.Used = true;
            await _dbContext.SaveChangesAsync();

            return Response(await GenerateToken(appUser));
        }

        [HttpGet]
        [Route("current")]
        public IActionResult GetCurrent()
        {
            return Response(new
            {
                IsAuthenticated = _user.IsAuthenticated(),
                ClaimsIdentity = _user.GetClaimsIdentity().Select(x => new { x.Type, x.Value }),
            });
        }

        private async Task<TokenViewModel> GenerateToken(ApplicationUser appUser)
        {
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, appUser.Email));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()));

            var userClaims = await _userManager.GetClaimsAsync(appUser);
            claimsIdentity.AddClaims(userClaims);

            var userRoles = await _userManager.GetRolesAsync(appUser);
            claimsIdentity.AddClaims(userRoles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                claimsIdentity.AddClaims(roleClaims);
            }

            var jwtToken = await _jwtFactory.GenerateJwtToken(claimsIdentity);
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString("N"),
                UserId = appUser.Id.ToString(),
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMinutes(90),
                JwtId = jwtToken.JwtId
            };
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new TokenViewModel
            {
                AccessToken = jwtToken.AccessToken,
                RefreshToken = refreshToken.Token,
            };
        }
    }
}
