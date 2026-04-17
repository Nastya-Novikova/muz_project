using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Application.Features.Notifications.DTOs;

namespace MusicianFinder.Application.Features.Notifications.GetNotifications
{
    /// <summary>
    /// Обработчик запроса <see cref="GetNotificationsQuery"/>.
    /// </summary>
    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, PagedResult<NotificationDto>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetNotificationsQueryHandler"/>.
        /// </summary>
        /// <param name="notificationRepository">Репозиторий уведомлений.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public GetNotificationsQueryHandler(
            INotificationRepository notificationRepository,
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<PagedResult<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль не найден.");

            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var (items, totalCount) = await _notificationRepository.GetByProfileIdAsync(profile.Id, request.Page, request.Limit, thirtyDaysAgo);

            var dtos = _mapper.Map<List<NotificationDto>>(items);

            return new PagedResult<NotificationDto>
            {
                Items = dtos,
                Total = totalCount,
                Page = request.Page,
                Limit = request.Limit
            };
        }
    }
}