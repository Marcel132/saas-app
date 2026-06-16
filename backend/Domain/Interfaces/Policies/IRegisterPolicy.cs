using backend.Api.Controllers.Auth.DTOs;

namespace backend.Domain.Interfaces;

public interface IRegisterPolicy
{
  void EnsureCanRegister(bool emailAlreadyExists, RegisterRequestDto user);
}