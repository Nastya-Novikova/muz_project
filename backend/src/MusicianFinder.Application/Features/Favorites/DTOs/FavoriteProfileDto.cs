using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Application.Features.Profiles.DTOs;

namespace MusicianFinder.Application.Features.Favorites.DTOs
{
    /// <summary>
    /// DTO избранного профиля.
    /// </summary>
    public class FavoriteProfileDto
    {
        /// <summary>
        /// Профиль.
        /// </summary>
        public ProfileDto Profile { get; set; } = new();

        /// <summary>
        /// Дата добавления.
        /// </summary>
        public DateTime AddedAt { get; set; }
    }
}