using Gestion_bibliot.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Book
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [Required]
    [StringLength(20)]
    public string ISBN { get; set; }

    [Required]
    public int AuthorId { get; set; }

    [ForeignKey("AuthorId")]
    public virtual Author Author { get; set; }

    [Required]
    [StringLength(50)]
    public string Category { get; set; }

    // 👇 Image
    [StringLength(255)]
    public string ImagePath { get; set; }
}
