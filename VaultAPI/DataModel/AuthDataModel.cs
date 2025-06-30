
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaultAPI.DataModel
{
    public class AuthDataModel
    {

        [Key]
        public Guid AuthKey { get; set; }

        [ForeignKey("AccountId")]
        public Guid AccountId { get; set; }

        public string ShortLivedKey { get; set; }

        public string LongLivedKey { get; set; }

        public AccountDataModel Account { get; set; }

    }
}
