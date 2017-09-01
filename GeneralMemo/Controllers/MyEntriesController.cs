using GeneralMemo.App_Code;
using GeneralMemo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeneralMemo.Controllers
{
    public class MyEntriesController : Controller {

        private const string MARKETING  = "MARKETING";
        private const string HOBRCODE   = "001";
        private const string OTHERS     = "OTHERS";
        private const string NA         = "NA";

        private string _UserName = "";
        // GET: MyEntries
        public ActionResult MyEntries() {

            _UserName = User.Identity.Name;
            Session["UserName"] = User.Identity.Name;

            if( _UserName == null || _UserName.Equals(String.Empty)){
                ViewBag.ErrorMessage="You must be logged in to continue.";
                return RedirectToAction("AwaitingMyApproval","AwaitingApproval");
            }

            Session["UserName"] = _UserName;

            //now resolve the user profile from AD and Xceed
            StaffADProfile staffADProfile = new StaffADProfile();
            staffADProfile.user_logon_name = _UserName;           


            //AD
            ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery( staffADProfile );
            staffADProfile = activeDirectoryQuery.GetStaffProfile();
            if( staffADProfile==null ){
                ViewBag.ErrorMessage="Your profile is not properly setup on the system. Please contact InfoTech.";
                return RedirectToAction( "AwaitingMyApproval","AwaitingApproval",new { UserName = Session["UserName"] as string } );
            }

            ViewBag.AppID=DataHandlers.APP_ID;
            ViewBag.StaffBranch = staffADProfile.branch_name; //+ ( ( staffADProfile.branch_code.Equals(HOBRCODE) ) ? " | " + staffADProfile.hodeptcode : String.Empty );
            
            //Check if the initiator/branch/has an existing entry for the AppraisalPeriod from the Database
            List<MyMemoEntriesModel> entryDetails =  new List<MyMemoEntriesModel>();
            entryDetails = new LINQCalls().getMyMemoWorkflows( staffADProfile );

            return View( entryDetails );
        }

        /*public ActionResult OpenTargetEntry( string WorkflowID , int RequestStageID) {

            //string UserName = Request.LogonUserIdentity.Name.Substring(Request.LogonUserIdentity.Name.LastIndexOf(@"\") + 1);
            string UserName = @User.Identity.Name;

            if( UserName == null || UserName.Equals(String.Empty)){
                ViewBag.ErrorMessage="You must be logged in to continue.";
                return RedirectToAction("AwaitingMyApproval","AwaitingApproval");
            }

            //now resolve the user profile from AD and Xceed
            StaffADProfile staffADProfile = new StaffADProfile();
            staffADProfile.user_logon_name = UserName;
          

            //AD
            ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery( staffADProfile );
            staffADProfile = activeDirectoryQuery.GetStaffProfile();
            if( staffADProfile==null ){
                ViewBag.ErrorMessage="Your profile is not properly setup on the system. Please contact InfoTech.";
                return RedirectToAction( "AwaitingMyApproval","AwaitingApproval",new { UserName = Session["UserName"] as string } );
            }
            
            string approvers = new LINQCalls().getApprovers( WorkflowID,RequestStageID );
            List<SignerDetails> realApprovers = new DataHandlers().GetApprovers( approvers );
            int index = realApprovers.FindIndex(f => f.ApproverStaffNumber == staffADProfile.employee_number);
            bool isApprover = ( index  >= 0 ) ? true : false;

            return RedirectToAction( "EditTarget","InputTarget",new {WorkflowID=WorkflowID , editMode =isApprover  , myEntries=true } );
        }*/
        public ActionResult OpenMemoEntry( string WorkflowID , int RequestStageID) {

            //string UserName = Request.LogonUserIdentity.Name.Substring(Request.LogonUserIdentity.Name.LastIndexOf(@"\") + 1);
            string UserName = @User.Identity.Name;

            if( UserName == null || UserName.Equals(String.Empty)){
                ViewBag.ErrorMessage="You must be logged in to continue.";
                return RedirectToAction("AwaitingMyApproval","AwaitingApproval");
            }

            //now resolve the user profile from AD and Xceed
            StaffADProfile staffADProfile = new StaffADProfile();
            staffADProfile.user_logon_name = UserName;
          

            //AD
            ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery( staffADProfile );
            staffADProfile = activeDirectoryQuery.GetStaffProfile();
            if( staffADProfile==null ){
                ViewBag.ErrorMessage="Your profile is not properly setup on the system. Please contact InfoTech.";
                return RedirectToAction( "AwaitingMyApproval","AwaitingApproval",null );
            }
            
            string approvers = new LINQCalls().getApprovers( WorkflowID,RequestStageID );
            List<SignerDetails> realApprovers = new DataHandlers().GetApprovers( approvers );
            int index = realApprovers.FindIndex(f => f.ApproverStaffNumber == staffADProfile.employee_number);
            bool isApprover = ( index  >= 0 ) ? true : false;

            return RedirectToAction( "EditMemo","MemoSetup",new {WorkflowID=WorkflowID , editMode =isApprover , myEntries=true } );
        }

        public ActionResult GetApprovers( string WorkflowID , int RequestStageID ) {

            string errorResult = "{{\"employee_number\":\"{0}\",\"name\":\"{1}\"}}";
            if( string.IsNullOrEmpty( WorkflowID ) ) {
                errorResult = string.Format(errorResult , "Error" , "Invalid entry detected");        
                return Content(errorResult, "application/json");
            }

            string approvers = new LINQCalls().getMemoApproverNames(WorkflowID,RequestStageID);
            if( approvers==null || approvers=="" ){
                errorResult = string.Format(errorResult , null , "No approvers found for the entry");        
                return Content(errorResult, "application/json");
            } else {
                //20090022$ESOGBUE IKECHUKWU P$14$205$SEA|20110326$KALU NNANNA I$15$248$EA
                List<SignerDetails> signerDetails = new List<SignerDetails>();
                SignerDetails _signerDetails;
                string[] approvers_ = approvers.Split('|');
                foreach (string str in approvers_) {

                    _signerDetails = new SignerDetails();
                    string[] appr_ = str.Split('$');

                    _signerDetails.ApproverStaffNumber  = appr_[0];
                    _signerDetails.ApproverStaffName    = appr_[1];
                    _signerDetails.GradeID              = Int32.Parse(appr_[2]);
                    _signerDetails.PayGroup_ID          = Int32.Parse(appr_[3]);
                    _signerDetails.ApproverGrade        = appr_[4];
                    signerDetails.Add(_signerDetails);
                } 

                return Json( signerDetails.ToArray() , JsonRequestBehavior.AllowGet );
            }
        }
    }
}