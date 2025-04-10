using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Map
{
    public class GeographicArea
    {
        public int Id { get; set; }

        [Required]
        public string AreaName { get; set; }

        public GeoCoordinate NorthWest { get; set; }

        public GeoCoordinate SouthEast { get; set; }

        // Navigation property for related areas if needed
        public virtual ICollection<GeographicArea> RelatedAreas { get; set; } = new List<GeographicArea>();
    }
}
