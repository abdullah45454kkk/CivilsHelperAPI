using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Emergencies.EmergPerson;
using static Models.EnumsClass;

namespace Models.User
{
    public class RegisterationRequestDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public Gender gender { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be 11 digits and contain only numbers.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Address must be between 10 and 200 characters.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "SSN is required.")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "SSN must be exactly 14 digits and contain only numbers.")]
        public string SSN { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
        public string? ImageProfile { get; set; }

    }
}
