using SIPRI.Domain.Contexts;
using SIPRI.Domain.Interfaces.Services;

namespace SIPRI.Domain.Services;

/// <summary>
/// Implementa a regra de Juros Simples
/// Esta regra se aplica ao tipo "CDB".
/// </summary>
public sealed class RegraCalculoCDB : IRegraCalculoInvestimento
{
    // Define o tipo de produto que esta regra atende.
    public string TipoProduto => "CDB";

    /// <summary>
    /// Fórmula: ValorFinal = ValorInvestido + Juros
    /// Juros = ValorInvestido * RentabilidadeBaseAnual * (PrazoMeses / 12)
    /// </summary>
    public decimal Calcular(CalculoInvestimentoContexto contexto)
    {
        ArgumentNullException.ThrowIfNull(contexto.Produto);

        // Converte o prazo de meses para anos (fração)
        decimal prazoEmAnos = (decimal)contexto.PrazoMeses / 12;

        decimal juros = contexto.ValorInvestido * contexto.Produto.RentabilidadeBase * prazoEmAnos;

        decimal valorFinal = contexto.ValorInvestido + juros;

        // Arredonda para 2 casas decimais (padrão monetário)
        return Math.Round(valorFinal, 2);
    }
}