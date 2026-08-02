namespace OrderFlow.Domain.Entities;

public class Usuario
{
    private readonly List<RefreshToken> _refreshTokens = [];

    private Usuario() { }

    public Usuario(string nome, string email, string senhaHash, string papel = "Usuario")
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome é obrigatório.", nameof(nome));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("E-mail é obrigatório.", nameof(email));
        if (string.IsNullOrWhiteSpace(senhaHash)) throw new ArgumentException("Hash da senha é obrigatório.", nameof(senhaHash));

        Id = Guid.NewGuid();
        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
        SenhaHash = senhaHash;
        Papel = papel;
        Ativo = true;
        CriadoEm = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public string Papel { get; private set; } = string.Empty;
    public bool Ativo { get; private set; }
    public DateTimeOffset CriadoEm { get; private set; }
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens;

    public void AdicionarRefreshToken(RefreshToken refreshToken) => _refreshTokens.Add(refreshToken);
}
