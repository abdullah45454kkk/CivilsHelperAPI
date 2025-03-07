using Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Emergencies
{
    public class EmergPerson
    {
        public int Id { get; set; }
        public DateTime SendAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
        public string Name { get; set; }
        public int Age { get; set; }
        public string SSN { get; set; }
        public string LastSeenLocation { get; set; }
        public DateTime LastSeenDateTime { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public LocalUser LocalUser { get; set; }
        //public int EmergencyId { get; set; }

        //[ForeignKey("EmergencyId")]
        //public EmergencyBase EmergencyBase { get; set; }
    }
}
