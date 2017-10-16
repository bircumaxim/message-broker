using Domain.UseCases;

namespace Data.Commands
{
    public class ExecuteDomainUseCaseCommand : ICommand
    {
        private readonly IUseCase _useCase;

        public ExecuteDomainUseCaseCommand(IUseCase useCase)
        {
            _useCase = useCase;
        }

        public void Execute()
        {
            _useCase?.Execute();
        }
    }
}