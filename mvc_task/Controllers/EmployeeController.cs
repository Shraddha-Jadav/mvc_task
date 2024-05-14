using mvc_task.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Task = mvc_task.Models.Task;

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
    }
}