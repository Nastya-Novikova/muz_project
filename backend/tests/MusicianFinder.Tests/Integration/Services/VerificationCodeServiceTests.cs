using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Infrastructure.Persistence;
using MusicianFinder.Infrastructure.Services;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Services
{
    public class VerificationCodeServiceTests : TestBase, IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private AppDbContext _dbContext = null!;
        private VerificationCodeService _service = null!;

        public VerificationCodeServiceTests(DatabaseFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _dbContext = _fixture.CreateDbContext();
            _service = new VerificationCodeService(_dbContext);
        }

        public Task DisposeAsync()
        {
            _dbContext?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GenerateAndSaveCodeAsync_GeneratesSixDigitCode()
        {
            LogInfo("Test: Generate verification code");
            var code = await _service.GenerateAndSaveCodeAsync("gen@test.com");
            code.Should().HaveLength(6);
            code.Should().MatchRegex("^[0-9]{6}$");
        }

        [Fact]
        public async Task ValidateCodeAsync_ValidCode_ReturnsTrueAndMarksUsed()
        {
            LogInfo("Test: Validate valid code");
            var email = "validate@test.com";
            var code = await _service.GenerateAndSaveCodeAsync(email);
            var isValid = await _service.ValidateCodeAsync(email, code);
            isValid.Should().BeTrue();

            var saved = await _dbContext.Set<EmailVerificationCode>().FirstOrDefaultAsync(c => c.Email == email && c.Code == code);
            saved!.IsUsed.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateCodeAsync_InvalidCode_ReturnsFalse()
        {
            LogInfo("Test: Validate invalid code");
            var email = "invalid@test.com";
            await _service.GenerateAndSaveCodeAsync(email);
            var isValid = await _service.ValidateCodeAsync(email, "000000");
            isValid.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateCodeAsync_ExpiredCode_ReturnsFalse()
        {
            LogInfo("Test: Validate expired code");
            var email = "expired@test.com";
            var code = "123456";
            var expiredCode = new EmailVerificationCode(email, code);
            var property = expiredCode.GetType().GetProperty("CreatedAt");
            property?.SetValue(expiredCode, System.DateTime.UtcNow.AddMinutes(-11));
            _dbContext.Set<EmailVerificationCode>().Add(expiredCode);
            await _dbContext.SaveChangesAsync();

            var isValid = await _service.ValidateCodeAsync(email, code);
            isValid.Should().BeFalse();
        }
    }
}