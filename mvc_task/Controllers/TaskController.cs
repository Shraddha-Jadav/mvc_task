using mvc_task.CustomeModel;
using mvc_task.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace mvc_task.Controllers
{
    [Authorize (Roles = "Employee, Manager")]
    public class TaskController : Controller
    {
        private shraddha_crmEntities2 _dbContext;

        public TaskController()
        {
            _dbContext = new shraddha_crmEntities2();
        }

        public ActionResult AllTasks()
        {
            return View();
        }

        public ActionResult AddTask(int id) //Action for open partial view for add or update task
        {
            if (id == 0)
            {
                Task task = new Task();
                return PartialView("_PageTask", task);
            }
            else
            {
                var task = _dbContext.Tasks.Where(x => x.TaskID == id).FirstOrDefault();
                return PartialView("_PageTask", task);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult submitTask(Task task) //Add or Update task in model
        {
            if (ModelState.IsValid)
            {
                task.EmployeeId = (int?)Session["EmpId"];
                task.Status = "Pending";
                if (task.TaskID == 0)
                {
                    task.CreatedOn = DateTime.Today;
                }
                else
                {
                    var currentTask = _dbContext.Tasks.Where(x => x.TaskID == task.TaskID).FirstOrDefault();
                    task.CreatedOn = currentTask.CreatedOn;
                    task.ModifiedOn = DateTime.Today;
                }
                task.ApproverId = 1;
                _dbContext.Tasks.AddOrUpdate(task);
                _dbContext.SaveChanges();
                TempData["AlertMessage"] = task.TaskID == 0 ? "Edit task sucessfully..." : "Add task sucessfully...";

                return Json(new { success = true, task = task });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        public ActionResult DeleteTask(int id)
        {
            var taskObj = _dbContext.Tasks.Find(id);
            if (taskObj != null)
            {
                _dbContext.Tasks.Remove(taskObj);
                _dbContext.SaveChanges();
                TempData["AlertMessage"] = "Delete task sucessfully...";
            }
            return RedirectToAction("AllTasks");
        }

        [HttpPost]
        public ActionResult GetList()
        {
            var TaskList = _dbContext.Tasks.ToList();
            return Json(new { data = TaskList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EmployeeTasks(JqueryDatatableParams model)
        {
            var loggedInEmployee = _dbContext.Employees.FirstOrDefault(e => e.Email == User.Identity.Name);

            if (loggedInEmployee != null)
            {
                var tasks = _dbContext.Tasks.Where(t => t.EmployeeId == loggedInEmployee.EmployeeId).ToList();

                if (!string.IsNullOrEmpty(model.search?.value))
                {
                    string searchValue = model.search.value.ToLower();
                    tasks = tasks.Where(t =>
                        t.TaskID.ToString().Contains(searchValue) ||
                        t.TaskDate.ToString().Contains(searchValue) ||
                        t.TaskName.ToLower().Contains(searchValue) ||
                        t.TaskDescription.ToLower().Contains(searchValue) ||
                        t.Employee2.FirstName.ToLower().Contains(searchValue) ||
                        t.Employee2.LastName.ToLower().Contains(searchValue) ||
                        t.Employee1.FirstName.ToLower().Contains(searchValue) ||
                        t.Employee1.LastName.ToLower().Contains(searchValue) ||
                        t.Status.ToLower().Contains(searchValue) ||
                        (t.CreatedOn.HasValue && t.CreatedOn.Value.ToString("yyyy-MM-dd").Contains(searchValue)) ||
                        (t.ModifiedOn.HasValue && t.ModifiedOn.Value.ToString("yyyy-MM-dd").Contains(searchValue))
                    ).ToList();
                }
                
                var recordsTotal = tasks.Count();

                if (model.order != null && model.order.Count > 0)
                {
                    var order = model.order[0];
                    string columnName = model.columns[order.column].data;
                    bool isAscending = order.dir == "asc";

                    if (!string.IsNullOrEmpty(columnName))
                    {
                        var propertyInfo = typeof(Task).GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                        if (propertyInfo != null)
                        {
                            if (isAscending)
                            {
                                tasks = tasks.OrderBy(p => propertyInfo.GetValue(p, null)).ToList();
                            }
                            else
                            {
                                tasks = tasks.OrderByDescending(p => propertyInfo.GetValue(p, null)).ToList();
                            }
                        }
                    }
                }

                var data = tasks
                    .Skip(model.start)
                    .Take(model.length)
                    .Select(t => new
                    {
                        t.TaskID,
                        t.TaskName,
                        t.TaskDescription,
                        TaskDate = t.TaskDate.HasValue ? t.CreatedOn.Value.ToString("yyyy-MM-dd") : string.Empty,
                        t.Status,
                        ApprovedOrRejectedOn = t.ApprovedOrRejectedOn.HasValue ? t.CreatedOn.Value.ToString("yyyy-MM-dd") : string.Empty,
                        CreatedOn = t.CreatedOn.HasValue ? t.CreatedOn.Value.ToString("yyyy-MM-dd") : string.Empty,
                        ModifiedOn = t.ModifiedOn.HasValue ? t.ModifiedOn.Value.ToString("yyyy-MM-dd") : string.Empty
                    })
                    .ToList();

                var jsonData = new
                {
                    draw = model.draw,
                    recordsTotal = tasks.Count,
                    recordsFiltered = recordsTotal,
                    data
                };

                return Content(JsonConvert.SerializeObject(jsonData), "application/json");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}