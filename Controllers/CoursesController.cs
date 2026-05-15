using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wikimedia.Models;
using DAL;

namespace Wikimedia.Controllers
{
    public class CoursesController : Controller
    {
        public ActionResult CourseList(string search = "")
        {
            var courses = DB.Courses.ToList();
            ViewBag.SearchString = search;
            ViewBag.Search = !string.IsNullOrEmpty(search);

            Session["CourseYearsList"] = courses.Select(c => int.Parse(c.Session)).Distinct().OrderByDescending(y => y).ToList();
            return View(courses);
        }

        public ActionResult Details(int id)
        {
            Course course = DB.Courses.Get(id);
            if (course == null)
                return RedirectToAction("List");

            var grouped = course.Registrations
                          .GroupBy(r => r.Year)
                          .OrderByDescending(g => g.Key)
                          .ToDictionary(g => g.Key, g => g.ToList());

            ViewBag.GroupedRegistrations = grouped;
            return View("CourseDetails",course);
        }

        public ActionResult Create()
        {
            ViewBag.PageTitle = "Cours - Création";
            return View(new Course());
        }
        public ActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                DB.Courses.Add(course);
                return RedirectToAction("List");
            }
            return View(course);
        }

        public ActionResult Edit(int id)
        {
            Course course = DB.Courses.Get(id);
            if (course == null)
                return RedirectToAction("List");

            var registeredStudents = course.NextSessionRegistrations.ToList();

            var allStudents = DB.Students.ToList();

            ViewBag.Registrations = new SelectList(registeredStudents, "Id", "FullName");
            ViewBag.Students = new SelectList(allStudents, "Id", "FullName");

            return View(course);
        }

        [HttpPost]
        public ActionResult Edit(Course course, List<int> selectedStudentsId)
        {
            if (ModelState.IsValid)
            {
                course.UpdateRegistrations(selectedStudentsId);
                DB.Courses.Update(course);
                return RedirectToAction("List");
            }
            return View(course);
        }

        public ActionResult Delete(int id)
        {
            Course course = DB.Courses.Get(id);
            if (course == null)
                return RedirectToAction("List");
            return View(course);
        }
    }
}