namespace backend.Application.Abstractions.CQRS;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
  Task<Result> HandleAsync(TCommand command );
}

public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
  Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}