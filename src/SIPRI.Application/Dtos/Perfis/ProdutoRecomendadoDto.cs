namespace SIPRI.Application.DTOs.Perfis;

/// <summary>
/// DTO para a resposta da consulta de Produtos Recomendados.
/// </summary>
public class ProdutoRecomendadoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Rentabilidade { get; set; }
    public string Risco { get; set; } = string.Empty;
}
