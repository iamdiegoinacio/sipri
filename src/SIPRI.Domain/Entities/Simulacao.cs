namespace SIPRI.Domain.Entities;

/// <summary>
/// Representa o registro de uma simulação de investimento realizada.
/// </summary>
public class Simulacao
{
    /// <summary>
    /// Identificador único da simulação.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID do cliente que realizou a simulação.
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// ID do produto de investimento que foi simulado.
    /// </summary>
    public Guid ProdutoId { get; set; }

    /// <summary>
    /// Nome do produto na data da simulação(desnormalizado).
    /// </summary>
    public string ProdutoNome { get; set; } = string.Empty;

    /// <summary>
    /// Valor que foi investido na simulação.
    /// </summary>
    public decimal ValorInvestido { get; set; }

    /// <summary>
    /// Prazo em meses que foi usado na simulação.
    /// </summary>
    public int PrazoMeses { get; set; }

    /// <summary>
    /// Valor final calculado pela simulação.
    /// </summary>
    public decimal ValorFinal { get; set; }

    /// <summary>
    /// Data e hora em que a simulação foi realizada.
    /// </summary>
    public DateTime DataSimulacao { get; set; }

    /// <summary>
    /// Construtor padrão para o EF Core.
    /// </summary>
    public Simulacao() { }
}