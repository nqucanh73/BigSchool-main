﻿using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BigSchool.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses
        public ActionResult Create()
        {
            //get list category
            BigSchoolDB context = new BigSchoolDB();
            Course objCourse = new Course();
            objCourse.ListCategory = context.Categories.ToList();
            return View(objCourse);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objcourse)
        {
            BigSchoolDB con = new BigSchoolDB();

            ModelState.Remove("LectureId");
            if (!ModelState.IsValid)
            {
                objcourse.ListCategory = con.Categories.ToList();
                return View("Create", objcourse);
            }

            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objcourse.LectureId = user.Id;

            con.Courses.Add(objcourse);
            con.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
        public ActionResult Attending()
        {
            BigSchoolDB con = new BigSchoolDB();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var ListAttendances = con.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (Attendance temp in ListAttendances)
            {
                Course objCourse = temp.Course;
                objCourse.LectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(objCourse.LectureId).Name;
                courses.Add(objCourse);
            }
            return View(courses);
        }
        public ActionResult Mine()
        {
            BigSchoolDB con = new BigSchoolDB();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var courses = con.Courses.Where(c =>c.LectureId == currentUser.Id && c.Datetime > DateTime.Now).ToList();
           
            foreach (Course i in courses )
            {
                i.LectureName = currentUser.Name;
            }
            return View(courses);
        }
    }
}