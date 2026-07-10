namespace backend.Application.Abstractions.CQRS;

public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
  Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken);
}