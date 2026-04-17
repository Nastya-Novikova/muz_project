using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Application.Features.VkIntegration.ConnectVk
{
    /// <summary>
    /// Обработчик команды <see cref="ConnectVkCommand"/>.
    /// </summary>
    public class ConnectVkCommandHandler : IRequestHandler<ConnectVkCommand>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IVkService _vkService;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ConnectVkCommandHandler"/>.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="vkService">Сервис VK.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public ConnectVkCommandHandler(
            IProfileRepository profileRepository,
            IVkService vkService,
            ICurrentUserService currentUserService)
        {
            _profileRepository = profileRepository;
            _vkService = vkService;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task Handle(ConnectVkCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль не найден.");

            if (!string.IsNullOrEmpty(profile.VkUserId))
                throw new ConflictException("Аккаунт VK уже привязан.");

            var vkUserId = await _vkService.ExchangeCodeAsync(request.Code, request.CodeVerifier, request.DeviceId);
            if (!vkUserId.HasValue)
                throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure(nameof(request.Code), "Не удалось получить идентификатор пользователя VK.") });

            profile.SetVkUserId(vkUserId.Value.ToString());
            await _profileRepository.UpdateAsync(profile);
        }
    }
}