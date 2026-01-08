namespace WireMess.Utils.AuthUtil.Interfaces
{
    public interface IPasswordHasher
    {
        (string Hash, string Salt) CreateHash(string password);
        bool VerifyHash(string password, string hash, string salt);
    }
}
