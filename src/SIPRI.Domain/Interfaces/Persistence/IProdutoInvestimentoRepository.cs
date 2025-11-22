using SIPRI.Domain.Entities;

namespace SIPRI.Domain.Interfaces.Persistence;

public interface IProdutoInvestimentoRepository
{
    /// <summary>
    /// Busca um produto pelo seu ID.
    /// </summary>
    Task<ProdutoInvestimento?> GetByIdAsync(Guid id);

    /// <summary>
    /// Busca todos os produtos.
    /// </summary>
    Task<IEnumerable<ProdutoInvestimento>> GetAllAsync();

    /// <summary>
    /// Busca produtos recomendados com base no perfil de risco.
    /// </summary>
    Task<IEnumerable<ProdutoInvestimento>> GetByPerfilRiscoAsync(string perfilRisco);

    /// <summary>
    /// Busca um produto adequado com base no tipo informado na simulação.
    /// (Ex: Busca um produto do tipo "CDB" ou "Fundo").
    /// </summary>
    Task<ProdutoInvestimento?> GetByTipoAsync(string tipoProduto);
}