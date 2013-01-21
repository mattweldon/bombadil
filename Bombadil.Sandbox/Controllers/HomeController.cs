using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bombadil.Core;
using Bombadil.Core.Managers;

namespace Bombadil.Sandbox.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(VaryByParam = "title", Duration = 180)]
        public ActionResult Post(string title)
        {
            ContentManager contentManager = new ContentManager(ContentType.Post);

            return View(contentManager.Get(title));
        }

        [OutputCache(VaryByParam = "title", Duration = 180)]
        public ActionResult Page(string title)
        {
            ContentManager contentManager = new ContentManager(ContentType.Page);

            return View(contentManager.Get(title));
        }

    }
}
