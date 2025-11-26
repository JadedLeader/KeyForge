
using KeyForgedShared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KeyForgedShared.SharedDataModels
{
    public class AccountDataModel : IEntity
    {

        [Key]
        public Guid AccountId { get; set; }

        public Guid Id => AccountId;

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
