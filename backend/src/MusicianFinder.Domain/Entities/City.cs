using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Справочник городов.
    /// </summary>
    public class City
    {
        private City() { }

        public City(string name, string localizedName)
        {
            Name = name;
            LocalizedName = localizedName;
        }

        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Английское название города.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Русское название города.
        /// </summary>
        public string LocalizedName { get; private set; }
    }
}
