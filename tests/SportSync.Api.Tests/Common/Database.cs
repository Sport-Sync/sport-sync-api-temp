using Microsoft.EntityFrameworkCore;
using Moq;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Persistence;

namespace SportSync.Api.Tests.Common;

public class Database
{
    public IDbContext DbContext { get; private set; }
    public IUnitOfWork UnitOfWork { get; private set; }

    private Database() { }

    public Task SaveChangesAsync()
    {
        return UnitOfWork.SaveChangesAsync();
    }

    public static Database Create()
    {
        var database = new Database();
        var options = new DbContextOptionsBuilder<SportSyncDbContext>()
            .UseInMemoryDatabase("sport-sync")
            .Options;

        var dateTimeMock = new Mock<IDateTime>();

        var dbContext = new SportSyncDbContext(options, dateTimeMock.Object);
        database.DbContext = dbContext;
        database.UnitOfWork = dbContext;

        return database;
    }
}