using backend.Api.Controllers.Auth.DTOs;

namespace backend.Domain.Interfaces;

public interface IRegisterPolicy
{
  public void EnsureCanRegisterPentester(bool emailAlreadyExists, bool nicknameAlreadyExists, RegisterPentesterRequestDto req);
  public void EnsureCanRegisterCompany(bool emailAlreadyExists, RegisterCompanyRequestDto req);
}