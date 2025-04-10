using Microsoft.AspNetCore.Identity;
using Models.Donations;
using Models.Emergencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Emergencies.EmergPerson;
using static Models.EnumsClass;

namespace Models.User
{
    public class LocalUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender gender { get; set; }
        public string Phone { get; set; } 
        public string Address { get; set; }
        public string SSN { get; set; }
        public string? ImageProfile { get; set; }

        // Removed Role field; use Identity's role system instead
        public ICollection<EmergPerson> EmergencyPersons { get; set; } = new List<EmergPerson>();
        public ICollection<EmergAnother> EmergencyAnothers { get; set; } = new List<EmergAnother>();
        public ICollection<Donation> Donations { get; set; } = new List<Donation>();

    }
}
