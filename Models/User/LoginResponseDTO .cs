﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.User
{
    public class LoginResponseDTO
    {
        public string UserName { get; set; }

        public string Token { get; set; }
    }
}
