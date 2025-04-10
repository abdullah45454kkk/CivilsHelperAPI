using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Emergencies
{
    public class EmergAnotherDTO
    {
        public int Id { get; set; }
        public DateTime SendAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
        public string TitleType { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
