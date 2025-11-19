using MediatR;
using SIPRI.Application.DTOs.Simulacoes;
using SIPRI.Application.Exceptions;
using SIPRI.Application.Interfaces;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Services;
using SIPRI.Domain.Interfaces.Persistence;

namespace SIPRI.Application.UseCases.Simulacoes;

/// <summary>
/// Comando (CQRS) para realizar uma simulação de investimento.
/// Implementa IRequest do MediatR retornando o DTO de resposta.
/// </summary>
public class SimularInvestimentoCommand : IRequest<SimulacaoResponseDto>
{
    public SimulacaoRequestDto RequestData { get; }

    public SimularInvestimentoCommand(SimulacaoRequestDto requestData)
    {
        RequestData = requestData ?? throw new ArgumentNullException(nameof(requestData));
    }
}

/// <summary>
/// Handler que processa o SimularInvestimentoCommand.
/// </summary>
public class SimularInvestimentoHandler : IRequestHandler<SimularInvestimentoCommand, SimulacaoResponseDto>
{
    // Dependências do Domínio
    private readonly IProdutoInvestimentoRepository _produtoRepository;
    private readonly ISimulacaoRepository _simulacaoRepository;
    private readonly ICalculadoraInvestimentoService _calculadoraService;
    private readonly IUnitOfWork _unitOfWork;

    // Dependências da Aplicação
    private readonly IDateTimeProvider _dateTimeProvider;

    public SimularInvestimentoHandler(
        IProdutoInvestimentoRepository produtoRepository,
        ISimulacaoRepository simulacaoRepository,
        ICalculadoraInvestimentoService calculadoraService,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _produtoRepository = produtoRepository;
        _simulacaoRepository = simulacaoRepository;
        _calculadoraService = calculadoraService;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<SimulacaoResponseDto> Handle(SimularInvestimentoCommand request, CancellationToken cancellationToken)
    {
        var dto = request.RequestData;

        // 1. Buscar e Validar Produto
        // Busca no banco um produto que atenda ao "Tipo" solicitado (ex: "CDB")
        var produtoValidado = await _produtoRepository.GetByTipoAsync(dto.TipoProduto);

        if (produtoValidado == null)
        {
            // Lança NotFoundException (Mapeado para 404 no Middleware)
            throw new NotFoundException(nameof(ProdutoInvestimento), dto.TipoProduto);
        }

        // 2. Realizar Cálculo
        // Delega a lógica complexa para o Domain Service
        var contextoCalculo = new CalculoInvestimentoContexto(
            dto.Valor,
            dto.PrazoMeses,
            produtoValidado
        );

        // A calculadora escolhe a estratégia correta (Strategy Pattern)
        decimal valorFinal = _calculadoraService.Calcular(contextoCalculo);

        // 3. Persistir a Simulação
        var simulacao = new Simulacao
        {
            Id = Guid.NewGuid(),
            ClienteId = dto.ClienteId,
            ProdutoId = produtoValidado.Id,
            ProdutoNome = produtoValidado.Nome, // Desnormalização
            ValorInvestido = dto.Valor,
            PrazoMeses = dto.PrazoMeses,
            ValorFinal = valorFinal,
            DataSimulacao = _dateTimeProvider.UtcNow
        };

        await _simulacaoRepository.AddAsync(simulacao);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Retornar DTO
        return new SimulacaoResponseDto
        {
            DataSimulacao = simulacao.DataSimulacao,
            ProdutoValidado = new ProdutoValidadoDto
            {
                Id = produtoValidado.Id,
                Nome = produtoValidado.Nome,
                Tipo = produtoValidado.Tipo,
                Rentabilidade = produtoValidado.RentabilidadeBase,
                Risco = produtoValidado.Risco
            },
            ResultadoSimulacao = new ResultadoSimulacaoDto
            {
                ValorFinal = valorFinal,
                PrazoMeses = dto.PrazoMeses,
                // Calcula a rentabilidade efetiva para exibição
                RentabilidadeEfetiva = (dto.Valor == 0) ? 0 : (valorFinal - dto.Valor) / dto.Valor
            }
        };
    }
}