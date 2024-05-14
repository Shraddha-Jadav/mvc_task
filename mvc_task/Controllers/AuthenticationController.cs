using Antlr.Runtime.Tree;
using Microsoft.Ajax.Utilities;
using mvc_task.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace mvc_task.Controllers
{
    public class AuthenticationController : Controller
    {
        private shraddha_crmEntities2 _dbContext;

        public AuthenticationController()
        {
            _dbContext = new shraddha_crmEntities2();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Employee employee)
        {
            bool existingEmp = _dbContext.Employees.Any(u => u.Email == employee.Email);
            if (existingEmp)
            {
                Console.WriteLine("Error");
                ModelState.AddModelError("Emal", "Email already exists. Please enter another email address.");
            }
            if (ModelState.IsValid)
            {
                employee.DepartmentId = 1;
                employee.ReportingPerson = 1;
                _dbContext.Employees.Add(employee);
                _dbContext.SaveChanges();
                TempData["AlertMessage"] = "Register sucessfully...";
                return RedirectToAction("login");
            }
            return View(employee);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
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
                    Session["Name"] = obj.FirstName;
                    Session["Department"] = obj.DepartmentId;
                    FormsAuthentication.SetAuthCookie(obj.Email, false);
                    TempData["AlertMessage"] = "Login sucessfully...";
                    if ((int)Session["Department"] == 1)
                    {
                        return RedirectToAction("Index", "Employee");
                    }
                    else if ((int)Session["Department"] == 2)
                    {
                        return RedirectToAction("Index", "Manager");
                    }
                    else if ((int)Session["Department"] == 3)
                    {
                        return RedirectToAction("Index", "Director");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Somthing went wrong");
                        return View();
                    }
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
                var empObj = _dbContext.Employees.FirstOrDefault(m => m.Email == employee.Email);
                if (empObj != null)
                {
                    empObj.Password = employee.Password;

                    ModelState.Remove("Email");

                    _dbContext.Entry(empObj).State = EntityState.Modified;

                    _dbContext.Configuration.ValidateOnSaveEnabled = false;
                    _dbContext.SaveChanges();
                    _dbContext.Configuration.ValidateOnSaveEnabled = true;

                    TempData["AlertMessage"] = "Reset Password Sucessfully...";
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

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            TempData["AlertMessage"] = "Logout Sucessfully...";
            return RedirectToAction("Login");
        }
    }
}