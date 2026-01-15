using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gestion_bibliot.DAL.Interfaces;
using Gestion_bibliot.DAL.Repositories;
using Gestion_bibliot.Models;

namespace Gestion_bibliot.Controllers
{
    public class BookController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookController()
        {
            _unitOfWork = new UnitOfWork();
        }

        public BookController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Book
        public ActionResult Index()
        {
            var books = _unitOfWork.Repository<Book>().GetAll();
            foreach (var book in books)
            {
                book.Author = _unitOfWork.Repository<Author>().GetById(book.AuthorId);
            }
            return View(books);
        }

        // GET: Book/Details/5
        public ActionResult Details(int id)
        {
            var book = _unitOfWork.Repository<Book>().GetById(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            book.Author = _unitOfWork.Repository<Author>().GetById(book.AuthorId);
            return View(book);
        }

        // GET: Book/Create
        public ActionResult Create()
        {
            LoadAuthorsSelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Book book, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var uploads = Server.MapPath("~/Content/Uploads");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                    var ext = Path.GetExtension(imageFile.FileName);
                    var fileName = Guid.NewGuid().ToString() + ext;
                    var path = Path.Combine(uploads, fileName);
                    imageFile.SaveAs(path);
                    book.ImagePath = Url.Content("~/Content/Uploads/" + fileName);
                }

                _unitOfWork.Repository<Book>().Add(book);
                _unitOfWork.Complete();
                return RedirectToAction("Index");
            }

            LoadAuthorsSelectList(book.AuthorId);
            return View(book);
        }

        // GET: Book/Edit/5
        public ActionResult Edit(int id)
        {
            var book = _unitOfWork.Repository<Book>().GetById(id);
            if (book == null)
            {
                return HttpNotFound();
            }

            LoadAuthorsSelectList(book.AuthorId);
            return View(book);
        }

        // POST: Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Book book, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var uploads = Server.MapPath("~/Content/Uploads");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                    var ext = Path.GetExtension(imageFile.FileName);
                    var fileName = Guid.NewGuid().ToString() + ext;
                    var path = Path.Combine(uploads, fileName);
                    imageFile.SaveAs(path);
                    book.ImagePath = Url.Content("~/Content/Uploads/" + fileName);
                }

                _unitOfWork.Repository<Book>().Update(book);
                _unitOfWork.Complete();
                return RedirectToAction("Index");
            }

            LoadAuthorsSelectList(book.AuthorId);
            return View(book);
        }

        // GET: Book/Delete/5
        public ActionResult Delete(int id)
        {
            var book = _unitOfWork.Repository<Book>().GetById(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            book.Author = _unitOfWork.Repository<Author>().GetById(book.AuthorId);
            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _unitOfWork.Repository<Book>().Remove(id);
            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }

        private void LoadAuthorsSelectList(object selectedValue = null)
        {
            var authors = _unitOfWork.Repository<Author>().GetAll()
                .Select(a => new { Id = a.Id, Name = a.FirstName + " " + a.LastName });
            ViewBag.AuthorId = new SelectList(authors, "Id", "Name", selectedValue);
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
