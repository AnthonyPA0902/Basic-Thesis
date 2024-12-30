using EnglishCenter.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnglishCenter.Controllers
{
    public class TeacherController : Controller
    {
        EnglishCenterDBEntities db = new EnglishCenterDBEntities();

        // GET: Teacher
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        private void SetTeacherSessionVariables(Teacher teacher)
        {
            Session["id"] = teacher.id;
            Session["name"] = teacher.name;
            Session["telephone"] = teacher.telephone;
            Session["gender"] = teacher.gender;
            Session["role"] = teacher.role;
            Session["username"] = teacher.username;
            Session["password"] = teacher.password;
        }

        private void SetTASessionVariables(TA ta)
        {
            Session["ta_id"] = ta.id;
            Session["ta_name"] = ta.name;
            Session["ta_telephone"] = ta.telephone;
            Session["ta_gender"] = ta.gender;
            Session["ta_username"] = ta.username;
            Session["ta_password"] = ta.password;
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Username, string Password)
        {
            if (ModelState.IsValid)
            {
                var teacherData = db.Teachers.Where(s => s.username.Equals(Username) && s.password.Equals(Password)).ToList();
                var taData = db.TAs.Where(s => s.username.Equals(Username) && s.password.Equals(Password)).ToList();

                if (teacherData.Count() > 0 || taData.Count() > 0)
                {
                    if (teacherData.Count() > 0)
                    {
                        SetTeacherSessionVariables(teacherData.FirstOrDefault());
                    }
                    else
                    {
                        SetTASessionVariables(taData.FirstOrDefault());
                   
                    }
                    return RedirectToAction("Index", "Teacher");
                }
                else
                {
                    ModelState.AddModelError("", "Username or password is incorrect");
                }
            }
            return View("Login");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Teacher");
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

        public ActionResult CreateAttend()
        {
            var courses = db.Courses.ToList();
            var students = db.Students.ToList();
            ViewBag.CourseList = new SelectList(courses, "id", "name");
            ViewBag.StudentList = new SelectList(students, "id", "englishName");
            ViewBag.AttendOptions = new SelectList(new[]
            {
                 new SelectListItem { Value = "true", Text = "YES" },
                 new SelectListItem { Value = "false", Text = "NO" }
            }, "Value", "Text");

            return View();
        }
        [HttpPost, ActionName("CreateAttend")]
        [ValidateInput(false)]
        public ActionResult CreateAttend(Attendance diemdanh, int courseId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = db.Attendances.FirstOrDefault(s => s.id == diemdanh.id);
                    if (check == null)
                    {
                        db.Attendances.Add(diemdanh);
                        db.SaveChanges();
                        return RedirectToAction("Attendance", new { courseId = courseId });
                    }
                    else
                    {
                        ViewBag.ThongBao = "Mã Khóa Học Đã Tồn Tại";
                    }
                }

            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Error Save Data");
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditAttend()
        {
            var courses = db.Courses.ToList();
            var students = db.Students.ToList();
            ViewBag.CourseList = new SelectList(courses, "id", "name");
            ViewBag.StudentList = new SelectList(students, "id", "englishName");
            ViewBag.AttendOptions = new SelectList(new[]
            {
                 new SelectListItem { Value = "true", Text = "YES" },
                 new SelectListItem { Value = "false", Text = "NO" }
            }, "Value", "Text");
            return View();
        }

        [HttpPost, ActionName("EditAttend")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditAttend(int id, int courseId)
        {
            var attendUpdate = db.Attendances.Find(id);
            if (TryUpdateModel(attendUpdate, "", new string[] { "lesson", "date", "attend", "studentId", "courseId" }))
            {
                try
                {
                    db.Entry(attendUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }
            }

            return RedirectToAction("Attendance", new { courseId = courseId });
        }

        [HttpGet]
        public ActionResult DeleteAttend(int id)
        {
            Attendance AT = db.Attendances.SingleOrDefault(n => n.id == id);
            return View(AT);
        }

        [HttpPost, ActionName("DeleteAttend")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAttendConfirmed(int id, int courseId)
        {
            Attendance AT = db.Attendances.SingleOrDefault(n => n.id == id);
            db.Attendances.Remove(AT);
            db.SaveChanges();

            return RedirectToAction("Attendance", new { courseId = courseId });
        }



        public ActionResult Schedule()
        {
            return View();
        }

        public ActionResult Homework()
        {
            return View();
        }

        public ActionResult CreateHomework()
        {
            var courses = db.Courses.ToList();
            var students = db.Students.ToList();
            ViewBag.CourseList = new SelectList(courses, "id", "name");
            ViewBag.StudentList = new SelectList(students, "id", "englishName");

            return View();
        }
        [HttpPost, ActionName("CreateHomework")]
        [ValidateInput(false)]
        public ActionResult CreateHomework(Homework baitap, int courseId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = db.Homework.FirstOrDefault(s => s.id == baitap.id);
                    if (check == null)
                    {
                        db.Homework.Add(baitap);
                        db.SaveChanges();
                        return RedirectToAction("Homework", new { courseId = courseId });
                    }
                    else
                    {
                        ViewBag.ThongBao = "Already Exists";
                    }
                }

            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Error Save Data");
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditHomework()
        {
            var courses = db.Courses.ToList();
            var students = db.Students.ToList();
            ViewBag.CourseList = new SelectList(courses, "id", "name");
            ViewBag.StudentList = new SelectList(students, "id", "englishName");
            return View();
        }

        [HttpPost, ActionName("EditHomework")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditHomework(int id, int courseId)
        {
            var homeworkUpdate = db.Homework.Find(id);
            if (TryUpdateModel(homeworkUpdate, "", new string[] { "date", "score", "studentId", "courseId" }))
            {
                try
                {
                    db.Entry(homeworkUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }
            }

            return RedirectToAction("Homework", new { courseId = courseId });
        }

        [HttpGet]
        public ActionResult DeleteHomework(int id)
        {
            Homework HW = db.Homework.SingleOrDefault(n => n.id == id);
            return View(HW);
        }
        [HttpPost, ActionName("DeleteHomework")]
        public ActionResult DeleteHomeworkConfirmed(int id, int courseId)
        {
            Homework HW = db.Homework.SingleOrDefault(n => n.id == id);
            db.Homework.Remove(HW);
            db.SaveChanges();
            return RedirectToAction("Homework", new { courseId = courseId });
        }

        public ActionResult Grade()
        {
            return View();
        }

        public ActionResult CreateGrade()
        {
            var courses = db.Courses.ToList();
            var students = db.Students.ToList();
            ViewBag.CourseList = new SelectList(courses, "id", "name");
            ViewBag.StudentList = new SelectList(students, "id", "englishName");

            return View();
        }
        [HttpPost, ActionName("CreateGrade")]
        [ValidateInput(false)]
        public ActionResult CreateGrade(Grade diem, int courseId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = db.Grades.FirstOrDefault(s => s.id == diem.id);
                    if (check == null)
                    {
                        db.Grades.Add(diem);
                        db.SaveChanges();
                        return RedirectToAction("Grade", new { courseId = courseId });
                    }
                    else
                    {
                        ViewBag.ThongBao = "Already Exists";
                    }
                }

            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Error Save Data");
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditGrade()
        {
            var courses = db.Courses.ToList();
            var students = db.Students.ToList();
            ViewBag.CourseList = new SelectList(courses, "id", "name");
            ViewBag.StudentList = new SelectList(students, "id", "englishName");
            return View();
        }

        [HttpPost, ActionName("EditGrade")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditGrade(int id, int courseId)
        {
            var GradeUpdate = db.Grades.Find(id);
            if (TryUpdateModel(GradeUpdate, "", new string[] { "attendance", "courseProject", "homework", "MMT", "EOMT", "total","courseId", "studentId" }))
            {
                try
                {
                    db.Entry(GradeUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }
            }

            return RedirectToAction("Grade", new { courseId = courseId });
        }

        [HttpGet]
        public ActionResult DeleteGrade(int id)
        {
            Grade GD = db.Grades.SingleOrDefault(n => n.id == id);
            return View(GD);
        }
        [HttpPost, ActionName("DeleteGrade")]
        public ActionResult DeleteGradeConfirmed(int id, int courseId)
        {
            Grade GD = db.Grades.SingleOrDefault(n => n.id == id);
            db.Grades.Remove(GD);
            db.SaveChanges();
            return RedirectToAction("Grade", new { courseId = courseId });
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