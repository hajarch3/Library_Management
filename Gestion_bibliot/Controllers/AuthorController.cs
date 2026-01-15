using System.Linq;
using System.Web.Mvc;
using Gestion_bibliot.DAL.Interfaces;
using Gestion_bibliot.DAL.Repositories;
using Gestion_bibliot.Models;

namespace Gestion_bibliot.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorController()
        {
            _unitOfWork = new UnitOfWork();
        }

        public AuthorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Author
        public ActionResult Index()
        {
            var authors = _unitOfWork.Repository<Author>().GetAll();
            return View(authors);
        }

        // GET: Author/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Author/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Author author)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Repository<Author>().Add(author);
                _unitOfWork.Complete();
                return RedirectToAction("Index");
            }

            return View(author);
        }

        // GET: Author/Details/5
        public ActionResult Details(int id)
        {
            var author = _unitOfWork.Repository<Author>().GetById(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // GET: Author/Edit/5
        public ActionResult Edit(int id)
        {
            var author = _unitOfWork.Repository<Author>().GetById(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: Author/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Author author)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Repository<Author>().Update(author);
                _unitOfWork.Complete();
                return RedirectToAction("Index");
            }
            return View(author);
        }

        // GET: Author/Delete/5
        public ActionResult Delete(int id)
        {
            var author = _unitOfWork.Repository<Author>().GetById(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: Author/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _unitOfWork.Repository<Author>().Remove(id);
            _unitOfWork.Complete();
            return RedirectToAction("Index");
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
