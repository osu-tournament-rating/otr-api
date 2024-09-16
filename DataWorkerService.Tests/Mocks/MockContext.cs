using Database;
using Moq;

namespace DataWorkerService.Tests.Mocks;

public class MockContext : Mock<OtrContext>
{
    public MockContext()
    {
        SetupSaveChangesAsync();
    }

    private void SetupSaveChangesAsync()
    {
        Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
    }
}
