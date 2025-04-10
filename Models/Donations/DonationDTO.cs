using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.EnumsClass;

namespace Models.Donations
{
    public class DonationDTO
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime DonatedAt { get; set; }
        public string UserId { get; set; }
        public string DonorName { get; set; }
        public int CampaignId { get; set; }
    }
}
