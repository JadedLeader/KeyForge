
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaultKeysAPI.DataModel
{
    public class VaultDataModel
    {

        [Key]

        public Guid VaultId { get; set; }

        [ForeignKey("AccountId")]
        public Guid AccountId { get; set; }

        public string VaultName { get; set; }   

        public DateTime VaultCreatedAt { get; set; }

        public VaultType VaultType { get; set; }   

        public AccountDataModel Account { get; set; }


    }

    public enum VaultType
    {
        Team, 
        Personal
    }
}
