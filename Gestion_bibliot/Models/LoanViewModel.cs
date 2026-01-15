using System;
using System.ComponentModel.DataAnnotations;

namespace Gestion_bibliot.Models
{
    public class LoanViewModel
    {
        public int Id { get; set; }

        [Display(Name = "ID �tudiant")]
        public string StudentId { get; set; }

        [Display(Name = "Nom de l'�tudiant")]
        public string StudentName { get; set; }

        [Display(Name = "ID de la copie")]
        public int CopyId { get; set; }

        [Display(Name = "Titre du livre")]
        public string BookTitle { get; set; }

        [Display(Name = "Auteur")]
        public string AuthorFullName { get; set; }

        [Display(Name = "Date d'emprunt")]
        [DataType(DataType.Date)]
        public DateTime LoanDate { get; set; }

        [Display(Name = "Date de retour pr�vue")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Display(Name = "Date de retour r�elle")]
        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "Statut")]
        public string Status { get; set; }
    }
}
