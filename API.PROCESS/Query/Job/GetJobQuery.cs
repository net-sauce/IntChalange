using API.PROCESS.Entities;
using API.PROCESS.Query.Process;
using COMMON.REPO.Abstraction;
using MediatR;

namespace API.PROCESS.Query.Job
{
   public record GetJobStatusQuery(Guid job_id) : IRequest<JobEnity>;

    public class GetJobStatusQueryQueryHandler : IRequestHandler<GetJobStatusQuery, JobEnity>
    {
        private readonly IGenericRepository<JobEnity> _repository;

        public GetJobStatusQueryQueryHandler(IGenericRepository<JobEnity> repository)
        {
            _repository = repository;
        }

        public Task<JobEnity> Handle(GetJobStatusQuery request, CancellationToken cancellationToken)
        {
        
            return _repository.GetSingle(x => x.JobID == request.job_id);
        }
    }

}
