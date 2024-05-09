using mvc_task.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mvc_task.Controllers
{
    [Authorize(Roles = "Director")]
    public class DirectorController : Controller
    {
        private shraddha_crmEntities2 _dbContext;

        public DirectorController()
        {
            _dbContext = new shraddha_crmEntities2();
        }

        public ActionResult Dashboard()
        {
            if (Session["EmpId"] != null)
            {
                var employees = _dbContext.Employees.ToList();
                int directorId = (int)Session["EmpId"];
                string dirName = (from e in _dbContext.Employees where e.EmployeeId == directorId select e.FirstName).FirstOrDefault();
                ViewBag.name  = dirName;
                return View(employees);
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }
        }
        
        public ActionResult EditEmp(int? id)
        {
            var empObj = _dbContext.Employees.Where(x => x.EmployeeId == id).FirstOrDefault();
            return View(empObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Director")]
        public ActionResult EditEmp(Employee employee)
        {
            List<SelectListItem> departmentNames = new List<SelectListItem>();
            Employee dm = new Employee();
            List<Department> departmentList = _dbContext.Departments.ToList();
            departmentList.ForEach(x =>
            {
                departmentNames.Add(new SelectListItem { Text = x.Name, Value = x.DepartmentId.ToString() });
            });
            dm.DepartmentNames = departmentNames;
            return View(dm);
        }

        [HttpPost]
        public ActionResult GetReportingPer()
        {
            List<SelectListItem> employeeNames = new List<SelectListItem>();
            EmployeeNamees en = new EmployeeNamees();
            return View();
        }

        public ActionResult Tasks(int? id)
        {
            if(id != null)
            {
                var task = _dbContext.Tasks.Where(x => x.EmployeeId == id).ToList();
                TempData["EmpId"] = id;
                return View(task);
            }
            return View();  
        }

        public ActionResult AppOrRejByDir(int? id, string btn)
        {
            if (id != null)
            {
                var taskObj = _dbContext.Tasks.Where(x => x.TaskID == id).FirstOrDefault();
                taskObj.ApprovedOrRejectedBy = (int)Session["EmpId"];
                taskObj.ApprovedOrRejectedOn = DateTime.Now;
                taskObj.Status = btn == "approve" ? "Approved" : "Rejected";
                _dbContext.Entry(taskObj).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
            int EmpId = (int)TempData["EmpId"];
            var tasks = _dbContext.Tasks.Where(x => x.EmployeeId == EmpId).ToList();    
            return RedirectToAction("Dashboard");
        }
    }
}