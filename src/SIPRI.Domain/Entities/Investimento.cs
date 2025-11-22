namespace SIPRI.Domain.Entities;

/// <summary>
/// Representa um investimento existente na carteira do cliente.
/// Esta é uma Entidade e será parte da agregação do Cliente (implícito) 
/// ou consultada diretamente.
/// </summary>
public class Investimento
{
    /// <summary>
    /// Identificador único do investimento.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID do cliente proprietário deste investimento.
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// ID do produto de investimento associado (ex: CDB Caixa 2026).
    /// </summary>
    public Guid ProdutoId { get; set; }

    /// <summary>
    /// Tipo do produto (ex: "CDB", "Fundo Multimercado").
    /// </summary>
    public string Tipo { get; set; } = string.Empty;

    /// <summary>
    /// Valor monetário atual do investimento.
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Rentabilidade registrada para este investimento específico.
    /// </summary>
    public decimal Rentabilidade { get; set; }

    /// <summary>
    /// Data em que o investimento foi realizado ou atualizado.
    /// </summary>
    public DateTime Data { get; set; }

    /// <summary>
    /// Construtor padrão para o EF Core.
    /// </summary>
    public Investimento() { }
}