using Common.Enums.Verification;
using Database.Entities;

namespace DataWorkerService.Processors.Resolvers.Interfaces;

/// <inheritdoc/>
public interface ITournamentProcessorResolver : IProcessorResolver<Tournament>
{
    /// <summary>
    /// The processor which a tournament with the given <see cref="processingStatus" /> should
    /// be executed against
    /// </summary>
    /// <param name="processingStatus">A <see cref="Tournament" />'s current ProcessingStatus</param>
    /// <returns>
    /// The processor which is to be executed next for any <see cref="Tournament" />
    /// which has the given <see cref="processingStatus" />
    /// </returns>
    IProcessor<Tournament> GetNextProcessor(TournamentProcessingStatus processingStatus);
}
