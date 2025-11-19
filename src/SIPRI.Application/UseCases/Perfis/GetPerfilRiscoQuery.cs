using MediatR;
using SIPRI.Application.DTOs.Perfis;
using SIPRI.Application.Interfaces;
using SIPRI.Domain.Interfaces.Persistence;
using SIPRI.Domain.Interfaces.Services;

namespace SIPRI.Application.UseCases.Perfis;

/// <summary>
/// Query para consultar e calcular o perfil de risco atual do cliente.
/// </summary>
public class GetPerfilRiscoQuery : IRequest<PerfilRiscoDto>
{
    public Guid ClienteId { get; }

    public GetPerfilRiscoQuery(Guid clienteId)
    {
        ClienteId = clienteId;
    }
}

/// <summary>
/// Handler que orquestra a obtenção de dados e o acionamento do Motor de Risco.
/// </summary>
public class GetPerfilRiscoHandler : IRequestHandler<GetPerfilRiscoQuery, PerfilRiscoDto>
{
    private readonly IInvestimentoRepository _investimentoRepository;
    private readonly IProdutoInvestimentoRepository _produtoRepository;
    private readonly IMotorPerfilRiscoServico _motorRisco;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetPerfilRiscoHandler(
        IInvestimentoRepository investimentoRepository,
        IProdutoInvestimentoRepository produtoRepository,
        IMotorPerfilRiscoServico motorRisco,
        IDateTimeProvider dateTimeProvider)
    {
        _investimentoRepository = investimentoRepository;
        _produtoRepository = produtoRepository;
        _motorRisco = motorRisco;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<PerfilRiscoDto> Handle(GetPerfilRiscoQuery request, CancellationToken cancellationToken)
    {
        // 1. Buscar dados necessários para o cálculo
        var investimentos = await _investimentoRepository.GetByClienteIdAsync(request.ClienteId);
        var produtos = await _produtoRepository.GetAllAsync();

        // 2. Executar o Motor de Risco (Domain Service)
        var perfilCalculado = _motorRisco.CalcularPerfil(
            investimentos.ToList(),
            produtos.ToList(),
            _dateTimeProvider.Today
        );

        // 3. Mapear VO (Domain) para DTO (Application)
        return new PerfilRiscoDto
        {
            ClienteId = request.ClienteId,
            Perfil = perfilCalculado.Perfil,
            Pontuacao = perfilCalculado.Pontuacao,
            Descricao = perfilCalculado.Descricao
        };
    }
}