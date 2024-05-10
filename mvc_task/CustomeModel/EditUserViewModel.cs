using mvc_task.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mvc_task.Models
{
    public class EditUserViewModel
    {
        public Employee EmpObj { get; set; }
        public IList<SelectListItem> DepartmentNames { get; set; }
        public IList<SelectListItem> EmployeeNames { get; set; }
    }
}