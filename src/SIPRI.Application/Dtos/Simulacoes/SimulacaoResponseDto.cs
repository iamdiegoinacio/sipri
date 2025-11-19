namespace SIPRI.Application.DTOs.Simulacoes;

/// <summary>
/// DTO para a resposta de uma simulação bem-sucedida.
/// </summary>
public class SimulacaoResponseDto
{
    public ProdutoValidadoDto ProdutoValidado { get; set; } = new();
    public ResultadoSimulacaoDto ResultadoSimulacao { get; set; } = new();
    public DateTime DataSimulacao { get; set; }
}

/// <summary>
/// Parte do DTO de resposta da simulação.
/// </summary>
public class ProdutoValidadoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Rentabilidade { get; set; }
    public string Risco { get; set; } = string.Empty;
}

/// <summary>
/// Parte do DTO de resposta da simulação.
/// </summary>
public class ResultadoSimulacaoDto
{
    public decimal ValorFinal { get; set; }
    public decimal RentabilidadeEfetiva { get; set; }
    public int PrazoMeses { get; set; }
}