using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.ValueObjects;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Обработчик команды <see cref="UpdateProfileCommand"/>.
    /// </summary>
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Guid>
    {
        private readonly ICurrentProfileProvider _profileProvider;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileProvider">Репозиторий профилей.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public UpdateProfileCommandHandler(ICurrentProfileProvider profileProvider)
        {
            _profileProvider = profileProvider;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.FullName) || request.Age.HasValue ||
                !string.IsNullOrWhiteSpace(request.Description) || request.CityId.HasValue)
            {
                profile.UpdateCoreInfo(
                    request.FullName != null ? new ProfileName(request.FullName) : profile.FullName,
                    request.Age ?? profile.Age,
                    request.Description ?? profile.Description,
                    request.CityId ?? profile.CityId);
            }
            if (!string.IsNullOrWhiteSpace(request.Phone) || !string.IsNullOrWhiteSpace(request.Telegram))
            {
                profile.UpdateContacts(
                    request.Phone != null ? new PhoneNumber(request.Phone) : profile.Phone,
                    request.Telegram != null ? new TelegramHandle(request.Telegram) : profile.Telegram);
            }
            if (request.GenreIds != null)
                profile.SetGenres(request.GenreIds.Select(id => new GenreId(id)));
            if (request.SpecialtyIds != null)
                profile.SetSpecialties(request.SpecialtyIds.Select(id => new SpecialtyId(id)));
            if (request.CollaborationGoalIds != null)
                profile.SetCollaborationGoals(request.CollaborationGoalIds.Select(id => new CollaborationGoalId(id)));
            if (request.DesiredGenreIds != null)
                profile.SetDesiredGenres(request.DesiredGenreIds.Select(id => new GenreId(id)));
            if (request.DesiredSpecialtyIds != null)
                profile.SetDesiredSpecialties(request.DesiredSpecialtyIds.Select(id => new SpecialtyId(id)));
            if (request.Experience.HasValue)
                profile.SetExperience(request.Experience.Value);
            if (request.LookingFor.HasValue)
                profile.SetLookingFor(request.LookingFor.Value);
            if (request.ProfileType.HasValue)
                profile.SetProfileType(request.ProfileType.Value);

            return profile.Id;
        }
    }
}