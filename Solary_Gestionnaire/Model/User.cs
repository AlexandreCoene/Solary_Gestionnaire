using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solary_Gestionnaire.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string StatusCompte { get; set; }
        public DateTime DateCreation { get; set; }
        public string PasswordHash { get; set; }
        public DateTime? DernierLogin { get; set; }
        public string OtpCode { get; set; }
        public int CompteVerifie { get; set; }
        public DateTime? OtpCreatedAt { get; set; }
    }
}

