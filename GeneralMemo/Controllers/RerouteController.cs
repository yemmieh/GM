using GeneralMemo.App_Code;
using GeneralMemo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeneralMemo.Controllers {
    public class RerouteController : Controller {
        // GET: Reroute

        private const string REROUTE_STATUS = "Rerouted";
        private string REROUTEDMSG          = "You have successfully rerouted the entry to ";
        public ActionResult Reroute( string WorkflowID , string ReportMode ){
            
            Session["ReportMode"] = ReportMode;

            string UserName = Session["UserName"] as string;
            if( UserName == null || UserName.Equals(String.Empty) ){
                return RedirectToAction("Login","Login",new {UserName=""});
            }

            RerouteModel rerouteModel    = new RerouteModel();
            if( TempData["rerouteModel"]!=null ){
                rerouteModel = TempData["rerouteModel"] as RerouteModel;
            } else {
                rerouteModel.WorkflowID         = WorkflowID;
                rerouteModel.EntryModel         = new LINQCalls().getWorkflowEntry( WorkflowID );
                rerouteModel.CurrentRequestStage= rerouteModel.EntryModel.RequestStage;          
            }   
            
            rerouteModel.NewRequestStage = SelectListItemHelper.GetRequestStages();              
            return View( rerouteModel );
        }
        
        [HttpPost]
        public ActionResult Reroute( string RerouteAction , RerouteModel rerouteModel ){

            //now resolve the user profile from AD and Xceed
            StaffADProfile staffADProfile = new StaffADProfile();
            staffADProfile.user_logon_name = Session["UserName"] as string;

            //AD
            ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery( staffADProfile );
            staffADProfile = activeDirectoryQuery.GetStaffProfile();
            if( staffADProfile==null ){
                ViewBag.ErrorMessage="Your profile is not properly setup on the system. Please contact InfoTech.";
                return View();
            }

           // staffADProfile = new LINQCalls().setInitiatorFields( staffADProfile );
           /*
            if( staffADProfile.branch_code==null ){
                ViewBag.ErrorMessage="Your profile is not properly setup for Target. Please contact Human Resources.";
                return View();
            }
*/
            staffADProfile.appperiod = "20150712";
            string _retVal = new AppDatabase().routeTargetEntries( rerouteModel , staffADProfile , "AppraisalDbConnectionString" );
            Debug.WriteLine(_retVal);

            if( _retVal!=null ){
                TempData["UploadComplete"]  = "false";
                ViewBag.ErrorMessage        = _retVal;
                TempData["rerouteModel"]    = rerouteModel;                        
            } else {
                int newstageid = Int32.Parse( rerouteModel.NewRequestStageCode );
                var approvers  = new LINQCalls().getApproverNames( rerouteModel.WorkflowID , newstageid );
                TempData["PostBackMessage"] = REROUTEDMSG;
                TempData["Approvers"] = string.Join("\\n", approvers.ToArray());
                return RedirectToAction("Reports","Reports",new { UserName=Session["UserName"] as string , ReportMode=Session["ReportMode"] as string});
            }
            
            return RedirectToAction("Reroute","Reroute",new { UserName=Session["UserName"] as string , ReportMode=Session["ReportMode"] as string});
        }

        public class SelectListItemHelper {
            internal static SelectList GetRequestStages(){                
                return new LINQCalls().getRequestStages();
            }
        }
    }
}