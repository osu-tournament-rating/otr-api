using API.Entities;
using API.Enums;
using API.Repositories.Interfaces;
using AutoMapper;

namespace API.Repositories.Implementations;

public class MatchHistoryRepository : RepositoryBase<MatchHistory>, IHistoryRepository<MatchHistory, Match>
{
    private readonly IMapper _mapper;

    public MatchHistoryRepository(OtrContext context, IMapper mapper)
        : base(context)
    {
        _mapper = mapper;
    }

    public async Task<MatchHistory?> CreateAsync(Match match, HistoryActionType action)
    {
        MatchHistory record = _mapper.Map<MatchHistory>(match);
        record.HistoryAction = (int)action;
        // Modifications made automatically (without user intervention) have an Id of null
        record.ModifierId = null;
        return await base.CreateAsync(record);
    }
}
