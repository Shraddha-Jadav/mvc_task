
using mvc_task.CustomeModel;
using mvc_task.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace mvc_task.Controllers
{
    [Authorize(Roles = "Director")]
    public class DirectorController : Controller
    {
        private shraddha_crmEntities2 _dbContext;

        public DirectorController()
        {
            _dbContext = new shraddha_crmEntities2();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AllStaff(int? id)
        {
            if (id != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult GetEmployeeTasks(int? empId)
        {
            if(empId == null)
            {
                empId = 1043;
            }
            try
            {
                //passed only needed column to view ---- bcz - circular reference error
                var tasks = _dbContext.Tasks
                              .Where(x => x.EmployeeId == empId)
                              .Select(t => new
                              {
                                  TaskId = t.TaskID,
                                  TaskName = t.TaskName,
                                  TaskDescription = t.TaskDescription
                              })
                              .ToList();
                //var tasks = _dbContext.Tasks.Where(x => x.EmployeeId == empId).ToList();
                return Json(new { success = true, tasks = tasks });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, error = ex.Message });
            }
        }


        public ActionResult EditEmp(int? id)
        {
            var empObj = _dbContext.Employees.Where(x => x.EmployeeId == id).FirstOrDefault();
            List<SelectListItem> departmentNames = new List<SelectListItem>();
            List<Department> departmentList = _dbContext.Departments.ToList();

            if (empObj.DepartmentId == 1)
            {
                departmentNames = _dbContext.Departments.Where(d => d.DepartmentId == 1 || d.DepartmentId == 2 || d.DepartmentId == 3).Select(d => new SelectListItem { Text = d.Name, Value = d.DepartmentId.ToString() }).ToList();
            }
            else if (empObj.DepartmentId == 2)
            {
                departmentNames = _dbContext.Departments.Where(d => d.DepartmentId == 2 || d.DepartmentId == 3).Select(d => new SelectListItem { Text = d.Name, Value = d.DepartmentId.ToString() }).ToList();
            }
            else
            {
                departmentNames = _dbContext.Departments.Where(d => d.DepartmentId == 3).Select(d => new SelectListItem { Text = d.Name, Value = d.DepartmentId.ToString() }).ToList();
            }

            List<SelectListItem> employeeNames = new List<SelectListItem>();
            List<Employee> employeeList = _dbContext.Employees.ToList();
            employeeList.ForEach(x =>
            {
                employeeNames.Add(new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.EmployeeId.ToString() });
            });

            var viewModel = new EditUserViewModel
            {
                EmpObj = empObj,
                DepartmentNames = departmentNames,
                EmployeeNames = employeeNames
            };
            return PartialView("_EditUserByDir", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmp(EditUserViewModel employee)
        {
            ModelState.Remove("EmpObj.Password");
            ModelState.Remove("EmpObj.ReportingPerson");
            ModelState.Remove("EmpObj.Email");
            if (ModelState.IsValid)
            {
                int id = employee.EmpObj.EmployeeId;
                var empObj = _dbContext.Employees.Where(x => x.EmployeeId == id).FirstOrDefault();
                if (empObj != null)
                {
                    empObj.Email = employee.EmpObj.Email;
                    empObj.FirstName = employee.EmpObj.FirstName;
                    empObj.LastName = employee.EmpObj.LastName;
                    empObj.DOB = employee.EmpObj.DOB;
                    empObj.Gender = employee.EmpObj.Gender;
                    empObj.DepartmentId = employee.EmpObj.DepartmentId;
                    if (employee.EmpObj.ReportingPerson == null)
                    {
                        empObj.ReportingPerson = id;
                    }
                    else
                    {
                        empObj.ReportingPerson = employee.EmpObj.ReportingPerson;
                    }
                    _dbContext.Entry(empObj).State = EntityState.Modified;
                    _dbContext.Configuration.ValidateOnSaveEnabled = false;
                    _dbContext.SaveChanges();
                    _dbContext.Configuration.ValidateOnSaveEnabled = true;
                }
                return RedirectToAction("AllStaff", "Director", empObj.DepartmentId);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetReportingPer(string departmentId)
        {
            int deptId;
            List<SelectListItem> employeeNames = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(departmentId))
            {
                deptId = Convert.ToInt32(departmentId);
                if (deptId == 3)
                {
                    return Json(new { }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<Employee> employees = _dbContext.Employees.Where(x => x.DepartmentId == deptId + 1).ToList();
                    employees.ForEach(x =>
                    {
                        employeeNames.Add(new SelectListItem
                        {
                            Text = x.FirstName + " " + x.LastName,
                            Value = x.EmployeeId.ToString()
                        });
                    });
                }
                return Json(employeeNames, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult Tasks(int? id)
        {
            if (id != null)
            {
                var task = _dbContext.Tasks.Where(x => x.EmployeeId == id).ToList();
                TempData["EmpId"] = id;
                return View(task);
            }
            return RedirectToAction("Index");
        }

        public ActionResult AppOrRejByDir(int? id, string btn)
        {
            if (id != null)
            {
                var taskObj = _dbContext.Tasks.Where(x => x.TaskID == id).FirstOrDefault();
                taskObj.ApprovedOrRejectedBy = (int)Session["EmpId"];
                taskObj.ApprovedOrRejectedOn = DateTime.Now;
                taskObj.Status = btn == "approve" ? "Approved" : "Rejected";
                _dbContext.Entry(taskObj).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
            int EmpId = (int)TempData["EmpId"];
            return RedirectToAction("Tasks", new { id = EmpId });
        }

        public ActionResult DeleteEmp(int? id)
        {
            var emp = _dbContext.Employees.Find(id);
            if (emp != null)
            {
                var tasks = _dbContext.Tasks.Where(x => x.EmployeeId == id).ToList();
                foreach (var task in tasks)
                {
                    _dbContext.Tasks.Remove(task);
                }
                _dbContext.Employees.Remove(emp);
                _dbContext.Configuration.ValidateOnSaveEnabled = false;
                _dbContext.SaveChanges();
                _dbContext.Configuration.ValidateOnSaveEnabled = true;
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteManager(int? id)
        {
            var manager = _dbContext.Employees.Find(id);
            if (manager != null)
            {
                var empList = _dbContext.Employees.Where(x => x.ReportingPerson == id).ToList();
                foreach (var emp in empList)
                {
                    emp.ReportingPerson = manager.ReportingPerson;
                    _dbContext.Entry(emp).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                }

                _dbContext.Employees.Remove(manager);
                _dbContext.Configuration.ValidateOnSaveEnabled = false;
                _dbContext.SaveChanges();
                _dbContext.Configuration.ValidateOnSaveEnabled = true;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EmployeeList(JqueryDatatableParams model)
        {
            var employees = _dbContext.Employees.Where(x => x.DepartmentId == 1).ToList();

            if (!string.IsNullOrEmpty(model.search?.value))
            {
                string searchValue = model.search.value.ToLower();
                employees = employees
                .Where(e =>
                    e.EmployeeId.ToString().Contains(searchValue) ||
                    e.Email.ToLower().Contains(searchValue) ||
                    e.FirstName.ToLower().Contains(searchValue) ||
                    e.LastName.ToLower().Contains(searchValue) ||
                    e.DOB.HasValue && e.DOB.Value.ToString("yyyy-MM-dd").Contains(searchValue) ||
                    e.Gender.ToLower().Contains(searchValue) ||
                    e.ReportingPerson.ToString().Contains(searchValue) ||
                    e.EmployeeCode.ToLower().Contains(searchValue)
                 ).ToList();
            }

            var recordsTotal = employees.Count();

            if (model.order != null && model.order.Count > 0)
            {
                var order = model.order[0];
                string columnName = model.columns[order.column].data;
                bool isAscending = order.dir == "asc";

                if (!string.IsNullOrEmpty(columnName))
                {
                    var propertyInfo = typeof(Employee).GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo != null)
                    {
                        if (isAscending)
                        {
                            employees = employees.OrderBy(p => propertyInfo.GetValue(p, null)).ToList();
                        }
                        else
                        {
                            employees = employees.OrderByDescending(p => propertyInfo.GetValue(p, null)).ToList();
                        }
                    }
                }
            }

            var data = employees
                .Skip(model.start)
                .Take(model.length)
                .Select(e => new
                {
                    e.EmployeeId,
                    e.EmployeeCode,
                    e.Email,
                    e.FirstName,
                    e.LastName,
                    e.DOB,
                    e.Gender,
                    e.ReportingPerson
                }).ToList();

            var jsonData = new
            {
                draw = model.draw,
                recordsTotal = employees.Count,
                recordsFiltered = recordsTotal,
                data
            };

            return Content(JsonConvert.SerializeObject(jsonData), "application/json");
        }
    }
}