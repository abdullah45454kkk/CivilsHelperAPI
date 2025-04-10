using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Map
{
    public class CreateGeographicAreaDto
    {
        public string AreaName { get; set; }
        public GeoCoordinateDto NorthWest { get; set; }
        public GeoCoordinateDto SouthEast { get; set; }
        public ICollection<int> RelatedAreaIds { get; set; } = new List<int>();

    }
}
