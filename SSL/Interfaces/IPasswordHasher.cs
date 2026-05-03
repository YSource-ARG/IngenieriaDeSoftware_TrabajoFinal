namespace SSL.Interfaces
{
    public interface IPasswordHasher
    {
        string GenerarHash(string password);
        bool VerificarPassword(string passwordIngresada, string passwordHashAlmacenado);
    }
}
