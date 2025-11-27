using KeyForgedShared.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyForgedShared.SharedDataModels
{
    public class AuthDataModel: IEntity
    {

        [Key]
        [Column("AuthKey")]
        public Guid Id { get; set; }

        [ForeignKey("AccountId")]
        public Guid AccountId { get; set; }

        public string ShortLivedKey { get; set; }

        public string? LongLivedKey { get; set; }

        public AccountDataModel Account { get; set; }

    }
}
