using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Services;
using SIPRI.Domain.ValueObjects;

namespace SIPRI.Domain.Services;

/// <summary>
/// Orquestra o cálculo do perfil de risco
/// executando um conjunto de regras de pontuação (Padrão Strategy).
/// </summary>
public sealed class MotorPerfilRiscoServico : IMotorPerfilRiscoServico
{
    private readonly IEnumerable<IRegraDePontuacao> _regrasDePontuacao;

    /// <summary>
    /// Injeta todas as regras de pontuação
    /// registradas no contêiner de Injeção de Dependência.
    /// </summary>
    public MotorPerfilRiscoServico(IEnumerable<IRegraDePontuacao> regrasDePontuacao)
    {
        ArgumentNullException.ThrowIfNull(regrasDePontuacao);

        if (!regrasDePontuacao.Any())
        {
            throw new ArgumentException("Nenhuma regra de pontuação foi injetada.", nameof(regrasDePontuacao));
        }

        _regrasDePontuacao = regrasDePontuacao;
    }

    /// <summary>
    /// Calcula o perfil de risco com base na carteira de investimentos e produtos.
    /// </summary>
    public PerfilRisco CalcularPerfil(
        IReadOnlyCollection<Investimento> investimentos,
        IReadOnlyCollection<ProdutoInvestimento> produtos,
        DateTime dataReferencia)
    {
        // --- VALIDAÇÃO DE ENTRADA (GUARD CLAUSES) ---
        ArgumentNullException.ThrowIfNull(investimentos);
        ArgumentNullException.ThrowIfNull(produtos);

        if (dataReferencia == DateTime.MinValue)
        {
            throw new ArgumentException("A data de referência não pode ser MinValue.", nameof(dataReferencia));
        }

        // --- LÓGICA DE NEGÓCIO ---
        if (!investimentos.Any() || !produtos.Any())
        {
            return PerfilRisco.ConservadorPadrao;
        }

        // --- CRIAR O CONTEXTO ---
        // Cria um único objeto que encapsula todos os dados.
        var contexto = new CalculoRiscoContexto(investimentos, produtos, dataReferencia);

        // --- ORQUESTRAÇÃO ---
        int pontuacaoFinal = _regrasDePontuacao
            .Sum(regra => regra.CalcularPontuacao(contexto));

        // --- CRIAÇÃO DO RESULTADO ---
        return PerfilRisco.Create(pontuacaoFinal);
    }
}