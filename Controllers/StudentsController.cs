using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wikimedia.Models;
using DAL;

namespace Wikimedia.Controllers
{
    public class StudentsController : Controller
    {
        public ActionResult SetYear()
        {
            ViewBag.Year = NextSession.Year;
            ViewBag.Session = NextSession.ValidSessions.Contains(1) ? "Automne" : "Hiver";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetYear(int year, string session)
        {
            NextSession.CurrentDate = new DateTime(year, (session == "Automne" ? 8 : 1), 15);
            return RedirectToAction("List");
        }

        public ActionResult Index(string search = "")
        {
            var students = DB.Students.ToList();
            ViewBag.SearchString = search;
            ViewBag.Search = !string.IsNullOrEmpty(search);

            return View(students);
        }

        public ActionResult List(string search = "")
        {
            var students = DB.Students.ToList();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                students = students.Where(s =>
                    (s.Code != null && s.Code.ToLower().Contains(search)) ||
                    (s.FirstName != null && s.FirstName.ToLower().Contains(search)) ||
                    (s.LastName != null && s.LastName.ToLower().Contains(search))
                ).ToList();
            }

            ViewBag.SearchString = search;
            ViewBag.SearchMode = Session["Search"] != null ? (bool)Session["Search"] : false;

            return View(students);
        }

        public ActionResult ToggleSearch()
        {
            bool current = Session["Search"] != null ? (bool)Session["Search"] : false;
            Session["Search"] = !current;

            if (!(bool)Session["Search"])
                Session["SearchString"] = null;

            return RedirectToAction("List");
        }

        public ActionResult Details(int id)
        {
            Student student = DB.Students.Get(id);
            if (student == null)
                return RedirectToAction("List");

            ViewBag.IsOwner = true;
            return View(student);
        }

        public ActionResult Create()
        {
            ViewBag.PageTitle = "Étudiant - Création";
            return View(new Student());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {
                DB.Students.Add(student);
                return RedirectToAction("List");
            }
            return View(student);
        }

        public ActionResult Edit(int id)
        {
            Student student = DB.Students.Get(id);
            if (student == null)
                return RedirectToAction("List");

            var registeredCourses = student.NextSessionCourses.ToList();
            var allCourses = DB.Courses.ToList();

            ViewBag.Registrations = new SelectList(registeredCourses, "Id", "Caption");
            ViewBag.Courses = new SelectList(allCourses, "Id", "Caption");

            return View(student);
        }

        [HttpPost]
        public ActionResult Edit(Student student, List<int> selectedCoursesId)
        {
            if (ModelState.IsValid)
            {
                student.UpdateRegistrations(selectedCoursesId);
                DB.Students.Update(student);
                return RedirectToAction("List");
            }
            return View(student);
        }

        public ActionResult Delete(int id)
        {
            Student student = DB.Students.Get(id);
            if (student == null)
                return RedirectToAction("List");

            return View(student);
        }
    }
}