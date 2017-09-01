/**using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeneralMemo.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
    }
}*/

using GeneralMemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.DirectoryServices;

namespace GeneralMemo.Controllers { 
    public class LoginController : Controller {
        
        //[HttpGet]
        public ActionResult Login() {
            return this.View();
        }

        [HttpPost]
        public ActionResult Login( LoginModel model, string returnUrl) {
            
            /*if (!this.ModelState.IsValid) {
                return this.View(model);
            }*/
            try { 
                if ( Membership.ValidateUser(model.UserName, model.Password) ) {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (this.Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\")) {

                        Session["UserName"] = User.Identity.Name;
                        return this.Redirect(returnUrl);
                    }
                    Session["UserName"] = User.Identity.Name;
                    return this.RedirectToAction("AwaitingMyApproval", "AwaitingApproval");
                }
                this.ModelState.AddModelError(string.Empty, "The user name or password provided is incorrect.");
            } catch (Exception ex) {
                this.ModelState.AddModelError(string.Empty, ex.Message);
            }            
            return this.View(model);
        }

        public ActionResult LogOff() {
            FormsAuthentication.SignOut();
            bool checkApproverUser = false;
            ViewData["checkApproverUser"] = checkApproverUser; 

            return this.RedirectToAction("Index", "Home");
        }
    }
}