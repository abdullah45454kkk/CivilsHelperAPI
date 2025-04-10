using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Emergencies.EmergPerson;

namespace Models.Emergencies
{
    public class EmergPersonDTO
    {
        public int Id { get; set; }
        public DateTime SendAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
        public string Name { get; set; }
        public string gender { get; set; }
        public int Age { get; set; }
        public string SSN { get; set; }
        public string LastSeenLocation { get; set; }
        public DateTime LastSeenDateTime { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

    }
}
