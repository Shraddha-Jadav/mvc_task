using mvc_task.Filter;
using mvc_task.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace mvc_task.Controllers
{
    [RoleAuthorize("Director" , "Manager", "Employee")]
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
                Session["Name"] = employee.FirstName;
                _dbContext.Entry(empObj).State = EntityState.Modified;
                ModelState.Remove(employee.RepeatPassword);
                _dbContext.SaveChanges();

                var employeeDetail = new
                {
                    FirstName = empObj.FirstName,
                    LastName = empObj.LastName,
                    DOB = empObj.DOB,
                    Gender = empObj.Gender
                };

                return Json(new { success = true, employee = employeeDetail });
            }
            return Json(new { success = false, error = "Employee not found" });
        }

    }
}