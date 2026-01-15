using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Gestion_bibliot.DAL.Interfaces;
using Gestion_bibliot.DAL.Repositories;
using Gestion_bibliot.Models;
using Microsoft.AspNet.Identity;

namespace Gestion_bibliot.Controllers
{
    [Authorize(Roles = "Librarian")]
    public class LibrarianController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LibrarianController()
        {
            _unitOfWork = new UnitOfWork();
        }

        public LibrarianController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Librarian/PendingLoans
        public ActionResult PendingLoans()
        {
            var loans = _unitOfWork.Repository<Loan>().GetAll()
                .Where(l => l.Status == "Pending")
                .ToList();

            foreach (var loan in loans)
            {
                loan.Copy = _unitOfWork.Repository<Copy>().GetById(loan.CopyId);
                if (loan.Copy != null)
                {
                    loan.Copy.Book = _unitOfWork.Repository<Book>().GetById(loan.Copy.BookId);
                    if (loan.Copy.Book != null)
                    {
                        loan.Copy.Book.Author = _unitOfWork.Repository<Author>().GetById(loan.Copy.Book.AuthorId);
                    }
                }
            }

            // Préparer un dictionnaire Id -> FullName pour tous les étudiants concernés
            using (var db = new ApplicationDbContext())
            {
                var userIds = loans.Select(l => l.UserId).Distinct().ToList();
                var users = db.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => new { u.Id, u.FullName })
                    .ToList();

                var dict = users.ToDictionary(u => u.Id, u => u.FullName);
                ViewBag.StudentNames = dict;
            }

