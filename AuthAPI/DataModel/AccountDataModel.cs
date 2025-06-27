using gRPCIntercommunicationService;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthAPI.DataModel
{
    public class AccountDataModel
    {
        [Key]
        public Guid AccountId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public AuthRoles AuthorisationLevel { get; set; }

        [Required]
        public DateTime AccountCreated { get; set; }


    }


    public enum AuthRoles
    {
        User, 
        Admin
    }
      
}
