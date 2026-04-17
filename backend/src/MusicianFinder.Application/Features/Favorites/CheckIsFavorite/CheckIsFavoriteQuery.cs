using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Favorites.CheckIsFavorite
{
    /// <summary>
    /// Запрос для проверки, находится ли профиль в избранном.
    /// </summary>
    public class CheckIsFavoriteQuery : IRequest<bool>
    {
        /// <summary>
        /// Идентификатор профиля.
        /// </summary>
        public Guid ProfileId { get; set; }
    }
}