using mvc_task.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mvc_task.Controllers
{
    public class EmployeeController : Controller
    {
        private shraddha_crmEntities1 _dbContext;

        public EmployeeController()
        {
            _dbContext = new shraddha_crmEntities1();
        }
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
            return RedirectToAction("Dashboard", "Dashboard");
        }
    }
}