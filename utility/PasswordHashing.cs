namespace TraineeApi.Utility;

public static class PasswordHashing
{
    public static string HashPassword(string Password)
    {
        return BCrypt.Net.BCrypt.HashPassword(Password);
    }

    public static bool VerifyPassword(string Password, string HashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(Password, HashedPassword);
    }
}