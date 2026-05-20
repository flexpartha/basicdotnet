namespace UserApi.Shared.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(long id) : base($"User not found with id: {id}") { }
}

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base("Invalid username or email") { }
}
