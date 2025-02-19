using API.PROCESS.Entities;
using COMMON.REPO.Abstraction;
using MediatR;

namespace API.PROCESS.Query.Job
{
    public record GetAllJobsQuery(int client_id) : IRequest<IEnumerable<JobEnity>>;
    public class GetAllJobsQueryHandler : IRequestHandler<GetAllJobsQuery, IEnumerable<JobEnity>>
    {
        private readonly IGenericRepository<JobEnity> _repository;

        public GetAllJobsQueryHandler(IGenericRepository<JobEnity> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<JobEnity>> Handle(GetAllJobsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllMatches(x => x.ClientID == request.client_id);
        }
    }

}

