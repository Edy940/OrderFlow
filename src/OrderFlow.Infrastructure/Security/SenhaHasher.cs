using System.Security.Cryptography;
using OrderFlow.Application.Interfaces;

namespace OrderFlow.Infrastructure.Security;

public class SenhaHasher : ISenhaHasher
{
    private const int Iteracoes = 100_000;
    private const int TamanhoSalt = 16;
    private const int TamanhoHash = 32;

    public string GerarHash(string senha)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(senha);
        var salt = RandomNumberGenerator.GetBytes(TamanhoSalt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, Iteracoes, HashAlgorithmName.SHA256, TamanhoHash);
        return $"{Iteracoes}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verificar(string senha, string hashArmazenado)
    {
        try
        {
            var partes = hashArmazenado.Split('.', 3);
            if (partes.Length != 3 || !int.TryParse(partes[0], out var iteracoes)) return false;
            var salt = Convert.FromBase64String(partes[1]);
            var hashEsperado = Convert.FromBase64String(partes[2]);
            var hashCalculado = Rfc2898DeriveBytes.Pbkdf2(
                senha, salt, iteracoes, HashAlgorithmName.SHA256, hashEsperado.Length);
            return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
