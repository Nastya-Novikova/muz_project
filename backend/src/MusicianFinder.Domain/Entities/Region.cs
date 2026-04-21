using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Справочник регионов.
    /// </summary>
    public class Region
    {
        private Region() { }

        public Region(string name, string localizedName)
        {
            Name = name;
            LocalizedName = localizedName;
        }

        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Английское название региона.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Русское название региона.
        /// </summary>
        public string LocalizedName { get; private set; }
    }
}
