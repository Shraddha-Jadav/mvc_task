using mvc_task.Filter;
using mvc_task.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mvc_task.Controllers
{
    [RoleAuthorize("Manager")]
    public class ManagerController : Controller
    {
        private shraddha_crmEntities2 _dbContext;

        public ManagerController()
        {
            _dbContext = new shraddha_crmEntities2();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EmployeeList()
        {
            int id = (int)Session["EmpId"];
            var emps = _dbContext.Employees.Where(x => x.ReportingPerson == id).ToList();
            return View(emps);
        }

        public ActionResult Tasks(int? id)
        {
            if (id != null)
            {
                var task = _dbContext.Tasks.Where(x => x.EmployeeId == id).ToList();
                Session["TaskEmpId"] = id;
                return View(task);
                //return RedirectToAction("_TaskTable", task);
            }
            return View();
        }

        //public ActionResult AppOrRejByManger(int? id, string btn)
        //{
        //    if (id != null)
        //    {
        //        var taskObj = _dbContext.Tasks.Where(x => x.TaskID == id).FirstOrDefault();
        //        taskObj.ApprovedOrRejectedBy = (int)Session["EmpId"];
        //        taskObj.ApprovedOrRejectedOn = DateTime.Now;
        //        taskObj.Status = btn == "approve" ? "Approved" : "Rejected";
        //        _dbContext.Entry(taskObj).State = EntityState.Modified;
        //        _dbContext.SaveChanges();
        //    }
        //    int EmpId = (int)TempData["EmpId"];
        //    var tasks = _dbContext.Tasks.Where(x => x.EmployeeId == EmpId).ToList();
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public ActionResult AppOrRejByManager(int id, string btn)
        {
            var taskObj = _dbContext.Tasks.FirstOrDefault(x => x.TaskID == id);
            if (taskObj != null)
            {
                taskObj.ApprovedOrRejectedBy = (int)Session["EmpId"];
                taskObj.ApprovedOrRejectedOn = DateTime.Now;
                taskObj.Status = btn == "approve" ? "Approved" : "Rejected";
                _dbContext.SaveChanges();
            }
            int EmpId = (int)Session["TaskEmpId"];
            var tasks = _dbContext.Tasks.Where(x => x.EmployeeId == EmpId).ToList();
            return PartialView("_TaskTable", tasks);
        }
    }
}