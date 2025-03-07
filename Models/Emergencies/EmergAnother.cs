using Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Emergencies
{
    public class EmergAnother
    {
        public int Id { get; set; }
        public DateTime SendAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
        public string TitleType { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public LocalUser LocalUser { get; set; }

        //public int EmergencyId { get; set; }

        //[ForeignKey("EmergencyId")]
        //public EmergencyBase EmergencyBase { get; set; }

    }
}
