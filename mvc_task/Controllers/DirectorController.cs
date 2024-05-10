
using mvc_task.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public ActionResult Dashboard()
        {
            var employees = _dbContext.Employees.ToList();
            int directorId = (int)Session["EmpId"];
            string dirName = (from e in _dbContext.Employees where e.EmployeeId == directorId select e.FirstName).FirstOrDefault();
            ViewBag.name = dirName;
            return View(employees);
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
            else if(empObj.DepartmentId == 2)
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

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmp(EditUserViewModel employee)
        {
            ModelState.Remove("EmpObj.Password");
            ModelState.Remove("EmpObj.ReportingPerson");
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
                    _dbContext.SaveChanges();
                }
                return RedirectToAction("Dashboard");
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
            return View();
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
            var tasks = _dbContext.Tasks.Where(x => x.EmployeeId == EmpId).ToList();
            return RedirectToAction("Dashboard");
        }
    }
}