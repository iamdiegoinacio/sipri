using FluentAssertions;
using SIPRI.Domain.ValueObjects;

namespace SIPRI.Domain.Tests.ValueObjects;

public class PerfilRiscoTests
{
    [Theory]
    [InlineData(0, "Conservador")]
    [InlineData(35, "Conservador")]
    [InlineData(36, "Moderado")]
    [InlineData(70, "Moderado")]
    [InlineData(71, "Agressivo")]
    [InlineData(100, "Agressivo")]
    public void Create_ShouldReturnCorrectProfile_BasedOnScore(int score, string expectedProfile)
    {
        // Act
        var perfil = PerfilRisco.Create(score);

        // Assert
        perfil.Perfil.Should().Be(expectedProfile);
        perfil.Pontuacao.Should().Be(Math.Max(0, score));
        perfil.Descricao.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Create_ShouldHandleNegativeScore_AsZero()
    {
        // Act
        var perfil = PerfilRisco.Create(-10);

        // Assert
        perfil.Pontuacao.Should().Be(0);
        perfil.Perfil.Should().Be("Conservador");
    }

    [Fact]
    public void ConservadorPadrao_ShouldBeConservadorWithZeroScore()
    {
        // Act
        var perfil = PerfilRisco.ConservadorPadrao;

        // Assert
        perfil.Pontuacao.Should().Be(0);
        perfil.Perfil.Should().Be("Conservador");
    }
}
