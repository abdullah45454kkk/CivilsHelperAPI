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
    public class Donation
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime DonatedAt { get; set; }
        public string UserId { get; set; }
        public LocalUser LocalUser { get; set; }
        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }

    }
}


