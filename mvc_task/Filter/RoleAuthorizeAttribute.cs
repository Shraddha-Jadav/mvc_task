using mvc_task.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace mvc_task.Filter
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedDepartmentIds;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            allowedDepartmentIds = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = httpContext.User;
            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                var username = user.Identity.Name;
                using (var dbContext = new shraddha_crmEntities2())
                {
                    string deptName;
                    var userDepartmentId = dbContext.Employees
                        .Where(e => e.Email == username)
                        .Select(e => e.DepartmentId)
                        .FirstOrDefault();

                    deptName = userDepartmentId == 1 ? "Employee" : userDepartmentId == 2 ? "Manager" : "Director";
                    return allowedDepartmentIds.Contains(deptName);
                }
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                Console.WriteLine("invalid");
                //user is not authenticated
                filterContext.Result = new HttpUnauthorizedResult();
                //filterContext.Result = new RedirectToRouteResult(
                //new System.Web.Routing.RouteValueDictionary(
                //new { controller = "Authentication", action = "Login" }
                //        ));
            }
            else
            {
                //user is authenticated but not authorized
                filterContext.Result = new RedirectResult("~/Error/Unauthorized.html");
            }
        }
    }
}
