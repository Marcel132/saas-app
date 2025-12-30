public interface IRegisterPolicy
{
  void EnsureCanRegister(bool emailAlreadyExists, RegisterRequestDto user);
}