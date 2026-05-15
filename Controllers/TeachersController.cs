using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wikimedia.Models;
using static Controllers.AccessControl;

namespace Wikimedia.Controllers
{
    [UserAccess(Access.Admin)]
    public class TeachersController : Controller
    {
        public ActionResult List(string search = "")
        {
            var teachers = DB.Teachers.ToList();

            ViewBag.SearchString = search;
            ViewBag.Search = !string.IsNullOrEmpty(search);

            if (!string.IsNullOrEmpty(search))
            {
                teachers = teachers
                           .Where(t => t.FullName.ToLower().Contains(search.ToLower()))
                           .OrderBy(t => t.LastName)
                           .ToList();
            }

            Session["TeachersYearsList"] = teachers
                                        .Select(t => t.StartDate.Year)
                                        .Distinct()
                                        .OrderByDescending(y => y)
                                        .ToList();
            return View(teachers);
        }

        public ActionResult Details(int id)
        {
            Teacher teacher = DB.Teachers.Get(id);
            if (teacher == null)
                return RedirectToAction("List");
            var grouped = teacher.Allocations
                                  .OrderByDescending(a => a.Year)
                                  .GroupBy(a => a.Year)
                                  .ToDictionary(g => g.Key, g => g.ToList());

            ViewBag.GroupedAllocations = grouped;
            ViewBag.IsOwner = true;
            return View(teacher);
        }

        public ActionResult Create()
        {
            ViewBag.PageTitle = "Enseignant - Création";
            return View(new Teacher());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Teacher teacher)
        {
            if (teacher.IsValid())
            {
                teacher.Code = "CLG-420-" + new Random().Next(1000, 9999);
                DB.Teachers.Add(teacher);
                return RedirectToAction("List");
            }
            return View(teacher);
        }

        public ActionResult Edit(int id)
        {
            Teacher teacher = DB.Teachers.Get(id);
            if (teacher == null)
                return RedirectToAction("List");
            ViewBag.Allocations = teacher.NextSessionCoursesToSelectList;
            ViewBag.Courses = SelectListUtilities<Course>.Convert(DB.Courses.ToList().OrderBy(c => c.Code).ToList(), "Caption");
            return View(teacher);
        }

        public ActionResult Edit(Teacher teacher, List<int> selectedCoursesId)
        {
            if (teacher.IsValid())
            {
                teacher.UpdateAllocations(selectedCoursesId);
                DB.Teachers.Update(teacher);
                return RedirectToAction("List");
            }
            return View(teacher);
        }
        public ActionResult Delete(int id)
        {
            Teacher teacher = DB.Teachers.Get(id);
            if(teacher != null)
                DB.Teachers.Delete(id);
            return RedirectToAction("List");
        }
    }
}