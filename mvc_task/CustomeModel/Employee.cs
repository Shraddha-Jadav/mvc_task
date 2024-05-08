using mvc_task.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace mvc_task.Model
{
        internal class EmployeeMetaData
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]

            public int EmployeeId { get; set; }

            [Display(Name = "EmpCode", ResourceType = typeof(StringResources))]
            public string EmployeeCode { get; set; }

            [Display(Name = "Email", ResourceType = typeof(StringResources))]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(StringResources))]
            [DataType(DataType.EmailAddress, ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(StringResources))]
            public string Email { get; set; }

            [Display(Name = "Pass", ResourceType = typeof(StringResources))]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(StringResources))]
            [DataType(DataType.Password)]
            [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessageResourceName = "PassError", ErrorMessageResourceType = typeof(StringResources))]
            public string Password { get; set; }

            [Display(Name = "FName", ResourceType = typeof(StringResources))]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(StringResources))]
            public string FirstName { get; set; }

            [Display(Name = "LName", ResourceType = typeof(StringResources))]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(StringResources))]
            public string LastName { get; set; }

            [Display(Name = "DOB", ResourceType = typeof(StringResources))]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(StringResources))]

            public Nullable<System.DateTime> DOB { get; set; }

            [Display(Name = "Gender", ResourceType = typeof(StringResources))]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(StringResources))]
            public string Gender { get; set; }

            [Display(Name = "DepId", ResourceType = typeof(StringResources))]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(StringResources))]
            public Nullable<int> DepartmentId { get; set; }

            [Display(Name = "ReportingPer", ResourceType = typeof(StringResources))]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(StringResources))]
            public Nullable<int> ReportingPerson { get; set; }

            public virtual ICollection<Employee> Employee1 { get; set; }
            public virtual Employee Employee2 { get; set; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<Task> Tasks { get; set; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<Task> Tasks1 { get; set; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<Task> Tasks2 { get; set; }
        }
}