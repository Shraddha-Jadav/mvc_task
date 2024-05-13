using mvc_task.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mvc_task.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private shraddha_crmEntities2 _dbContext;

        public EmployeeController()
        {
            _dbContext = new shraddha_crmEntities2();
        }

        //------------------Index---------------

        public ActionResult Index()
        {
            var tasks = _dbContext.Tasks.ToList();
            int empId = (int)Session["EmpId"];
            var empName = (from e in _dbContext.Employees
                           where e.EmployeeId == empId
                           select e.FirstName).FirstOrDefault();
            ViewBag.name = empName;
            return View(tasks);
        }

        //---------------employee details----------
        public ActionResult ShowEmpDetails()
        {
            int id = (int)Session["EmpId"];
            var employee = _dbContext.Employees.Where(x => x.EmployeeId == id).FirstOrDefault();
            return View(employee);
        }

        public ActionResult EditPerDetail(int id)
        {
            var employee = _dbContext.Employees.Where(x => x.EmployeeId == id).FirstOrDefault();
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPerDetail(Employee employee)
        {
            var id = (int)Session["EmpId"];
            var empObj = _dbContext.Employees.FirstOrDefault(m => m.EmployeeId == id);

            if (empObj != null)
            {
                empObj.Email = employee.Email;
                empObj.FirstName = employee.FirstName;
                empObj.LastName = employee.LastName;
                empObj.DOB = employee.DOB;
                empObj.Gender = employee.Gender;

                _dbContext.Entry(empObj).State = EntityState.Modified;
                TempData["AlertMessage"] = "Edit Details Sucessfully...";
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        //-------------------------task---------------------
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
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult EditTask(int? id, string status)
        {
            if (status != "Pending")
            {
                return RedirectToAction("Index");
            }
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
                return RedirectToAction("Index");
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
            return RedirectToAction("Index");
        }
    }
}