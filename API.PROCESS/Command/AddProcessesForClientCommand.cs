using API.PROCESS.Entities;
using COMMON.REPO.Abstraction;
using MediatR;

namespace API.PROCESS.Command
{
    public record AddProcessesForClientCommand(ProcessEntity process):IRequest<bool>;
    public class AddProcessesForClientCommandCommandHandler : IRequestHandler<AddProcessesForClientCommand, bool>
    {
        private IGenericRepository<ProcessEntity> _repository;

        public AddProcessesForClientCommandCommandHandler(IGenericRepository<ProcessEntity> repository)
        {
            this._repository = repository;
        }
        public async Task<bool> Handle(AddProcessesForClientCommand request, CancellationToken cancellationToken)
        {
            return await _repository.Insert(request.process);
        }
    }

}
