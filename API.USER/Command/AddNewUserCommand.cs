using API.USER.Entities;
using COMMON.REPO.Abstraction;
using MediatR;

namespace API.USER.Command
{


    public record AddNewUserCommand(UserEntity newUser) : IRequest<bool>;
    public class AddNewUserCommandHandler : IRequestHandler<AddNewUserCommand, bool>
    {
        private IGenericRepository<UserEntity> _repository;

        public AddNewUserCommandHandler(IGenericRepository<UserEntity> repository)
        {
            this._repository = repository;
        }
        public async Task<bool> Handle(AddNewUserCommand request, CancellationToken cancellationToken)
        {
            return await _repository.Insert(request.newUser);
        }
    }
}