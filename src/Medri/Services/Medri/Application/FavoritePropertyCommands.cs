using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public class FavoritePropertyCommandResult
    {
        public Guid PropertyId { get; set; }
        public bool IsSaved { get; set; }
        public bool Succeeded { get; set; }
    }

    public class AddFavoritePropertyCommand
    {
        private readonly MedriDbContext dbContext;

        public AddFavoritePropertyCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<FavoritePropertyCommandResult> ExecuteAsync(
            Guid userId,
            Guid propertyListingId,
            CancellationToken cancellationToken = default)
        {
            var propertyExists = await dbContext.PropertyListings
                .AsNoTracking()
                .AnyAsync(property => property.Id == propertyListingId, cancellationToken);
            if (!propertyExists)
            {
                return new FavoritePropertyCommandResult
                {
                    PropertyId = propertyListingId,
                    IsSaved = false,
                    Succeeded = false
                };
            }

            var favoriteExists = await dbContext.FavoriteProperties
                .AsNoTracking()
                .AnyAsync(
                    favorite => favorite.UserId == userId &&
                        favorite.PropertyListingId == propertyListingId,
                    cancellationToken);
            if (favoriteExists)
            {
                return new FavoritePropertyCommandResult
                {
                    PropertyId = propertyListingId,
                    IsSaved = true,
                    Succeeded = true
                };
            }

            dbContext.FavoriteProperties.Add(new FavoriteProperty
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PropertyListingId = propertyListingId,
                CreatedAtUtc = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync(cancellationToken);
            return new FavoritePropertyCommandResult
            {
                PropertyId = propertyListingId,
                IsSaved = true,
                Succeeded = true
            };
        }
    }

    public class RemoveFavoritePropertyCommand
    {
        private readonly MedriDbContext dbContext;

        public RemoveFavoritePropertyCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<FavoritePropertyCommandResult> ExecuteAsync(
            Guid userId,
            Guid propertyListingId,
            CancellationToken cancellationToken = default)
        {
            var favorite = await dbContext.FavoriteProperties
                .FirstOrDefaultAsync(
                    item => item.UserId == userId &&
                        item.PropertyListingId == propertyListingId,
                    cancellationToken);
            if (favorite == null)
            {
                return new FavoritePropertyCommandResult
                {
                    PropertyId = propertyListingId,
                    IsSaved = false,
                    Succeeded = true
                };
            }

            dbContext.FavoriteProperties.Remove(favorite);
            await dbContext.SaveChangesAsync(cancellationToken);
            return new FavoritePropertyCommandResult
            {
                PropertyId = propertyListingId,
                IsSaved = false,
                Succeeded = true
            };
        }
    }
}
