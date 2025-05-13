using API.Utilities;
using Database.Interceptors;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace Database.Tests;

/// <summary>
/// Shared
/// <remarks>
/// Mostly following the design pattern shown here
/// https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database
/// </remarks>
public class DatabaseFixture
{
    private const string ConnectionString =
        @"Server=localhost;Port=5432;User Id=postgres;Password=password;Include Error Detail=true;";

    /// <summary>
    /// Creates an instance of <see cref="OtrContext"/> where commands are wrapped in a transaction.
    /// Auditing is enabled automatically.
    /// </summary>
    /// <param name="httpContext">Provide an HttpContext instance to enable auditing support</param>
    /// <remarks>
    /// See this page for how to fake HttpContext just enough to set the principal & identity:
    /// https://stackoverflow.com/questions/4379450/mock-httpcontext-current-in-test-init-method
    /// </remarks>
    public static OtrContext CreateContext(HttpContext? httpContext = null)
    {
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.SetupGet(x => x.HttpContext).Returns(httpContext);

        ISaveChangesInterceptor interceptor = httpContext is null
            ? new AuditingInterceptor()
            : new AuditBlamingInterceptor(httpContextAccessorMock.Object);

        var context = new OtrContext(
            new DbContextOptionsBuilder<OtrContext>()
                .UseNpgsql(ConnectionString)
                .AddInterceptors(interceptor)
                .UseSnakeCaseNamingConvention()
                .Options);

        context.Database.BeginTransaction();
        return context;
    }
}

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    /*
     * This class has no code, and is never created. Its purpose is simply
       to be the place to apply [CollectionDefinition] and all the
       ICollectionFixture<> interfaces.

     * See here: https://xunit.net/docs/shared-context#class-fixture
    */
}
