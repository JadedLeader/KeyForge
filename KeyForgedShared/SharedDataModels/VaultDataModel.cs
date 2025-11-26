
using KeyForgedShared.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyForgedShared.SharedDataModels
{
    public class VaultDataModel: IEntity
    {

        [Key]

        public Guid VaultId { get; set; }

        public Guid Id => VaultId;

        [ForeignKey("AccountId")]
        public Guid AccountId { get; set; }

        public string VaultName { get; set; }   

        public DateTime VaultCreatedAt { get; set; }

        public VaultType VaultType { get; set; }   

        public AccountDataModel Account { get; set; }

        public ICollection<VaultKeysDataModel> VaultKeys { get; set; } = new List<VaultKeysDataModel>();


    }

    public enum VaultType
    {
        Team, 
        Personal
    }
}
