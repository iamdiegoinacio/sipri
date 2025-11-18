namespace SIPRI.Domain.Entities;

/// <summary>
/// Representa um produto de investimento disponível para simulação e recomendação.
/// </summary>
public class ProdutoInvestimento
{
    /// <summary>
    /// Identificador único do produto.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome comercial do produto (ex: "CDB Caixa 2026").
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Tipo do produto (ex: "CDB", "Fundo", "LCI").
    /// </summary>
    public string Tipo { get; set; } = string.Empty;

    /// <summary>
    /// A taxa de rentabilidade base do produto (ex: 0.12 para 12%).
    /// </summary>
    public decimal RentabilidadeBase { get; set; }

    /// <summary>
    /// Descrição textual do risco (ex: "Baixo", "Moderado", "Alto").
    /// </summary>
    public string Risco { get; set; } = string.Empty;

    /// <summary>
    /// Representação numérica do risco.
    /// Usado internamente pelo MotorPerfilRiscoService.
    /// Ex: Baixo = 1, Moderado = 2, Alto = 3.
    /// </summary>
    public int NivelRisco { get; set; }

    /// <summary>
    /// Construtor padrão para o EF Core e serialização.
    /// </summary>
    public ProdutoInvestimento() { }

    /// <summary>
    /// Construtor
    /// </summary>
    public ProdutoInvestimento(Guid id, string nome, string tipo, decimal rentabilidadeBase, string risco, int nivelRisco)
    {
        Id = id;
        Nome = nome;
        Tipo = tipo;
        RentabilidadeBase = rentabilidadeBase;
        Risco = risco;
        NivelRisco = nivelRisco;
    }
}