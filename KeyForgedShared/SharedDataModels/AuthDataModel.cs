using KeyForgedShared.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyForgedShared.SharedDataModels
{
    public class AuthDataModel: IEntity
    {

        [Key]
        public Guid AuthKey { get; set; }

        public Guid Id => AuthKey;

        [ForeignKey("AccountId")]
        public Guid AccountId { get; set; }

        public string ShortLivedKey { get; set; }

        public string? LongLivedKey { get; set; }

        public AccountDataModel Account { get; set; }

    }
}
