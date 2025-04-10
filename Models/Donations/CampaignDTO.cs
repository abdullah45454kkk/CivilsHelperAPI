using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.EnumsClass;

namespace Models.Donations
{
    public class CampaignDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CollectedAmount { get; set; }

    }
}
