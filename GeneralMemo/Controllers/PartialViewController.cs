using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeneralMemo.Controllers {
    public class PartialViewController : Controller {
        // GET: PartialView
        public ActionResult ApprovalHistory(  ) {
            return View();
        }
        public ActionResult AuditHistory() {
            return View();
        }
    }
}