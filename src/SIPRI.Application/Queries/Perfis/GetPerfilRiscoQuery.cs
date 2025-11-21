using MediatR;
using SIPRI.Application.DTOs.Perfis;

namespace SIPRI.Application.Queries.Perfis;

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
