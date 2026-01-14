using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gestion_bibliot.Models
{
    public class Loan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // ASP.NET Identity

        [Required]
        public int CopyId { get; set; }

        [ForeignKey("CopyId")]
        public virtual Copy Copy { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime LoanDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }
    }
}