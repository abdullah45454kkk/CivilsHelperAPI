using Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.EnumsClass;

namespace Models.Donations
{
    public class Campaign
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CollectedAmount { get; set; } = 0;
        public ICollection<Donation> Donations { get; set; } = new List<Donation>();
    }
}


