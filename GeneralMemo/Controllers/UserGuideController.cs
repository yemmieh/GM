using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeneralMemo.Controllers
{
    public class UserGuideController : Controller
    {
        // GET: UserGuide
        public ActionResult Index()
        {
            return View();
        }
    }
}