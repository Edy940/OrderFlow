namespace OrderFlow.Domain.Entities;

public class RefreshToken
{
    private RefreshToken() { }

    public RefreshToken(Guid usuarioId, string tokenHash, DateTimeOffset expiraEm)
    {
        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        TokenHash = tokenHash;
        CriadoEm = DateTimeOffset.UtcNow;
        ExpiraEm = expiraEm;
    }

    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTimeOffset CriadoEm { get; private set; }
    public DateTimeOffset ExpiraEm { get; private set; }
    public DateTimeOffset? RevogadoEm { get; private set; }
    public string? SubstituidoPorTokenHash { get; private set; }
    public Usuario Usuario { get; private set; } = null!;

    public bool EstaAtivo(DateTimeOffset agora) => RevogadoEm is null && ExpiraEm > agora;

    public void Revogar(string? substituidoPorTokenHash = null)
    {
        RevogadoEm = DateTimeOffset.UtcNow;
        SubstituidoPorTokenHash = substituidoPorTokenHash;
    }
}