            return View(loans);
        }

        // GET: Librarian/LoanDetails/5
        public ActionResult LoanDetails(int id)
        {
            var loan = _unitOfWork.Repository<Loan>().GetById(id);
            if (loan == null)
            {
                return HttpNotFound();
            }

            loan.Copy = _unitOfWork.Repository<Copy>().GetById(loan.CopyId);
            if (loan.Copy != null)
            {
                loan.Copy.Book = _unitOfWork.Repository<Book>().GetById(loan.Copy.BookId);
                if (loan.Copy.Book != null)
                {
                    loan.Copy.Book.Author = _unitOfWork.Repository<Author>().GetById(loan.Copy.Book.AuthorId);
                }
            }

            // Charger aussi le nom complet de l'étudiant pour l'affichage des détails
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id == loan.UserId);
                ViewBag.StudentName = user != null ? user.FullName : loan.UserId;
            }

            return View(loan);
        }

        // POST: Librarian/ApproveLoan/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveLoan(int id)
        {
            var loan = _unitOfWork.Repository<Loan>().GetById(id);
            if (loan == null)
            {
                return HttpNotFound();
            }

            if (loan.Status != "Pending")
            {
                TempData["Error"] = "Cette demande n'est plus en attente.";
                return RedirectToAction("PendingLoans");
            }

            var originalCopy = _unitOfWork.Repository<Copy>().GetById(loan.CopyId);
            if (originalCopy == null)
            {
                TempData["Error"] = "Exemplaire introuvable.";
                return RedirectToAction("PendingLoans");
            }

            var bookId = originalCopy.BookId;

            var availableCopies = _unitOfWork.Repository<Copy>().GetAll()
                .Where(c => c.BookId == bookId && c.IsAvailable)
                .ToList();

            if (!availableCopies.Any())
            {
                TempData["Error"] = "Aucune copie disponible pour ce livre.";
                loan.Status = "Rejected";
                _unitOfWork.Repository<Loan>().Update(loan);
                _unitOfWork.Complete();
                return RedirectToAction("PendingLoans");
            }

            var random = new Random();
            var randomCopy = availableCopies[random.Next(availableCopies.Count)];

            loan.CopyId = randomCopy.Id;
            loan.LoanDate = DateTime.Now;
            loan.DueDate = DateTime.Now.AddDays(14);
            loan.Status = "Approved";

            randomCopy.IsAvailable = false;

            _unitOfWork.Repository<Loan>().Update(loan);
            _unitOfWork.Repository<Copy>().Update(randomCopy);
            _unitOfWork.Complete();

            TempData["Message"] = "Demande acceptée et exemplaire affecté.";
            return RedirectToAction("PendingLoans");
        }

        // POST: Librarian/RejectLoan/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectLoan(int id)
        {
            var loan = _unitOfWork.Repository<Loan>().GetById(id);
            if (loan == null)
            {
                return HttpNotFound();
            }

            if (loan.Status != "Pending")
            {
                TempData["Error"] = "Cette demande n'est plus en attente.";
                return RedirectToAction("PendingLoans");
            }

            loan.Status = "Rejected";
            _unitOfWork.Repository<Loan>().Update(loan);
            _unitOfWork.Complete();

            TempData["Message"] = "Demande refusée.";
            return RedirectToAction("PendingLoans");
        }

        // GET: Librarian/Loans
        public ActionResult Loans()
        {
            using (var db = new ApplicationDbContext())
            {
                var loans = (from l in db.Loans
                             join u in db.Users on l.UserId equals u.Id
                             join c in db.Copies on l.CopyId equals c.Id
                             join b in db.Books on c.BookId equals b.Id
                             join a in db.Authors on b.AuthorId equals a.Id
                             select new LoanViewModel
                             {
                                 Id = l.Id,
                                 StudentId = l.UserId,
                                 StudentName = u.FullName,
                                 CopyId = c.Id,
                                 BookTitle = b.Title,
                                 AuthorFullName = a.FirstName + " " + a.LastName,
                                 LoanDate = l.LoanDate,
                                 DueDate = l.DueDate,
                                 ReturnDate = l.ReturnDate,
                                 Status = l.Status
                             }).ToList();

                return View(loans);
            }
        }

        // GET: Librarian/EditLoan/5
        public ActionResult EditLoan(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var loan = db.Loans.Find(id);
                if (loan == null)
                {
                    return HttpNotFound();
                }

                var user = db.Users.FirstOrDefault(u => u.Id == loan.UserId);
                var copy = db.Copies.Find(loan.CopyId);
                Book book = null;
                Author author = null;
                if (copy != null)
                {
                    book = db.Books.Find(copy.BookId);
                    if (book != null)
                    {
                        author = db.Authors.Find(book.AuthorId);
                    }
                }

                var vm = new LoanViewModel
                {
                    Id = loan.Id,
                    StudentId = loan.UserId,
                    StudentName = user != null ? user.FullName : loan.UserId,
                    CopyId = loan.CopyId,
                    BookTitle = book != null ? book.Title : string.Empty,
                    AuthorFullName = author != null ? author.FirstName + " " + author.LastName : string.Empty,
                    LoanDate = loan.LoanDate,
                    DueDate = loan.DueDate,
                    ReturnDate = loan.ReturnDate,
                    Status = loan.Status
                };

                return View(vm);
            }
        }

        // POST: Librarian/EditLoan/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLoan(LoanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var db = new ApplicationDbContext())
            {
                var loan = db.Loans.Find(model.Id);
                if (loan == null)
                {
                    return HttpNotFound();
                }

                // ne permettre la modification que de la date de retour réelle
                loan.ReturnDate = model.ReturnDate;

                db.SaveChanges();
            }

            return RedirectToAction("Loans");
        }

        // POST: Librarian/DeleteLoan/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLoan(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var loan = db.Loans.Find(id);
                if (loan == null)
                {
                    return HttpNotFound();
                }

                // If the loan is approved and the copy is still not returned, free the copy
                if (loan.Status == "Approved" && !loan.ReturnDate.HasValue)
                {
                    var copy = db.Copies.Find(loan.CopyId);
                    if (copy != null)
                    {
                        copy.IsAvailable = true;
                    }
                }

                db.Loans.Remove(loan);
                db.SaveChanges();
            }

            return RedirectToAction("Loans");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}