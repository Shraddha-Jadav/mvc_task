﻿using mvc_task.CustomeModel;
using System.Web;
using System.Web.Mvc;

namespace mvc_task
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
