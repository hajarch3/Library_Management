using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gestion_bibliot.Models
{
    public class Penalty
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LoanId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [Range(0, 1000)]
        [Column(TypeName = "decimal")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(100)]
        public string Reason { get; set; }

        [Required]
        public bool IsPaid { get; set; }

        [DataType(DataType.Date)]
        public DateTime IssuedDate { get; set; }
    }

}