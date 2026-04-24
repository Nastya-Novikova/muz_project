namespace MusicianFinder.Application.DTOs.Events
{
    /// <summary>
    /// Фильтр мероприятий.
    /// </summary>
    public class EventFilterDto
    {
        /// <summary>Поисковый запрос.</summary>
        public string? Query { get; set; }
        /// <summary>Идентификатор региона.</summary>
        public Guid? RegionId { get; set; }
        /// <summary>Идентификатор города.</summary>
        public Guid? CityId { get; set; }
        /// <summary>Дата начала (с).</summary>
        public DateTime? FromDate { get; set; }
        /// <summary>Дата начала (по).</summary>
        public DateTime? ToDate { get; set; }
        /// <summary>Статус.</summary>
        public string? Status { get; set; }
        /// <summary>Идентификатор создателя.</summary>
        public Guid? CreatorProfileId { get; set; }
        /// <summary>Номер страницы.</summary>
        public int Page { get; set; } = 1;
        /// <summary>Размер страницы.</summary>
        public int Limit { get; set; } = 20;
        /// <summary>Поле сортировки.</summary>
        public string? SortBy { get; set; }
        /// <summary>Сортировка по убыванию.</summary>
        public bool SortDesc { get; set; } = true;
    }
}