using mvc_task.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace mvc_task.Controllers
{
    [Authorize (Roles = "Employee, Manager")]
    public class TaskController : Controller
    {
        private shraddha_crmEntities2 _dbContext;

        public TaskController()
        {
            _dbContext = new shraddha_crmEntities2();
        }

        public ActionResult AllTasks()
        {
            var tasks = _dbContext.Tasks.ToList();
            int empId = (int)Session["EmpId"];
            var empName = (from e in _dbContext.Employees
                           where e.EmployeeId == empId
                           select e.FirstName).FirstOrDefault();
            ViewBag.name = empName;
            return View(tasks);
        }

        public ActionResult AddTask(int id) //Action for open partial view for add or update task
        {
            if (id == 0)
            {
                Task task = new Task();
                return PartialView("_PartialPageTask", task);
            }
            else
            {
                var task = _dbContext.Tasks.Where(x => x.TaskID == id).FirstOrDefault();
                return PartialView("_PartialPageTask", task);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult submitTask(Task task) //Add or Update task in model
        {
            if (ModelState.IsValid)
            {
                task.EmployeeId = (int?)Session["EmpId"];
                task.Status = "Pending";
                if (task.TaskID == 0)
                {
                    task.CreatedOn = DateTime.Today;
                }
                else
                {
                    var currentTask = _dbContext.Tasks.Where(x => x.TaskID == task.TaskID).FirstOrDefault();
                    task.CreatedOn = currentTask.CreatedOn;
                    task.ModifiedOn = DateTime.Today;
                }
                task.ApproverId = 1;
                _dbContext.Tasks.AddOrUpdate(task);
                _dbContext.SaveChanges();
                TempData["AlertMessage"] = "Add task sucessfully...";
                return RedirectToAction("AllTasks");
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
            return RedirectToAction("AllTasks");
        }
    }
}