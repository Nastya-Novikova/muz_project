using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Favorites.AddFavorite
{
    /// <summary>
    /// Команда для добавления профиля в избранное.
    /// </summary>
    public class AddFavoriteCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор профиля.
        /// </summary>
        public Guid ProfileId { get; set; }
    }
}