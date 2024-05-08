using mvc_task.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mvc_task.Controllers
{
    public class DashboardController : Controller
    {
        private shraddha_crmEntities1 _dbContext;

        public DashboardController()
        {
            _dbContext = new shraddha_crmEntities1();
        }
        public ActionResult Dashboard()
        {
            if (Session["EmpId"] != null)
            {
                var tasks = _dbContext.Tasks.ToList();
                int empId = (int)Session["EmpId"];
                var empName = (from e in _dbContext.Employees
                               where e.EmployeeId == empId
                               select e.FirstName).FirstOrDefault();
                ViewBag.EmployeeName = empName;
                return View(tasks);
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }
        }

        public ActionResult AddTask()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTask(Task task)
        {
            if (ModelState.IsValid)
            {
                task.EmployeeId = (int?)Session["EmpId"];
                task.ApproverId = 1;
                task.Status = "Pending";
                task.CreatedOn = DateTime.Today;

                _dbContext.Tasks.Add(task);
                _dbContext.SaveChanges();
                TempData["AlertMessage"] = "Add task sucessfully...";
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        public ActionResult EditTask(int? id)
        {
            var task = _dbContext.Tasks.Where(x => x.TaskID == id).FirstOrDefault();
            TempData["TaskId"] = id;
            TempData.Keep();
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTask(Task task)
        {
            int id = (int)TempData["TaskId"];
            var taskObj = _dbContext.Tasks.Where(x => x.TaskID == id).FirstOrDefault();
            if (taskObj != null)
            {
                taskObj.CreatedOn = DateTime.Today;
                taskObj.ModifiedOn = DateTime.Today;
                taskObj.TaskDate = DateTime.Today;
                taskObj.TaskName = task.TaskName;
                taskObj.TaskDescription = task.TaskDescription;

                _dbContext.Entry(taskObj).State = EntityState.Modified;
                _dbContext.SaveChanges();
                TempData["AlertMessage"] = "Edit task sucessfully...";
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        public ActionResult DeleteTask(int id)
        {
            var taskObj = _dbContext.Tasks.Find(id);
            if (taskObj != null)
            {
                _dbContext.Tasks.Remove(taskObj);
                _dbContext.SaveChanges();
                TempData["AlertMessage"] = "Delete task sucessfully...";
            }
            return RedirectToAction("Dashboard");
        }
    }
}