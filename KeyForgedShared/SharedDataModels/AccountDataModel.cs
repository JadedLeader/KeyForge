
using KeyForgedShared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyForgedShared.SharedDataModels
{
    public class AccountDataModel : IEntity
    {

        [Key]
        [Column("AccountId")]
        public Guid Id { get; set; }

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
