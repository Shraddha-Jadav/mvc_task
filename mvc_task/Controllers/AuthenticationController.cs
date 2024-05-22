using mvc_task.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;

namespace mvc_task.Controllers
{
    public class AuthenticationController : Controller
    {
        private shraddha_crmEntities2 _dbContext;

        public AuthenticationController()
        {
            _dbContext = new shraddha_crmEntities2();
        }

        //------------------------------------------SignUp-----------------------------------------
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
                ModelState.AddModelError("Email", "Email already exists. Please enter another email.");
            }
            if(employee.Password != employee.RepeatPassword)
            {
                ModelState.AddModelError("RepeatPassword", "Password and Confirm password are not same!!");
            }

            ModelState.Remove("DepartmentId");
            ModelState.Remove("ReportingPerson");
            if (ModelState.IsValid)
            {
                employee.DepartmentId = 1;
                employee.ReportingPerson = 1;
                employee.Password = HashPassword(employee.Password);
                _dbContext.Employees.Add(employee);
                _dbContext.SaveChanges();
                TempData["AlertMessage"] = "Register sucessfully...";
                return RedirectToAction("login");
            }
            else
            {
                ModelState.AddModelError("", "Somthing went wrong");
                return View(employee);
            }
        }

        //---------------------------------------------Sign In--------------------------------------
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Employee employee)
        {
            try
            {
                var hashPass = HashPassword(employee.Password);
                var obj = _dbContext.Employees
                     .Where(user => user.Email == employee.Email && user.Password == hashPass)
                     .FirstOrDefault();

                if (obj != null)
                {
                    var token = JwtHelper.GenerateToken(obj.EmployeeId, obj.Email);
                    var cookie = new HttpCookie("jwt", token)
                    {
                        Expires = DateTime.Now.AddHours(2)
                    };

                    Response.Cookies.Add(cookie);

                    Session["LoginId"] = obj.Email;
                    Session["EmpId"] = obj.EmployeeId;
                    Session["Name"] = obj.FirstName;
                    Session["Department"] = obj.DepartmentId;
                    FormsAuthentication.SetAuthCookie(obj.Email, false);

                    if (obj.DepartmentId == 1)
                    {
                        return RedirectToAction("AllTasks", "Task");
                    }
                    else if (obj.DepartmentId == 2)
                    {
                        return RedirectToAction("Index", "Manager");
                    }
                    else if (obj.DepartmentId == 3)
                    {
                        return RedirectToAction("Index", "Director");
                    }
                    else
                    {
                        ModelState.AddModelError("Email", "Somthing went wrong");
                        return View();
                    }
                }

                ModelState.AddModelError("", "Invalid username or password");
                return View(employee);
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while attempting to login. Please try again later.");
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
                    empObj.Password = HashPassword(employee.Password);
                    _dbContext.Entry(empObj).State = EntityState.Modified;

                    _dbContext.Configuration.ValidateOnSaveEnabled = false;
                    _dbContext.SaveChanges();
                    _dbContext.Configuration.ValidateOnSaveEnabled = true;

                    TempData["AlertMessage"] = "Reset Password Sucessfully...";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("Email", "Please Enter Registered Email");
                    return View();
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                var sessionCookie = new HttpCookie("ASP.NET_SessionId")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Path = "/"
                };
                Response.Cookies.Add(sessionCookie);
            }
            TempData["AlertMessage"] = "Logout Sucessfully...";
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashedBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }
}