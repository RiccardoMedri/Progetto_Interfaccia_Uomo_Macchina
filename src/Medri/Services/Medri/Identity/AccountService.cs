using Medri.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Identity
{
    public sealed class RegisterClientUserCommand
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public sealed class CheckLoginCredentialsQuery
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public sealed class UserDetailDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string Role { get; set; }
    }

    public sealed class AccountService
    {
        private readonly MedriDbContext dbContext;

        public AccountService(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<UserDetailDTO> RegisterClientAsync(
            RegisterClientUserCommand command,
            CancellationToken cancellationToken = default)
        {
            var email = command.Email?.Trim();

            if (string.IsNullOrWhiteSpace(email))
                throw new LoginException("Inserisci un indirizzo email valido");

            if (string.IsNullOrWhiteSpace(command.Password))
                throw new LoginException("Inserisci una password valida");

            var normalizedEmail = email.ToUpperInvariant();
            var emailAlreadyExists = await dbContext.Users
                .AnyAsync(user =>
                    user.Email != null &&
                    user.Email.ToUpper() == normalizedEmail,
                    cancellationToken);

            if (emailAlreadyExists)
                throw new LoginException("Esiste gia un account con questa email. Accedi con le tue credenziali.");

            var firstName = command.FirstName?.Trim();
            var lastName = command.LastName?.Trim();

            var user = new User
            {
                Email = email,
                Password = AccountPasswordHasher.Hash(command.Password),
                FirstName = firstName,
                LastName = lastName,
                NickName = firstName,
                Role = UserRoles.Client
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(cancellationToken);

            return ToDetail(user);
        }

        public async Task<UserDetailDTO> CheckCredentialsAsync(
            CheckLoginCredentialsQuery query,
            CancellationToken cancellationToken = default)
        {
            var email = query.Email?.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(query.Password))
                throw new LoginException("Email o password errate");

            var normalizedEmail = email.ToUpperInvariant();
            var user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(item =>
                    item.Email != null &&
                    item.Email.ToUpper() == normalizedEmail,
                    cancellationToken);

            if (user == null || !user.IsMatchWithPassword(query.Password))
                throw new LoginException("Email o password errate");

            return ToDetail(user);
        }

        private static UserDetailDTO ToDetail(User user)
        {
            return new UserDetailDTO
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NickName = user.NickName,
                Role = user.Role
            };
        }
    }

    internal static class AccountPasswordHasher
    {
        public static string Hash(string password)
        {
            using var sha256 = SHA256.Create();
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.ASCII.GetBytes(password)));
        }

        public static bool Verify(string password, string expectedHash)
        {
            return !string.IsNullOrWhiteSpace(password) && Hash(password) == expectedHash;
        }
    }
}
