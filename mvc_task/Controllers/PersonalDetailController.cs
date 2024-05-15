using mvc_task.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mvc_task.Controllers
{
    [Authorize(Roles = "Director, Manager, Employee")]
    public class PersonalDetailController : Controller
    {
        private shraddha_crmEntities2 _dbContext;

        public PersonalDetailController()
        {
            _dbContext = new shraddha_crmEntities2();
        }

        public ActionResult Index() {
            return View();
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
            return PartialView("_UserEditForm", employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDetails(Employee employee)
        {
            var id = (int)Session["EmpId"];
            var empObj = _dbContext.Employees.FirstOrDefault(m => m.EmployeeId == id);

            if (empObj != null)
            {
                empObj.FirstName = employee.FirstName;
                empObj.LastName = employee.LastName;
                empObj.DOB = employee.DOB;
                empObj.Gender = employee.Gender;

                Session["Email"] = employee.Email;
                Session["Name"] = employee.FirstName;
                _dbContext.Entry(empObj).State = EntityState.Modified;
                TempData["AlertMessage"] = "Edit Details Sucessfully...";
                _dbContext.Configuration.ValidateOnSaveEnabled = false;
                _dbContext.SaveChanges();
                _dbContext.Configuration.ValidateOnSaveEnabled = true;
            }
            return RedirectToAction("ShowEmpDetails");
        }
    }
}