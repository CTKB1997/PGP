using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProductAPI_Server.Models
{
    public class TK
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Password { get; set; }
    }
}