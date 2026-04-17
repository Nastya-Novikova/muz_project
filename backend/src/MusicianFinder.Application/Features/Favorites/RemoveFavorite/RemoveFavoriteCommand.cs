using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Favorites.RemoveFavorite
{
    /// <summary>
    /// Команда для удаления профиля из избранного.
    /// </summary>
    public class RemoveFavoriteCommand : IRequest
    {
        /// <summary>
        /// Идентификатор профиля.
        /// </summary>
        public Guid ProfileId { get; set; }
    }
}