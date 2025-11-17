using SIPRI.Domain.Entities;

namespace SIPRI.Domain.Interfaces;

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
}