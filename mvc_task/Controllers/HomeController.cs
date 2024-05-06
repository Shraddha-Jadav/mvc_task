using mvc_task.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;

namespace mvc_task.Controllers
{
    public class HomeController : Controller
    {
        private shraddha_crmEntities1 _dbcontext;

        public HomeController()
        {
            _dbcontext = new shraddha_crmEntities1();
        }

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
            _dbcontext.Employees.Add(employee);
            _dbcontext.SaveChanges();
            employee.EmployeeCode = "SIT-" + employee.EmployeeId;
            _dbcontext.Entry(employee).State = EntityState.Modified;
            _dbcontext.SaveChanges();
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
                var obj = _dbcontext.Employees
                     .Where(user => user.Email == employee.Email && user.Password == employee.Password)
                     .FirstOrDefault();

                if (obj != null)
                {
                    Session["EmpId"] = obj.EmployeeId;
                    Session["Email"] = obj.Email;
                    return RedirectToAction("Deshboard");
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

        public ActionResult Deshboard()
        {
            if (Session["EmpId"]  != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}