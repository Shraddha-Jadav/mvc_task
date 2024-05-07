using mvc_task.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Services.Protocols;

namespace mvc_task.Controllers
{
    public class HomeController : Controller
    {
        private shraddha_crmEntities1 _dbContext;

        public HomeController()
        {
            _dbContext = new shraddha_crmEntities1();
        }

        //-------------User Authentication-----------------
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Employee employee)
        {
            employee.DepartmentId = 1;
            employee.ReportingPerson = 1;
            _dbContext.Employees.Add(employee);
            _dbContext.SaveChanges();
            employee.EmployeeCode = "SIT-" + employee.EmployeeId;
            _dbContext.Entry(employee).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return RedirectToAction("login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Employee employee)
        {
            try
            {
                var obj = _dbContext.Employees
                     .Where(user => user.Email == employee.Email && user.Password == employee.Password)
                     .FirstOrDefault();

                if (obj != null)
                {
                    Session["EmpId"] = obj.EmployeeId;
                    Session["Email"] = obj.Email;
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password. Please try again.");
                    return View(employee);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while attempting to log in. Please try again later.");
                return View(employee);
            }
        }

        public ActionResult ResetPass()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPass(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                var empObj = _dbContext.Employees.FirstOrDefault(m =>  m.Email == employee.Email);
                if(empObj != null)
                {
                   empObj.Password = employee.Password;

                   _dbContext.Entry(empObj).State = EntityState.Modified;
                   _dbContext.SaveChanges();

                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Please Enter Registered Email");
                    return View();
                }
            }
            return View();
        }

        //-------------------Dashboard-------------------
        public ActionResult Dashboard()
        {
            if (Session["EmpId"]  != null)
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
                return RedirectToAction("Login");
            }
        }


        //------------------------Task------------------
        public ActionResult AddTask()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTask(Task task)
        {
            if(ModelState.IsValid)
            {
                task.EmployeeId = (int?)Session["EmpId"];
                task.ApproverId = 1;
                task.Status = "Pending";
                task.CreatedOn = DateTime.Today;

                _dbContext.Tasks.Add(task);
                _dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        public ActionResult EditTask()
        {
            var task = _dbContext.Tasks.FirstOrDefault();
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTask(Task task)
        {
            var taskObj = _dbContext.Tasks.Where(x => x.TaskID == task.TaskID).FirstOrDefault();
            if(taskObj != null)
            {
                taskObj.CreatedOn = DateTime.Today;
                taskObj.TaskDate = DateTime.Today;
                taskObj.TaskName = task.TaskName;
                taskObj.TaskDescription = task.TaskDescription;

                _dbContext.Entry(taskObj).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
            return View();
        }

        //-------------------Employee Detail-----------------
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
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }
    }
}