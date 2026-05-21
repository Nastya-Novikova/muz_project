using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace MusicianFinder.Tests.Shared.Extensions
{
    /// <summary>
    /// Кастомные проверки для часто используемых типов.
    /// </summary>
    public static class AssertionExtensions
    {
        /// <summary>Проверяет, что ProblemDetails имеет ожидаемый тип и заголовок.</summary>
        public static void ShouldBeProblemDetails(this ProblemDetails problem, string expectedType, string expectedTitle)
        {
            problem.Should().NotBeNull();
            problem.Type.Should().Be(expectedType);
            problem.Title.Should().Be(expectedTitle);
        }
    }
}