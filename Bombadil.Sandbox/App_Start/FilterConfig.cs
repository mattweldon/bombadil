﻿using System.Web;
using System.Web.Mvc;

namespace Bombadil.Sandbox
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}