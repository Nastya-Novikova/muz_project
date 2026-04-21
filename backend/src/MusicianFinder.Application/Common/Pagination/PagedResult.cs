using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Application.Common.Pagination
{
    /// <summary>
    /// Представляет результат пагинации списка элементов.
    /// </summary>
    /// <typeparam name="T">Тип элементов списка.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Элементы текущей страницы.
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// Общее количество элементов.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Номер текущей страницы.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Размер страницы.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Общее количество страниц.
        /// </summary>
        public int TotalPages => (Total + Limit - 1) / Limit;
    }
}