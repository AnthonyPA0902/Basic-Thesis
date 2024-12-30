using EnglishCenter.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace EnglishCenter.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        EnglishCenterDBEntities db = new EnglishCenterDBEntities();

        // GET: Admin/Admin
        public ActionResult Index()
        {
            if (Session["id"] != null) {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(String username, String password)
        {
            if (ModelState.IsValid)
            {
                var data = db.Managers.Where(s => s.username.Equals(username) && s.password.Equals(password)).ToList();
                if (data.Count() > 0)
                {
                    Session["id"] = data.FirstOrDefault().id;
                    Session["name"] = data.FirstOrDefault().name;
                    Session["telephone"] = data.FirstOrDefault().telephone;
                    Session["username"] = data.FirstOrDefault().username;
                    Session["password"] = data.FirstOrDefault().password;
                    return RedirectToAction("Index", "Admin");
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
            return RedirectToAction("Login");
        }

        //Student Functions
        [HttpGet]
        public ActionResult StudentList(string name, int page = 1, int pagesize = 5)
        {
            var model = arrangeStudent(name, page, pagesize);
            return View(model);
        }

        public Tuple<PagedList.IPagedList<Student>, string> arrangeStudent(string studentName, int page = 1, int pagesize = 5)
        {
            var model = searchStudent(studentName);
            model = model.OrderBy(x => x.name).ToPagedList(page, pagesize);
            var tupleModel = new Tuple<PagedList.IPagedList<Student>, string>((IPagedList<Student>)model, studentName);
            return tupleModel;
        }

        public IEnumerable<Student> searchStudent(string studentName)
        {
            if (string.IsNullOrWhiteSpace(studentName))
            {
                return db.Students;
            }

            return db.Students.Where(s => s.name.Contains(studentName) || SqlFunctions.PatIndex("%" + studentName + "%", s.name) > 0);
        }

        public ActionResult CreateStudent()
        {
            var courses = db.Courses.ToList();
            ViewBag.CourseList = new SelectList(courses, "id", "name");
            return View();
        }
        [HttpPost, ActionName("CreateStudent")]
        [ValidateInput(false)]
        public ActionResult CreateStudent(Student hocsinh)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = db.Students.FirstOrDefault(s => s.id == hocsinh.id);
                    if (check == null)
                    {
                        db.Students.Add(hocsinh);
                        db.SaveChanges();
                        return RedirectToAction("StudentList");
                    }
                    else
                    {
                        ViewBag.ThongBao = "Mã Học Sinh Đã Tồn Tại";
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
        public ActionResult EditStudent()
        {
            var courses = db.Courses.ToList();
            ViewBag.CourseList = new SelectList(courses, "id", "name");
            return View();
        }

        [HttpPost, ActionName("EditStudent")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudent(int id)
        {
            var hocsinhUpdate = db.Students.Find(id);
            if (TryUpdateModel(hocsinhUpdate, "", new string[] { "name", "englishName", "birthday", "gender", "contactPerson", "telephone", "username", "password", "courseID" }))
            {
                try
                {
                    db.Entry(hocsinhUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }
            }

            return RedirectToAction("StudentList");
        }

        [HttpGet]
        public ActionResult DeleteStudent(int id)
        {
            Student ST = db.Students.SingleOrDefault(n => n.id == id);
            return View(ST);
        }
        [HttpPost, ActionName("DeleteStudent")]
        public ActionResult DeleteStudentConfirmed(int id)
        {
            Student ST = db.Students.SingleOrDefault(n => n.id == id);
            db.Students.Remove(ST);
            db.SaveChanges();
            return RedirectToAction("StudentList");
        }
        //Student Functions

        //Teacher Functions
        [HttpGet]
        public ActionResult TeacherList(string name, int page = 1, int pagesize = 5)
        {
            var model = arrangeTeacher(name, page, pagesize);
            return View(model);
        }

        public Tuple<PagedList.IPagedList<Teacher>, string> arrangeTeacher(string teacherName, int page = 1, int pagesize = 5)
        {
            var model = searchTeacher(teacherName);
            model = model.OrderBy(x => x.name).ToPagedList(page, pagesize);
            var tupleModel = new Tuple<PagedList.IPagedList<Teacher>, string>((IPagedList<Teacher>)model, teacherName);
            return tupleModel;
        }

        public IEnumerable<Teacher> searchTeacher(string teacherName)
        {
            if (string.IsNullOrWhiteSpace(teacherName))
            {
                return db.Teachers;
            }

            return db.Teachers.Where(s => s.name.Contains(teacherName) || SqlFunctions.PatIndex("%" + teacherName + "%", s.name) > 0);
        }

        public ActionResult CreateTeacher()
        {
            return View();
        }
        [HttpPost, ActionName("CreateTeacher")]
        [ValidateInput(false)]
        public ActionResult CreateTeacher(Teacher giaovien)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = db.Teachers.FirstOrDefault(s => s.id == giaovien.id);
                    if (check == null)
                    {
                        db.Teachers.Add(giaovien);
                        db.SaveChanges();
                        return RedirectToAction("TeacherList");
                    }
                    else
                    {
                        ViewBag.ThongBao = "Mã Giáo Viên Đã Tồn Tại";
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
        public ActionResult EditTeacher()
        {
            return View();
        }

        [HttpPost, ActionName("EditTeacher")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditTeacher(int id)
        {
            var teacherUpdate = db.Teachers.Find(id);
            if (TryUpdateModel(teacherUpdate, "", new string[] { "name", "gender", "role", "telephone", "username", "password" }))
            {
                try
                {
                    db.Entry(teacherUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }
            }

            return RedirectToAction("TeacherList");
        }

        [HttpGet]
        public ActionResult DeleteTeacher(int id)
        {
            Teacher TC = db.Teachers.SingleOrDefault(n => n.id == id);
            return View(TC);
        }
        [HttpPost, ActionName("DeleteTeacher")]
        public ActionResult DeleteTeacherConfirmed(int id)
        {
            Teacher TC = db.Teachers.SingleOrDefault(n => n.id == id);
            db.Teachers.Remove(TC);
            db.SaveChanges();
            return RedirectToAction("TeacherList");
        }

        //Teacher Functions

        //TA Functions
        [HttpGet]
        public ActionResult TAList(string name, int page = 1, int pagesize = 5)
        {
            var model = arrangeTA(name, page, pagesize);
            return View(model);
        }

        public Tuple<PagedList.IPagedList<TA>, string> arrangeTA(string TAName, int page = 1, int pagesize = 5)
        {
            var model = SearchTA(TAName);
            model = model.OrderBy(x => x.name).ToPagedList(page, pagesize);
            var tupleModel = new Tuple<PagedList.IPagedList<TA>, string>((IPagedList<TA>)model, TAName);
            return tupleModel;
        }

        public IEnumerable<TA> SearchTA(string TAName)
        {
            if (string.IsNullOrWhiteSpace(TAName))
            {
                return db.TAs;
            }

            return db.TAs.Where(s => s.name.Contains(TAName) || SqlFunctions.PatIndex("%" + TAName + "%", s.name) > 0);
        }

        public ActionResult CreateTA()
        {
            return View();
        }
        [HttpPost, ActionName("CreateTA")]
        [ValidateInput(false)]
        public ActionResult CreateTA(TA trogiang)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = db.TAs.FirstOrDefault(s => s.id == trogiang.id);
                    if (check == null)
                    {
                        db.TAs.Add(trogiang);
                        db.SaveChanges();
                        return RedirectToAction("TAList");
                    }
                    else
                    {
                        ViewBag.ThongBao = "Mã Trợ Giảng Đã Tồn Tại";
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
        public ActionResult EditTA()
        {
            return View();
        }

        [HttpPost, ActionName("EditTA")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditTA(int id)
        {
            var TAUpdate = db.TAs.Find(id);
            if (TryUpdateModel(TAUpdate, "", new string[] { "name", "gender", "telephone", "username", "password" }))
            {
                try
                {
                    db.Entry(TAUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }
            }

            return RedirectToAction("TAList");
        }

        [HttpGet]
        public ActionResult DeleteTA(int id)
        {
            TA TA = db.TAs.SingleOrDefault(n => n.id == id);
            return View(TA);
        }
        [HttpPost, ActionName("DeleteTA")]
        public ActionResult DeleteTAConfirmed(int id)
        {
            TA TA = db.TAs.SingleOrDefault(n => n.id == id);
            db.TAs.Remove(TA);
            db.SaveChanges();
            return RedirectToAction("TAList");
        }

        //TA Functions

        //Course Functions
        [HttpGet]
        public ActionResult CourseList(string name, int page = 1, int pagesize = 5)
        {
            var model = arrangeCourse(name, page, pagesize);

            return View(model);
        }

        public Tuple<PagedList.IPagedList<Course>, string> arrangeCourse(string courseName, int page = 1, int pagesize = 5)
        {
            var model = searchCourse(courseName);
            model = model.OrderBy(x => x.name).ToPagedList(page, pagesize);
            var tupleModel = new Tuple<PagedList.IPagedList<Course>, string>((IPagedList<Course>)model, courseName);
            return tupleModel;
        }

        public IEnumerable<Course> searchCourse(string courseName)
        {
            if (string.IsNullOrWhiteSpace(courseName))
            {
                return db.Courses;
            }

            return db.Courses.Where(s => s.name.Contains(courseName) || SqlFunctions.PatIndex("%" + courseName + "%", s.name) > 0);
        }

        public ActionResult CreateCourse()
        {
            var teachers = db.Teachers.ToList();
            var TAs = db.TAs.ToList();

            ViewBag.TeacherList = new SelectList(teachers, "id", "name");
            ViewBag.TAList = new SelectList(TAs, "id", "name");

            return View();
        }
        [HttpPost, ActionName("CreateCourse")]
        [ValidateInput(false)]
        public ActionResult CreateCourse(Course khoahoc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = db.Courses.FirstOrDefault(s => s.id == khoahoc.id);
                    if (check == null)
                    {
                        db.Courses.Add(khoahoc);
                        db.SaveChanges();
                        return RedirectToAction("CourseList");
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
        public ActionResult EditCourse()
        {
            var teachers = db.Teachers.ToList();
            var TAs = db.TAs.ToList();

            ViewBag.TeacherList = new SelectList(teachers, "id", "name");
            ViewBag.TAList = new SelectList(TAs, "id", "name");

            return View();
        }

        [HttpPost, ActionName("EditCourse")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditCourse(int id)
        {
            var courseUpdate = db.Courses.Find(id);
            if (TryUpdateModel(courseUpdate, "", new string[] { "name", "courseLevel", "module", "duration", "beginDay", "endDay", "time", "room", "teacherID", "TA_ID" }))
            {
                try
                {
                    db.Entry(courseUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }
            }

            return RedirectToAction("CourseList");
        }

        [HttpGet]
        public ActionResult DeleteCourse(int id)
        {
            Course CS = db.Courses.SingleOrDefault(n => n.id == id);
            return View(CS);
        }
        [HttpPost, ActionName("DeleteCourse")]
        public ActionResult DeleteCourseConfirmed(int id)
        {
            Course CS = db.Courses.SingleOrDefault(n => n.id == id);
            db.Courses.Remove(CS);
            db.SaveChanges();
            return RedirectToAction("CourseList");
        }

        //Course Functions

        //Couse Schedule Functions
        [HttpGet]
        public ActionResult ScheduleList(string scheduleLesson, int page = 1, int pagesize = 5)
        {
            var model = arrangeSchedule(scheduleLesson, page, pagesize);
            return View(model);
        }

        public Tuple<PagedList.IPagedList<Course_Schedule>, string> arrangeSchedule(string scheduleLesson, int page = 1, int pagesize = 5)
        {
            var model = searchSchedule(scheduleLesson);
            model = model.OrderBy(x => x.lesson).ToPagedList(page, pagesize);
            var tupleModel = new Tuple<PagedList.IPagedList<Course_Schedule>, string>((IPagedList<Course_Schedule>)model, scheduleLesson);
            return tupleModel;
        }

        public IEnumerable<Course_Schedule> searchSchedule(string scheduleLesson)
        {
            if (string.IsNullOrWhiteSpace(scheduleLesson))
            {
                return db.Course_Schedule;
            }

            return db.Course_Schedule.Where(s => s.lessonAims.Contains(scheduleLesson) || SqlFunctions.PatIndex("%" + scheduleLesson + "%", s.lessonAims) > 0);
        }

        public ActionResult CreateSchedule()
        {
            var courses = db.Courses.ToList();
            var teachers = db.Teachers.ToList();
            var TAs = db.TAs.ToList();
            ViewBag.CourseList = new SelectList(courses, "id", "name");
            ViewBag.TeacherList = new SelectList(teachers, "id", "name");
            ViewBag.TAList = new SelectList(TAs, "id", "name");
            return View();
        }
        [HttpPost, ActionName("CreateSchedule")]
        [ValidateInput(false)]
        public ActionResult CreateSchedule(Course_Schedule lichhoc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = db.Course_Schedule.FirstOrDefault(s => s.id == lichhoc.id);
                    if (check == null)
                    {
                        db.Course_Schedule.Add(lichhoc);
                        db.SaveChanges();
                        return RedirectToAction("ScheduleList");
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
        public ActionResult EditSchedule()
        {
            var courses = db.Courses.ToList();
            var teachers = db.Teachers.ToList();
            var TAs = db.TAs.ToList();
            ViewBag.CourseList = new SelectList(courses, "id", "name");
            ViewBag.TeacherList = new SelectList(teachers, "id", "name");
            ViewBag.TAList = new SelectList(TAs, "id", "name");
            return View();
        }

        [HttpPost, ActionName("EditSchedule")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditSchedule(int id)
        {
            var scheduleUpdate = db.Course_Schedule.Find(id);
            if (TryUpdateModel(scheduleUpdate, "", new string[] { "lesson", "date", "unit_and_grade", "lessonAims", "notes", "teacher", "TA", "courseId" }))
            {
                try
                {
                    db.Entry(scheduleUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }
            }

            return RedirectToAction("ScheduleList");
        }

        [HttpGet]
        public ActionResult DeleteSchedule(int id)
        {
            Course_Schedule CS = db.Course_Schedule.SingleOrDefault(n => n.id == id);
            return View(CS);
        }
        [HttpPost, ActionName("DeleteSchedule")]
        public ActionResult DeleteScheduleConfirmed(int id)
        {
            Course_Schedule CS = db.Course_Schedule.SingleOrDefault(n => n.id == id);
            db.Course_Schedule.Remove(CS);
            db.SaveChanges();
            return RedirectToAction("ScheduleList");
        }

        //Couse Schedule Functions
    }
}