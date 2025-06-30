
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaultAPI.DataModel
{
    public class VaultDataModel
    {

        [Key]

        public required Guid VaultId { get; set; }

        [ForeignKey("AccountId")]
        public required Guid AccountId { get; set; }

        public required string VaultName { get; set; }   

        public required DateTime VaultCreatedAt { get; set; }

        public required VaultType VaultType { get; set; }   

        public AccountDataModel Account { get; set; }


    }

    public enum VaultType
    {
        Team, 
        Personal
    }
}
