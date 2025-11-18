using System.ComponentModel.DataAnnotations;

namespace SIPRI.Application.DTOs.Simulacao;

/// <summary>
/// DTO para a solicitação de uma nova simulação
/// </summary>
public class SimulacaoRequestDto
{
    [Required]
    public Guid ClienteId { get; set; }

    [Required]
    [Range(1, (double)decimal.MaxValue)]
    public decimal Valor { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int PrazoMeses { get; set; }

    [Required]
    [MinLength(1)]
    public string TipoProduto { get; set; } = string.Empty;
}