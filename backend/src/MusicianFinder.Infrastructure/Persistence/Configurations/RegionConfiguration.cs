using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Конфигурация сущности <see cref="Region"/>.
    /// </summary>
    public class RegionConfiguration : IEntityTypeConfiguration<Region>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.ToTable("Region");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).ValueGeneratedOnAdd();
            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
            builder.Property(r => r.LocalizedName).IsRequired().HasMaxLength(100);

            builder.HasData(
                new Region(1, "Altai Krai", "Алтайский край"),
                new Region(2, "Amur Oblast", "Амурская область"),
                new Region(3, "Arkhangelsk Oblast", "Архангельская область"),
                new Region(4, "Astrakhan Oblast", "Астраханская область"),
                new Region(5, "Belgorod Oblast", "Белгородская область"),
                new Region(6, "Bryansk Oblast", "Брянская область"),
                new Region(7, "Vladimir Oblast", "Владимирская область"),
                new Region(8, "Volgograd Oblast", "Волгоградская область"),
                new Region(9, "Vologda Oblast", "Вологодская область"),
                new Region(10, "Voronezh Oblast", "Воронежская область"),
                new Region(11, "Donetsk People's Republic", "Донецкая Народная Республика"),
                new Region(12, "Jewish Autonomous Oblast", "Еврейская автономная область"),
                new Region(13, "Zabaykalsky Krai", "Забайкальский край"),
                new Region(14, "Zaporizhzhia Oblast", "Запорожская область"),
                new Region(15, "Ivanovo Oblast", "Ивановская область"),
                new Region(16, "Irkutsk Oblast", "Иркутская область"),
                new Region(17, "Kabardino-Balkar Republic", "Кабардино-Балкарская Республика"),
                new Region(18, "Kaliningrad Oblast", "Калининградская область"),
                new Region(19, "Kaluga Oblast", "Калужская область"),
                new Region(20, "Kamchatka Krai", "Камчатский край"),
                new Region(21, "Karachay-Cherkess Republic", "Карачаево-Черкесская Республика"),
                new Region(22, "Kemerovo Oblast", "Кемеровская область"),
                new Region(23, "Kirov Oblast", "Кировская область"),
                new Region(24, "Kostroma Oblast", "Костромская область"),
                new Region(25, "Krasnodar Krai", "Краснодарский край"),
                new Region(26, "Krasnoyarsk Krai", "Красноярский край"),
                new Region(27, "Kurgan Oblast", "Курганская область"),
                new Region(28, "Kursk Oblast", "Курская область"),
                new Region(29, "Leningrad Oblast", "Ленинградская область"),
                new Region(30, "Lipetsk Oblast", "Липецкая область"),
                new Region(31, "Luhansk People's Republic", "Луганская Народная Республика"),
                new Region(32, "Magadan Oblast", "Магаданская область"),
                new Region(33, "Moscow", "Москва"),
                new Region(34, "Moscow Oblast", "Московская область"),
                new Region(35, "Murmansk Oblast", "Мурманская область"),
                new Region(36, "Nenets Autonomous Okrug", "Ненецкий автономный округ"),
                new Region(37, "Nizhny Novgorod Oblast", "Нижегородская область"),
                new Region(38, "Novgorod Oblast", "Новгородская область"),
                new Region(39, "Novosibirsk Oblast", "Новосибирская область"),
                new Region(40, "Omsk Oblast", "Омская область"),
                new Region(41, "Orenburg Oblast", "Оренбургская область"),
                new Region(42, "Oryol Oblast", "Орловская область"),
                new Region(43, "Penza Oblast", "Пензенская область"),
                new Region(44, "Perm Krai", "Пермский край"),
                new Region(45, "Primorsky Krai", "Приморский край"),
                new Region(46, "Pskov Oblast", "Псковская область"),
                new Region(47, "Republic of Adygea", "Республика Адыгея"),
                new Region(48, "Altai Republic", "Республика Алтай"),
                new Region(49, "Republic of Bashkortostan", "Республика Башкортостан"),
                new Region(50, "Republic of Buryatia", "Республика Бурятия"),
                new Region(51, "Republic of Dagestan", "Республика Дагестан"),
                new Region(52, "Republic of Ingushetia", "Республика Ингушетия"),
                new Region(53, "Republic of Kalmykia", "Республика Калмыкия"),
                new Region(54, "Republic of Karelia", "Республика Карелия"),
                new Region(55, "Komi Republic", "Республика Коми"),
                new Region(56, "Republic of Crimea", "Республика Крым"),
                new Region(57, "Mari El Republic", "Республика Марий Эл"),
                new Region(58, "Republic of Mordovia", "Республика Мордовия"),
                new Region(59, "Sakha (Yakutia) Republic", "Республика Саха (Якутия)"),
                new Region(60, "Republic of North Ossetia–Alania", "Республика Северная Осетия — Алания"),
                new Region(61, "Tatarstan", "Республика Татарстан"),
                new Region(62, "Tuva Republic", "Республика Тыва"),
                new Region(63, "Republic of Khakassia", "Республика Хакасия"),
                new Region(64, "Rostov Oblast", "Ростовская область"),
                new Region(65, "Ryazan Oblast", "Рязанская область"),
                new Region(66, "Samara Oblast", "Самарская область"),
                new Region(67, "Saint Petersburg", "Санкт-Петербург"),
                new Region(68, "Saratov Oblast", "Саратовская область"),
                new Region(69, "Sakhalin Oblast", "Сахалинская область"),
                new Region(70, "Sverdlovsk Oblast", "Свердловская область"),
                new Region(71, "Sevastopol", "Севастополь"),
                new Region(72, "Smolensk Oblast", "Смоленская область"),
                new Region(73, "Stavropol Krai", "Ставропольский край"),
                new Region(74, "Tambov Oblast", "Тамбовская область"),
                new Region(75, "Tver Oblast", "Тверская область"),
                new Region(76, "Tomsk Oblast", "Томская область"),
                new Region(77, "Tula Oblast", "Тульская область"),
                new Region(78, "Tyumen Oblast", "Тюменская область"),
                new Region(79, "Udmurt Republic", "Удмуртская Республика"),
                new Region(80, "Ulyanovsk Oblast", "Ульяновская область"),
                new Region(81, "Khabarovsk Krai", "Хабаровский край"),
                new Region(82, "Khanty-Mansi Autonomous Okrug", "Ханты-Мансийский автономный округ — Югра"),
                new Region(83, "Kherson Oblast", "Херсонская область"),
                new Region(84, "Chelyabinsk Oblast", "Челябинская область"),
                new Region(85, "Chechen Republic", "Чеченская Республика"),
                new Region(86, "Chuvash Republic", "Чувашская Республика"),
                new Region(87, "Chukotka Autonomous Okrug", "Чукотский автономный округ"),
                new Region(88, "Yamalo-Nenets Autonomous Okrug", "Ямало-Ненецкий автономный округ"),
                new Region(89, "Yaroslavl Oblast", "Ярославская область")
            );
        }
    }
}