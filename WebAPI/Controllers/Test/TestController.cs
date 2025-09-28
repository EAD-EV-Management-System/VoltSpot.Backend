using Infrastructure.Data.Context;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebAPI.Controllers.Base;

namespace WebAPI.Controllers.Test
{
    public class TestController : BaseController
    {
        private readonly AppDbContext _context;

        public TestController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("health")]
        public async Task<IActionResult> HealthCheck()
        {
            try
            {
                // Test database connection
                var userCount = await _context.Users.CountDocumentsAsync(FilterDefinition<Domain.Entities.User>.Empty);
                var evOwnerCount = await _context.EVOwners.CountDocumentsAsync(FilterDefinition<Domain.Entities.EVOwner>.Empty);

                return Success(new
                {
                    Status = "Healthy",
                    DatabaseConnected = true,
                    Collections = new
                    {
                        Users = userCount,
                        EVOwners = evOwnerCount
                    },
                    Timestamp = DateTime.UtcNow
                }, "System is healthy and database is connected");
            }
            catch (Exception ex)
            {
                return Error($"Database connection failed: {ex.Message}", 500);
            }
        }

        [HttpGet("collections")]
        public async Task<IActionResult> GetCollectionsInfo()
        {
            try
            {
                var collections = new
                {
                    Users = new
                    {
                        Count = await _context.Users.CountDocumentsAsync(FilterDefinition<Domain.Entities.User>.Empty),
                        Collection = "Users"
                    },
                    EVOwners = new
                    {
                        Count = await _context.EVOwners.CountDocumentsAsync(FilterDefinition<Domain.Entities.EVOwner>.Empty),
                        Collection = "EVOwners"
                    }
                };

                return Success(collections, "Collections information retrieved successfully");
            }
            catch (Exception ex)
            {
                return Error($"Failed to retrieve collections info: {ex.Message}", 500);
            }
        }
    }
}
