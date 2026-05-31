using System.Threading;
using System.Threading.Tasks;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Tests.Shared.Mocks
{
    /// <summary>
    /// Заглушка <see cref="IVerificationCodeService"/>, возвращающая фиксированный код "111111".
    /// </summary>
    public class MockVerificationCodeService : IVerificationCodeService
    {
        public Task<string> GenerateAndSaveCodeAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult("111111");

        public Task<bool> ValidateCodeAsync(string email, string code, CancellationToken cancellationToken = default)
            => Task.FromResult(code == "111111");
    }
}