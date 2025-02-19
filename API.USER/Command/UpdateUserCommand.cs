using API.USER.Entities;
using COMMON.REPO.Abstraction;
using MediatR;

namespace API.USER.Command
{
    public record UpdateUserCommand(UserEntity oldState,UserEntity newState) : IRequest<bool>;
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private IGenericRepository<UserEntity> _repository;

        public UpdateUserCommandHandler(IGenericRepository<UserEntity> repository)
        {
            this._repository = repository;
        }
        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            return await _repository.Update(request.oldState, request.newState);          
        }
    }
}
