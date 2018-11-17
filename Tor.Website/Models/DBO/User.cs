using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Tor.Website.Models.DBO
{
    [DataContract]
    public class User
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [IgnoreDataMember]
        [Required]
        [StringLength(256)]
        public string Login { get; set; }

        [IgnoreDataMember]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [IgnoreDataMember]
        public UserToken UserToken { get; set; }

    }
}
