using MediatR;
using SIPRI.Application.DTOs.Simulacoes;

namespace SIPRI.Application.Commands.Simulacoes;

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
