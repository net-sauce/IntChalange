using API.USER.Command;
using API.USER.Entities;
using COMMON.REPO.Abstraction;
using MediatR;

namespace API.USER.Query
{
    public record GetAllUsersQuery() : IRequest<IEnumerable<UserEntity>>;
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserEntity>>
    {
        private IGenericRepository<UserEntity> _repository;

        public GetAllUsersQueryHandler(IGenericRepository<UserEntity> repository)
        {
            this._repository = repository;
        }
        public async Task<IEnumerable<UserEntity>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAll();
        }
    }
}