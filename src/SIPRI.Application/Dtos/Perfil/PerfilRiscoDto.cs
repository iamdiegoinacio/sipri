namespace SIPRI.Application.DTOs.Perfil;

/// <summary>
/// DTO para a resposta da consulta de Perfil de Risco.
/// </summary>
public class PerfilRiscoDto
{
    public Guid ClienteId { get; set; }
    public string Perfil { get; set; } = string.Empty;
    public int Pontuacao { get; set; }
    public string Descricao { get; set; } = string.Empty;
}