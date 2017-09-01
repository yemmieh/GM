using GeneralMemo.App_Code;
using GeneralMemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeneralMemo.Controllers {
    public class ApprovedByMeController : Controller{
        // GET: ApprovedByMe

        //private string UserID = "";
        private string _UserName="";
        private LogWriter logWriter;
        
        [Authorize]
        public ActionResult ApprovedByMe( ){
            /**First let's check if the PostBackMessage has something
             * Very important---DO NOT DELETE!!!!!!!!!!!!!!!!!!!!!**/
            string PostBackMessage = TempData["PostBackMessage"] as string;
            string Approvers = TempData["Approvers"] as string;
            
            _UserName = User.Identity.Name;
            Session["UserName"] = _UserName;

            this.logWriter = new LogWriter();
            try {
                logWriter.WriteErrorLog(string.Format("about to PostBackMessage : Exception!!! / {0}", "Posted back")); 

                if (!String.IsNullOrEmpty(PostBackMessage)) {
                    logWriter.WriteErrorLog(string.Format("PostBackMessage Status : Exception!!! / {0}", "Posted back")); 

                    ViewBag.PostBackMessage = string.Format("<script type='text/javascript'>alert(\"" + PostBackMessage + "\\n\\n{0}\");</script>", Approvers);
                }
                logWriter.WriteErrorLog(string.Format("After post back : Exception!!! / {0}", "Posted back")); 

                //now get the pending items
                if (_UserName == null || _UserName.Equals(String.Empty))
                {
                    ViewBag.ErrorMessage = "You must be logged in to continue.";
                    return View();
                }
               

                //now resolve the user profile from AD and Xceed
                StaffADProfile staffADProfile = new StaffADProfile();
                staffADProfile.user_logon_name = _UserName;

                //AD
                ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);
                staffADProfile = activeDirectoryQuery.GetStaffProfile();
                if (staffADProfile == null) {
                    ViewBag.ErrorMessage = "Your profile is not properly setup on the system. Please contact InfoTech.";
                    return View();
                }
                //Check if the approver has processed an entry for the AppraisalPeriod from the Database
                logWriter.WriteErrorLog(string.Format("Get Approved : about firing getMyApprovedTargetWorkflows!!! / {0}", "")); 

                List<EntriesModel> entryDetails = new List<EntriesModel>();
                entryDetails = new LINQCalls().getMyApprovedTargetWorkflows(staffADProfile);
                
                return View(entryDetails);
            }
            catch(Exception ex) {
                logWriter.WriteErrorLog(string.Format("ApprovedByMe : Exception!!! / {0}", ex.Message)); 
                return View();
            }
        }
    }
}