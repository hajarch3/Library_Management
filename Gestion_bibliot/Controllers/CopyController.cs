using System.Linq;
using System.Web.Mvc;
using Gestion_bibliot.DAL.Interfaces;
using Gestion_bibliot.DAL.Repositories;
using Gestion_bibliot.Models;

namespace Gestion_bibliot.Controllers
{
    public class CopyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CopyController()
        {
            _unitOfWork = new UnitOfWork();
        }

        public CopyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Copy
        public ActionResult Index()
        {
            var copies = _unitOfWork.Repository<Copy>().GetAll().ToList();

            // Load related books for display (title, image)
            foreach (var copy in copies)
            {
                copy.Book = _unitOfWork.Repository<Book>().GetById(copy.BookId);
            }

            return View(copies);
        }

        // GET: Copy/Details/5
        public ActionResult Details(int id)
        {
            var copy = _unitOfWork.Repository<Copy>().GetById(id);
            if (copy == null)
            {
                return HttpNotFound();
            }

            copy.Book = _unitOfWork.Repository<Book>().GetById(copy.BookId);
            return View(copy);
        }

        // GET: Copy/Create
        public ActionResult Create()
        {
            LoadBooksSelectList();
            return View();
        }

        // POST: Copy/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Copy copy)
        {
            if (ModelState.IsValid)
            {
                // By default a new copy is available unless specified otherwise
                _unitOfWork.Repository<Copy>().Add(copy);
                _unitOfWork.Complete();
                return RedirectToAction("Index", "Book");
            }

            LoadBooksSelectList(copy.BookId);
            return View(copy);
        }

        // GET: Copy/Edit/5
        public ActionResult Edit(int id)
        {
            var copy = _unitOfWork.Repository<Copy>().GetById(id);
            if (copy == null)
            {
                return HttpNotFound();
            }

            LoadBooksSelectList(copy.BookId);
            return View(copy);
        }

        // POST: Copy/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Copy copy)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Repository<Copy>().Update(copy);
                _unitOfWork.Complete();
                return RedirectToAction("Index");
            }

            LoadBooksSelectList(copy.BookId);
            return View(copy);
        }

        // GET: Copy/Delete/5
        public ActionResult Delete(int id)
        {
            var copy = _unitOfWork.Repository<Copy>().GetById(id);
            if (copy == null)
            {
                return HttpNotFound();
            }

            copy.Book = _unitOfWork.Repository<Book>().GetById(copy.BookId);
            return View(copy);
        }

        // POST: Copy/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Copy copy)
        {
            _unitOfWork.Repository<Copy>().Remove(copy.Id);
            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }

        private void LoadBooksSelectList(object selectedValue = null)
        {
            var books = _unitOfWork.Repository<Book>().GetAll()
                .Select(b => new { Id = b.Id, Title = b.Title });
            ViewBag.BookId = new SelectList(books, "Id", "Title", selectedValue);
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
