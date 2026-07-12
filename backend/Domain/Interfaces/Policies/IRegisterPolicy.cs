using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Auth.Commands;

namespace backend.Domain.Interfaces;

public interface IRegisterPolicy
{
  public Result CanRegisterPentester(bool emailAlreadyExists, bool nicknameAlreadyExists, RegisterPentesterCommand req);
  public Result CanRegisterCompany(bool emailAlreadyExists, RegisterCompanyCommand req);
}