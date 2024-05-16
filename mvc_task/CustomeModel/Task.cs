using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace mvc_task.Models
{
    [MetadataType(typeof(TaskMetaData))]
    public partial class Task
    {
        internal class TaskMetaData
        {
            public int TaskID { get; set; }

            [Display(Name = "TaskDate", ResourceType = typeof(StringResources))]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(StringResources))]
            public Nullable<System.DateTime> TaskDate { get; set; }

            [Display(Name = "EmpId", ResourceType = typeof(StringResources))]
            public Nullable<int> EmployeeId { get; set; }

            [Display(Name = "TaskName", ResourceType = typeof(StringResources))]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(StringResources))]
            public string TaskName { get; set; }

            [Display(Name = "TaskDesc", ResourceType = typeof(StringResources))]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(StringResources))]
            public string TaskDescription { get; set; }

            [Display(Name = "AppID", ResourceType = typeof(StringResources))]
            public Nullable<int> ApproverId { get; set; }

            [Display(Name = "AppOrRejBy", ResourceType = typeof(StringResources))]
            public Nullable<int> ApprovedOrRejectedBy { get; set; }

            [Display(Name = "AppOrRejOn", ResourceType = typeof(StringResources))]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public Nullable<System.DateTime> ApprovedOrRejectedOn { get; set; }

            [Display(Name = "Status", ResourceType = typeof(StringResources))]
            public string Status { get; set; }

            [Display(Name = "CreatedOn", ResourceType = typeof(StringResources))]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public Nullable<System.DateTime> CreatedOn { get; set; }

            [Display(Name = "ModifiedOn", ResourceType = typeof(StringResources))]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public Nullable<System.DateTime> ModifiedOn { get; set; }

            public virtual Employee Employee { get; set; }
            public virtual Employee Employee1 { get; set; }
            public virtual Employee Employee2 { get; set; }
        }
    }
}