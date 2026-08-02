namespace OrderFlow.Infrastructure.Security;

public class JwtSettings
{
    public const string Secao = "Jwt";
    public string Chave { get; set; } = string.Empty;
    public string Emissor { get; set; } = string.Empty;
    public string Audiencia { get; set; } = string.Empty;
    public int AccessTokenMinutos { get; set; } = 15;
    public int RefreshTokenDias { get; set; } = 7;
}
