using Gestion_bibliot.DAL.Interfaces;
using Gestion_bibliot.DAL.Repositories;
using Gestion_bibliot.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Gestion_bibliot.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class SubscriptionsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionsController()
        {
            _unitOfWork = new UnitOfWork();
        }

        public SubscriptionsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Subscriptions
        public ActionResult Index()
        {
            var today = DateTime.Today;

            var subscriptions = _unitOfWork
                .Repository<Subscription>()
                .GetAll()
                .ToList();

            // Auto-expire subscriptions
            foreach (var sub in subscriptions)
            {
                if (sub.IsActive && sub.EndDate < today)
                {
                    sub.IsActive = false;
                }
            }

            _unitOfWork.Complete();

            // Load users manually (since no Include here)
            foreach (var sub in subscriptions)
            {
                sub.User = _unitOfWork.Repository<ApplicationUser>()
                    .GetById(sub.UserId);
            }

            return View(subscriptions);
        }

        // GET: Subscriptions/Create
        public ActionResult Create()
        {
            LoadStudents();
            return View();
        }

        // POST: Subscriptions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Subscription subscription)
        {
            if (!ModelState.IsValid)
            {
                LoadStudents(subscription.UserId);
                return View(subscription);
            }

            // Deactivate existing active subscriptions
            var activeSubs = _unitOfWork.Repository<Subscription>()
                .GetAll()
                .Where(s => s.UserId == subscription.UserId && s.IsActive)
                .ToList();

            foreach (var sub in activeSubs)
            {
                sub.IsActive = false;
            }

            subscription.IsActive = true;
            _unitOfWork.Repository<Subscription>().Add(subscription);
            _unitOfWork.Complete();

            TempData["SuccessMessage"] = "Subscription assigned successfully.";
            return RedirectToAction("Index");
        }

        // GET: Subscriptions/Edit/5
        public ActionResult Edit(int id)
        {
            var subscription = _unitOfWork.Repository<Subscription>().GetById(id);
            if (subscription == null)
                return HttpNotFound();

            LoadStudents(subscription.UserId);
            return View(subscription);
        }
        // GET: Subscriptions/Delete/5
        public ActionResult Delete(int id)
        {
            var subscription = _unitOfWork.Repository<Subscription>().GetById(id);
            if (subscription == null)
                return HttpNotFound();

            subscription.User = _unitOfWork.Repository<ApplicationUser>()
                .GetById(subscription.UserId);

            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var subscription = _unitOfWork.Repository<Subscription>().GetById(id);

            if (subscription == null)
                return HttpNotFound();

            _unitOfWork.Repository<Subscription>().Remove(id);
            _unitOfWork.Complete();

            return RedirectToAction("Index");
        }



        // POST: Subscriptions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Subscription model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var subscription = _unitOfWork.Repository<Subscription>().GetById(model.Id);

            if (subscription == null)
                return HttpNotFound();

            // Update fields manually
            subscription.StartDate = model.StartDate;
            subscription.EndDate = model.EndDate;
            subscription.MaxLoans = model.MaxLoans;
            subscription.IsActive = model.IsActive;
            subscription.UserId = model.UserId;

            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }


        private void LoadStudents(string selectedUserId = null)
        {
            var students = _unitOfWork.Repository<ApplicationUser>()
                .GetAll()
                .Where(u => u.Role == "Student")
                .ToList();

            ViewBag.Users = new SelectList(
                students,
                "Id",
                "FullName",
                selectedUserId
            );
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
