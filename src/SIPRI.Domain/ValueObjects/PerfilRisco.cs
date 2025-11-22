namespace SIPRI.Domain.ValueObjects;

/// <summary>
/// Representa o perfil de risco calculado como um Value Object.
/// Garante que um perfil só possa ser criado com um estado válido.
/// </summary>
public class PerfilRisco
{
    // Constantes de mapeamento que pertencem a este VO
    private const int LimitePontuacaoConservador = 35;
    private const int LimitePontuacaoModerado = 70;

    /// <summary>
    /// Um perfil padrão para casos sem dados,
    /// garantindo consistência.
    /// </summary>
    public static readonly PerfilRisco ConservadorPadrao = Create(0);

    public int Pontuacao { get; }
    public string Perfil { get; }
    public string Descricao { get; }

    /// <summary>
    /// O construtor é privado para forçar a criação
    /// através do método de fábrica "Create",
    /// garantindo a validação.
    /// </summary>
    private PerfilRisco(int pontuacao, string perfil, string descricao)
    {
        Pontuacao = pontuacao;
        Perfil = perfil;
        Descricao = descricao;
    }

    /// <summary>
    /// Método de Fábrica: O único portão de entrada para criar um PerfilRisco.
    /// Contém a lógica de negócio para mapear a pontuação.
    /// </summary>
    public static PerfilRisco Create(int pontuacao)
    {
        // Garante que a pontuação nunca seja negativa
        if (pontuacao < 0)
            pontuacao = 0;

        // A lógica de mapeamento agora vive aqui,
        // protegendo as regras de negócio.
        var (perfil, descricao) = pontuacao switch
        {
            <= LimitePontuacaoConservador => ("Conservador", "Foco em liquidez e baixa movimentação."),
            <= LimitePontuacaoModerado => ("Moderado", "Perfil equilibrado entre segurança e rentabilidade."),
            _ => ("Agressivo", "Busca por alta rentabilidade, maior risco.")
        };

        // Chama o construtor privado,
        // garantindo um objeto 100% válido.
        return new PerfilRisco(pontuacao, perfil, descricao);
    }
}