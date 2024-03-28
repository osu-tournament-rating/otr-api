using API.Entities;
using API.Enums;
using API.Repositories.Interfaces;
using Moq;

namespace APITests.Framework.MockRepositories;

public class MockRatingAdjustmentsRepository : Mock<IRatingAdjustmentsRepository>
{
    public MockRatingAdjustmentsRepository()
    {
        SetupAll();
    }

    public MockRatingAdjustmentsRepository SetupAll() =>
        SetupGet();

    public MockRatingAdjustmentsRepository SetupGet()
    {
        Setup(x =>
                x.GetAsync(It.IsAny<int>())
            )
            .ReturnsAsync((int id) =>
            {
                RatingAdjustment ratingAdjustment = new()
                {
                    Id = id,
                    PlayerId = Random.Shared.Next() % 10000,
                    Mode = 0,
                    RatingAdjustmentAmount = -2.3333,
                    VolatilityAdjustmentAmount = 0.45,
                    RatingBefore = 1500,
                    RatingAfter = 1500 - 2.333,
                    VolatilityBefore = 200,
                    VolatilityAfter = 200.45,
                    RatingAdjustmentType = (int)RatingAdjustmentType.Decay,
                    Timestamp = default
                };

                return ratingAdjustment;
            });

        return this;
    }
}
