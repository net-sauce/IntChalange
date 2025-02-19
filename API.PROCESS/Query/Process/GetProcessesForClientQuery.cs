using API.PROCESS.Entities;
using COMMON.REPO.Abstraction;
using COMMON.REPO.Implementation;
using MediatR;

namespace API.PROCESS.Query.Process
{
    public record GetProcessesForClientQuery(int client_id) : IRequest<IEnumerable<ProcessEntity>>;

    public class GetProcessesForClientQueryHandler : IRequestHandler<GetProcessesForClientQuery, IEnumerable<ProcessEntity>>
    {
        private readonly IGenericRepository<ProcessEntity> _repository;

        public GetProcessesForClientQueryHandler(IGenericRepository<ProcessEntity> repository)
        {
            _repository = repository;
        }       

        public Task<IEnumerable<ProcessEntity>> Handle(GetProcessesForClientQuery request, CancellationToken cancellationToken)
        {
           return _repository.GetAllMatches(x => x.ClientID == request.client_id);
        }
    }

}
