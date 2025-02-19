using API.USER.Entities;
using COMMON.REPO.Abstraction;
using MediatR;

namespace API.USER.Query
{
    public record GetUsersByIdQuery(int userId) : IRequest<UserEntity>;
    public class GetUsersByIdQueryHandler : IRequestHandler<GetUsersByIdQuery, UserEntity>
    {
        private IGenericRepository<UserEntity> _repository;

        public GetUsersByIdQueryHandler(IGenericRepository<UserEntity> repository)
        {
            this._repository = repository;
        }
        public async Task<UserEntity> Handle(GetUsersByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetSingle(x => x.UserID == request.userId);
        }
    }
}
