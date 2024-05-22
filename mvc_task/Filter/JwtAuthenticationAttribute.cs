using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace mvc_task.CustomeModel
{

    public class JwtAuthenticationAttribute : ActionFilterAttribute
    {
        private static readonly string SecretKey = "UHJTFRTYUY787FVGHMJYAERvlkuytnbf";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var token = request.Cookies["jwt"].Value;

            if (token != null)
            {
                var userName = JwtHelper.ValidateToken(token);
                if (userName == null)
                {
                    filterContext.Result = new HttpStatusCodeResult(401, "No Username found.");
                }
            }
            else
            {
                filterContext.Result = new HttpStatusCodeResult(401, "Token Null");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}

