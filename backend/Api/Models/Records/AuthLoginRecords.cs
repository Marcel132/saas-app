public record AuthLoginResult
(
  bool Success,
  UsersModel? User,
  string HttpCode
);


