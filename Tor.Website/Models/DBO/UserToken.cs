using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tor.Website.Models.DBO
{
    public class UserToken
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(256)]
        public string Value { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfExpire { get; set; }

        [Required]
        public User User { get; set; }
    }
}
