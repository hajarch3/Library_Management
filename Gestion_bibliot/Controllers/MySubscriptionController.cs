using Gestion_bibliot.DAL.Interfaces;
using Gestion_bibliot.DAL.Repositories;
using Gestion_bibliot.Models;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;

namespace Gestion_bibliot.Controllers
{
    [Authorize(Roles = "Student")]
    public class MySubscriptionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MySubscriptionController()
        {
            _unitOfWork = new UnitOfWork();
        }

        public MySubscriptionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            var subscription = _unitOfWork.Repository<Subscription>()
                .GetAll()
                .FirstOrDefault(s =>
                    s.UserId == userId &&
                    s.IsActive);

            return View(subscription);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}
