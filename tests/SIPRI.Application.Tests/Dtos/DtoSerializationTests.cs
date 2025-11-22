using System.Text.Json;
using SIPRI.Application.DTOs.Investimentos;
using SIPRI.Application.DTOs.Perfis;
using SIPRI.Application.DTOs.Simulacoes;
using SIPRI.Application.DTOs.Telemetria;
using Xunit;

namespace SIPRI.Application.Tests.Dtos;

public class DtoSerializationTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    #region HistoricoInvestimentoDto Tests

    [Fact]
    public void HistoricoInvestimentoDto_ShouldSerializeToJson()
    {
        // Arrange
        var dto = new HistoricoInvestimentoDto
        {
            Id = Guid.NewGuid(),
            Tipo = "CDB Premium",
            Valor = 10000m,
            Rentabilidade = 12.5m,
            Data = new DateTime(2024, 1, 1)
        };

        // Act
        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        var deserialized = JsonSerializer.Deserialize<HistoricoInvestimentoDto>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(dto.Id, deserialized.Id);
        Assert.Equal(dto.Tipo, deserialized.Tipo);
        Assert.Equal(dto.Valor, deserialized.Valor);
        Assert.Equal(dto.Rentabilidade, deserialized.Rentabilidade);
        Assert.Equal(dto.Data, deserialized.Data);
    }

    #endregion

    #region PerfilRiscoDto Tests

    [Fact]
    public void PerfilRiscoDto_ShouldSerializeToJson()
    {
        // Arrange
        var dto = new PerfilRiscoDto
        {
            ClienteId = Guid.NewGuid(),
            Perfil = "Moderado",
            Pontuacao = 75,
            Descricao = "Perfil equilibrado"
        };

        // Act
        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        var deserialized = JsonSerializer.Deserialize<PerfilRiscoDto>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(dto.ClienteId, deserialized.ClienteId);
        Assert.Equal(dto.Perfil, deserialized.Perfil);
        Assert.Equal(dto.Pontuacao, deserialized.Pontuacao);
        Assert.Equal(dto.Descricao, deserialized.Descricao);
    }

    #endregion

    #region ProdutoRecomendadoDto Tests

    [Fact]
    public void ProdutoRecomendadoDto_ShouldSerializeToJson()
    {
        // Arrange
        var dto = new ProdutoRecomendadoDto
        {
            Id = Guid.NewGuid(),
            Nome = "CDB Premium",
            Tipo = "Renda Fixa",
            Risco = "Baixo",
            Rentabilidade = 0.12m
        };

        // Act
        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        var deserialized = JsonSerializer.Deserialize<ProdutoRecomendadoDto>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(dto.Id, deserialized.Id);
        Assert.Equal(dto.Nome, deserialized.Nome);
        Assert.Equal(dto.Tipo, deserialized.Tipo);
        Assert.Equal(dto.Risco, deserialized.Risco);
        Assert.Equal(dto.Rentabilidade, deserialized.Rentabilidade);
    }

    #endregion

    #region HistoricoSimulacaoDto Tests

    [Fact]
    public void HistoricoSimulacaoDto_ShouldSerializeToJson()
    {
        // Arrange
        var dto = new HistoricoSimulacaoDto
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            Produto = "CDB Premium",
            ValorInvestido = 10000m,
            ValorFinal = 11000m,
            PrazoMeses = 12,
            DataSimulacao = new DateTime(2024, 1, 1)
        };

        // Act
        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        var deserialized = JsonSerializer.Deserialize<HistoricoSimulacaoDto>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(dto.Id, deserialized.Id);
        Assert.Equal(dto.ClienteId, deserialized.ClienteId);
        Assert.Equal(dto.Produto, deserialized.Produto);
        Assert.Equal(dto.ValorInvestido, deserialized.ValorInvestido);
        Assert.Equal(dto.ValorFinal, deserialized.ValorFinal);
        Assert.Equal(dto.PrazoMeses, deserialized.PrazoMeses);
        Assert.Equal(dto.DataSimulacao, deserialized.DataSimulacao);
    }

    #endregion

    #region SimulacaoAgregadaDto Tests

    [Fact]
    public void SimulacaoAgregadaDto_ShouldSerializeToJson()
    {
        // Arrange
        var dto = new SimulacaoAgregadaDto
        {
            Produto = "CDB",
            Data = new DateOnly(2024, 1, 1),
            QuantidadeSimulacoes = 10,
            MediaValorFinal = 11500m
        };

        // Act
        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        var deserialized = JsonSerializer.Deserialize<SimulacaoAgregadaDto>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(dto.Produto, deserialized.Produto);
        Assert.Equal(dto.Data, deserialized.Data);
        Assert.Equal(dto.QuantidadeSimulacoes, deserialized.QuantidadeSimulacoes);
        Assert.Equal(dto.MediaValorFinal, deserialized.MediaValorFinal);
    }

    #endregion

    #region SimulacaoRequestDto Tests

    [Fact]
    public void SimulacaoRequestDto_ShouldSerializeToJson()
    {
        // Arrange
        var dto = new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = "CDB",
            Valor = 10000m,
            PrazoMeses = 12
        };

        // Act
        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        var deserialized = JsonSerializer.Deserialize<SimulacaoRequestDto>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(dto.ClienteId, deserialized.ClienteId);
        Assert.Equal(dto.TipoProduto, deserialized.TipoProduto);
        Assert.Equal(dto.Valor, deserialized.Valor);
        Assert.Equal(dto.PrazoMeses, deserialized.PrazoMeses);
    }

    #endregion

    #region SimulacaoResponseDto Tests

    [Fact]
    public void SimulacaoResponseDto_ShouldSerializeToJson()
    {
        // Arrange
        var dto = new SimulacaoResponseDto
        {
            DataSimulacao = new DateTime(2024, 1, 1),
            ProdutoValidado = new ProdutoValidadoDto
            {
                Id = Guid.NewGuid(),
                Nome = "CDB Premium",
                Tipo = "Renda Fixa",
                Rentabilidade = 0.12m,
                Risco = "Baixo"
            },
            ResultadoSimulacao = new ResultadoSimulacaoDto
            {
                ValorFinal = 11000m,
                RentabilidadeEfetiva = 1000m,
                PrazoMeses = 12
            }
        };

        // Act
        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        var deserialized = JsonSerializer.Deserialize<SimulacaoResponseDto>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(dto.DataSimulacao, deserialized.DataSimulacao);
        
        Assert.NotNull(deserialized.ProdutoValidado);
        Assert.Equal(dto.ProdutoValidado.Id, deserialized.ProdutoValidado.Id);
        Assert.Equal(dto.ProdutoValidado.Nome, deserialized.ProdutoValidado.Nome);
        
        Assert.NotNull(deserialized.ResultadoSimulacao);
        Assert.Equal(dto.ResultadoSimulacao.ValorFinal, deserialized.ResultadoSimulacao.ValorFinal);
    }

    #endregion

    #region TelemetriaDto Tests

    [Fact]
    public void TelemetriaDto_ShouldSerializeToJson()
    {
        // Arrange
        var dto = new TelemetriaDto
        {
            Periodo = new PeriodoTelemetriaDto
            {
                Inicio = new DateOnly(2024, 1, 1),
                Fim = new DateOnly(2024, 1, 31)
            },
            Servicos = new List<ServicoTelemetriaDto>
            {
                new ServicoTelemetriaDto
                {
                    Nome = "Simulacao",
                    QuantidadeChamadas = 100,
                    MediaTempoRespostaMs = 50
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        var deserialized = JsonSerializer.Deserialize<TelemetriaDto>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserialized);
        Assert.NotNull(deserialized.Periodo);
        Assert.Equal(dto.Periodo.Inicio, deserialized.Periodo.Inicio);
        
        Assert.NotNull(deserialized.Servicos);
        Assert.Single(deserialized.Servicos);
        Assert.Equal(dto.Servicos[0].Nome, deserialized.Servicos[0].Nome);
    }

    #endregion
}
