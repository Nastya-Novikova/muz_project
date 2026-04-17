using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Application.Features.Metadata.DTOs
{
    /// <summary>
    /// DTO элемента справочника.
    /// </summary>
    public class LookupItemDto
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название на английском.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Локализованное название.
        /// </summary>
        public string LocalizedName { get; set; } = string.Empty;
    }
}