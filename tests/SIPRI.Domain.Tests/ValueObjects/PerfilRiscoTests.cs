using FluentAssertions;
using SIPRI.Domain.ValueObjects;

namespace SIPRI.Domain.Tests.ValueObjects;

public class PerfilRiscoTests
{
    [Fact]
    public void ConservadorPadrao_ShouldReturnDefaultConservativeProfile()
    {
        // Act
        var perfil = PerfilRisco.ConservadorPadrao;

        // Assert
        perfil.Should().NotBeNull();
        perfil.Pontuacao.Should().Be(0);
        perfil.Perfil.Should().Be("Conservador");
        perfil.Descricao.Should().Be("Foco em liquidez e baixa movimentação.");
    }

    [Theory]
    [InlineData(0, "Conservador", "Foco em liquidez e baixa movimentação.")]
    [InlineData(10, "Conservador", "Foco em liquidez e baixa movimentação.")]
    [InlineData(35, "Conservador", "Foco em liquidez e baixa movimentação.")]
    public void Create_ShouldReturnConservador_WhenPontuacaoIsLowOrEqual35(int pontuacao, string expectedPerfil, string expectedDescricao)
    {
        // Act
        var perfil = PerfilRisco.Create(pontuacao);

        // Assert
        perfil.Pontuacao.Should().Be(pontuacao);
        perfil.Perfil.Should().Be(expectedPerfil);
        perfil.Descricao.Should().Be(expectedDescricao);
    }

    [Theory]
    [InlineData(36, "Moderado", "Perfil equilibrado entre segurança e rentabilidade.")]
    [InlineData(50, "Moderado", "Perfil equilibrado entre segurança e rentabilidade.")]
    [InlineData(70, "Moderado", "Perfil equilibrado entre segurança e rentabilidade.")]
    public void Create_ShouldReturnModerado_WhenPontuacaoIsBetween36And70(int pontuacao, string expectedPerfil, string expectedDescricao)
    {
        // Act
        var perfil = PerfilRisco.Create(pontuacao);

        // Assert
        perfil.Pontuacao.Should().Be(pontuacao);
        perfil.Perfil.Should().Be(expectedPerfil);
        perfil.Descricao.Should().Be(expectedDescricao);
    }

    [Theory]
    [InlineData(71, "Agressivo", "Busca por alta rentabilidade, maior risco.")]
    [InlineData(80, "Agressivo", "Busca por alta rentabilidade, maior risco.")]
    [InlineData(100, "Agressivo", "Busca por alta rentabilidade, maior risco.")]
    [InlineData(1000, "Agressivo", "Busca por alta rentabilidade, maior risco.")]
    public void Create_ShouldReturnAgressivo_WhenPontuacaoIsGreaterThan70(int pontuacao, string expectedPerfil, string expectedDescricao)
    {
        // Act
        var perfil = PerfilRisco.Create(pontuacao);

        // Assert
        perfil.Pontuacao.Should().Be(pontuacao);
        perfil.Perfil.Should().Be(expectedPerfil);
        perfil.Descricao.Should().Be(expectedDescricao);
    }

    [Fact]
    public void Create_ShouldNormalizeNegativePontuacaoToZero()
    {
        // Act
        var perfil = PerfilRisco.Create(-10);

        // Assert
        perfil.Pontuacao.Should().Be(0);
        perfil.Perfil.Should().Be("Conservador");
    }

    [Fact]
    public void Create_ShouldHandleBoundaryValue_35()
    {
        // Act
        var perfil = PerfilRisco.Create(35);

        // Assert
        perfil.Perfil.Should().Be("Conservador");
    }

    [Fact]
    public void Create_ShouldHandleBoundaryValue_36()
    {
        // Act
        var perfil = PerfilRisco.Create(36);

        // Assert
        perfil.Perfil.Should().Be("Moderado");
    }

    [Fact]
    public void Create_ShouldHandleBoundaryValue_70()
    {
        // Act
        var perfil = PerfilRisco.Create(70);

        // Assert
        perfil.Perfil.Should().Be("Moderado");
    }

    [Fact]
    public void Create_ShouldHandleBoundaryValue_71()
    {
        // Act
        var perfil = PerfilRisco.Create(71);

        // Assert
        perfil.Perfil.Should().Be("Agressivo");
    }

    [Fact]
    public void PerfilRisco_ShouldBeImmutable()
    {
        // Act
        var perfil = PerfilRisco.Create(50);

        // Assert
        perfil.Pontuacao.Should().Be(50);
        // Não há setters públicos, garantindo imutabilidade
        perfil.GetType().GetProperty(nameof(perfil.Pontuacao))!.CanWrite.Should().BeFalse();
        perfil.GetType().GetProperty(nameof(perfil.Perfil))!.CanWrite.Should().BeFalse();
        perfil.GetType().GetProperty(nameof(perfil.Descricao))!.CanWrite.Should().BeFalse();
    }
}
