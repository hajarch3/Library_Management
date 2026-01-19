using System;
using System.Linq;
using System.Web.Mvc;
using Gestion_bibliot.DAL.Interfaces;
using Gestion_bibliot.DAL.Repositories;
using Gestion_bibliot.Models;
using Microsoft.AspNet.Identity;

namespace Gestion_bibliot.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentController()
        {
            _unitOfWork = new UnitOfWork();
        }

        public StudentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        
        public ActionResult Books()
        {
            var books = _unitOfWork.Repository<Book>().GetAll().ToList();

            foreach (var book in books)
            {
                book.Author = _unitOfWork.Repository<Author>().GetById(book.AuthorId);
            }

            return View(books);
        }

     
        public ActionResult BookDetails(int id)
        {
            var book = _unitOfWork.Repository<Book>().GetById(id);
            if (book == null)
                return HttpNotFound();

            book.Author = _unitOfWork.Repository<Author>().GetById(book.AuthorId);
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestLoan(int bookId)
        {
            var userId = User.Identity.GetUserId();

          
            var subscription = _unitOfWork.Repository<Subscription>()
                .GetAll()
                .FirstOrDefault(s =>
                    s.UserId == userId &&
                    s.IsActive &&
                    s.StartDate <= DateTime.Today &&
                    s.EndDate >= DateTime.Today
                );

            if (subscription == null)
            {
                TempData["Error"] = "Vous n'avez aucun abonnement actif.";
                return RedirectToAction("Books");
            }

            int activeLoansCount = _unitOfWork.Repository<Loan>()
                .GetAll()
                .Count(l =>
                    l.UserId == userId &&
                    (l.Status == "Pending" || l.Status == "Approved") &&
                    l.ReturnDate == null
                );

            if (activeLoansCount >= subscription.MaxLoans)
            {
                TempData["Error"] = $"Limite atteinte ({subscription.MaxLoans} emprunts maximum).";
                return RedirectToAction("Books");
            }

            var copy = _unitOfWork.Repository<Copy>()
                .GetAll()
                .FirstOrDefault(c => c.BookId == bookId && c.IsAvailable);

            if (copy == null)
            {
                TempData["Error"] = "Aucun exemplaire disponible.";
                return RedirectToAction("Books");
            }

            var loan = new Loan
            {
                UserId = userId,
                CopyId = copy.Id,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                Status = "Pending"
            };

           
            copy.IsAvailable = false;
            _unitOfWork.Repository<Copy>().Update(copy);

            _unitOfWork.Repository<Loan>().Add(loan);
            _unitOfWork.Complete();

            TempData["Message"] = "Demande envoyée au bibliothécaire.";
            return RedirectToAction("Books");
        }


        
        public ActionResult MyLoans()
        {
            var userId = User.Identity.GetUserId();

            var loans = _unitOfWork.Repository<Loan>()
                .GetAll()
                .Where(l => l.UserId == userId)
                .ToList();

            foreach (var loan in loans)
            {
                loan.Copy = _unitOfWork.Repository<Copy>().GetById(loan.CopyId);

                if (loan.Copy != null)
                {
                    loan.Copy.Book =
                        _unitOfWork.Repository<Book>().GetById(loan.Copy.BookId);

                    if (loan.Copy.Book != null)
                    {
                        loan.Copy.Book.Author =
                            _unitOfWork.Repository<Author>()
                            .GetById(loan.Copy.Book.AuthorId);
                    }
                }
            }

            return View(loans);
        }

       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _unitOfWork.Dispose();

            base.Dispose(disposing);
        }
    }
}