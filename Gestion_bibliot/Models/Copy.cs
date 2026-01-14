using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gestion_bibliot.Models
{
    public class Copy
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

        [Required]
        public bool IsAvailable { get; set; }

        [StringLength(50)]
        public string ShelfLocation { get; set; }
    }
}