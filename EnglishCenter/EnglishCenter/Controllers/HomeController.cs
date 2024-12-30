using EnglishCenter.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EnglishCenter.Controllers
{
    public class HomeController : Controller
    {
        EnglishCenterDBEntities db = new EnglishCenterDBEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Username, string Password)
        {
            if (ModelState.IsValid)
            {
                var data = db.Students.Where(s => s.username.Equals(Username) && s.password.Equals(Password)).ToList();
                if (data.Count() > 0)
                {
                    Session["id"] = data.FirstOrDefault().id;
                    Session["name"] = data.FirstOrDefault().name;
                    Session["englishName"] = data.FirstOrDefault().englishName;
                    Session["birthday"] = data.FirstOrDefault().birthday;
                    Session["telephone"] = data.FirstOrDefault().telephone;
                    Session["gender"] = data.FirstOrDefault().gender;
                    Session["contactPerson"] = data.FirstOrDefault().contactPerson;
                    Session["username"] = data.FirstOrDefault().username;
                    Session["password"] = data.FirstOrDefault().password;
                    Session["courseID"] = data.FirstOrDefault().courseID;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ThongBao = "Đăng Nhập Không Thành Công";
                }
            }
            return View("Login");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Courses()
        {
            return View();
        }

        public ActionResult Course_Menu()
        {
            return View();
        }

        public ActionResult Attendance()
        {
            return View();
        }
        public ActionResult Schedule()
        {
            return View();
        }
        public ActionResult Homework()
        {
            return View();
        }
        public ActionResult Grade()
        {
            return View();
        }

        public ActionResult Information()
        {
            return View();
        }

        public ActionResult Support()
        {
            return View();
        }
    }
}