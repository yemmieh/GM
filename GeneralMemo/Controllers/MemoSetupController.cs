using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeneralMemo.Models;
using GeneralMemo.App_Code;
using System.Diagnostics;
using System.Globalization;
using System.Data;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace GeneralMemo.Controllers {
    public class MemoSetupController : Controller {
        // GET: MemoSetup

        private const string INIT_STAGE         = "Initiate Memo";
        private const string BHEAD_STAGE 		= "Branch Head Approval";
	    private const string ZONAL_HEAD_STAGE   = "Zonal Head Approval";
	    private const string DEPT_HEAD_STAGE	= "Dept Head Approval";
	    private const string GROUP_HEAD_STAGE	= "Group Head Approval";
	    private const string GROUP_ZHEAD_STAGE  = "Group Zonal Head Approval";
	    private const string HR_UPLOAD 		    = "HR Upload";
        private const string FLOATING 		    = "Floating";
	    private const string DENIED 			= "Denied";
	    private const string APPROVED 		    = "Approved";

        private const string SAVED_STATUS       = "Saved";
        private const string SUBMIT_STATUS      = "Submitted";
        private const string DENIED_STATUS      = DENIED;

        private string SUMBMITTEDMSG    = "You have successfully submitted the memo for possible approval by:";
        private string APPROVEDMSG      = "You have successfully processed the memo for possible approval by:";
        private string UPLOADEDMSG      = "You have successfully approved the Memo";
        private string DENIEDMSG        = "You have successfully denied this Memo.\\nThe Memo will be sent for review by:";
          
        private string CANSAVE      = "true";

        private string _UserName    = "";

        public ActionResult Index() {
            return View();
        }



        [HttpGet]
        [Authorize]  
        //[ValidateInput(false)]
        public ActionResult NewMemo() {

            _UserName = User.Identity.Name;
            Session["UserName"] = User.Identity.Name;

            if( _UserName == null || _UserName.Equals(String.Empty)){
                ViewBag.ErrorMessage="You must be logged in to continue.";
                return RedirectToAction("AwaitingMyApproval","AwaitingApproval");
            } 

            MemoSetup memoSetup;

            if( TempData["memoSetup"]!=null ){

                memoSetup = TempData["memoSetup"] as MemoSetup;

            } else {

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

                staffADProfile.grade = new LINQCalls().getStaffGrade( staffADProfile.employee_number );

                memoSetup = new MemoSetup();
                memoSetup.OriginatorName    = staffADProfile.user_logon_name;
                memoSetup.OriginatorNumber  = staffADProfile.employee_number;

                memoSetup.WorkflowID        = Guid.NewGuid().ToString().ToUpper();
                memoSetup.EntryKey          = staffADProfile.employee_number+"_"+ DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + "_"+ staffADProfile.branch_code;
                memoSetup.RequestStageId    = 0;
                memoSetup.RequestStage      = INIT_STAGE;
                memoSetup.StaffADProfile    = staffADProfile;
                memoSetup.SignerDetailsList = new List<SignerDetails>();
                memoSetup.SignerDetails     = new SignerDetails();
                memoSetup.Branch            = staffADProfile.branch_name;
                memoSetup.BranchCode        = staffADProfile.branch_code;

                memoSetup.Signers           = String.Empty;
                memoSetup.CCFields          = String.Empty;

                memoSetup.DeptName          = staffADProfile.branch_name;
                memoSetup.DeptCode          = staffADProfile.branch_code;

                memoSetup.StaffADProfile    = staffADProfile;
                
                DateTime dt                 = DateTime.Today;
                memoSetup.DateOfMemo        = dt.ToString( "dddd MMMM d, yyyy", CultureInfo.CreateSpecificCulture("en-US") );
            }

            memoSetup.SignerDetailsList     =  memoSetup.SignerDetailsList.OrderBy(x => x.PayGroup_ID).ToList() ?? memoSetup.SignerDetailsList;

            /**
            //sort the list
            if ( memoSetup.SignerDetailsList!=null ) {
                var d = from x in memoSetup.SignerDetailsList orderby x.PayGroup_ID select x;
                memoSetup.SignerDetailsList = d.ToList();
            }**/

            Session["signerDetailsList"] = memoSetup.SignerDetailsList;
            ViewBag.editMode = TempData["editMode"] ?? true;

            if( TempData["ErrorMessage"]!=null ){
                ViewBag.ErrorMessage=TempData["ErrorMessage"] as string;
            }

            return View( memoSetup );
        }

        [HttpPost]
        [Authorize]        
        public ActionResult NewMemo( MemoSetup memoSetup , string MemoAction , string StaffNumber) {

            List<SignerDetails> signerDetails = memoSetup.SignerDetailsList ?? new List<SignerDetails>();

            //IEnumerable<RequestDetails> requestdetails = memoSetup.RequestDetails;
            //DataTable dataTable = DataHandlers.ToDataTable(requestdetails);
            string retVal="";

            switch ( MemoAction ) {
                case "AddSignatory":
                    //ADD the new Staff from the list
                    if( AddSignatory( memoSetup ) ){
                        signerDetails.Add( TempData["newSignatory"] as SignerDetails );
                    }
                    break;
                case "DeleteSignatory":
                    //DELETE the selected Staff from the list
                    if( DeleteSignatory( memoSetup , StaffNumber ) ){
                        signerDetails = TempData["signerDetails"] as List<SignerDetails>;
                    }                    
                    break;
                case "Reset":
                    // RESET all entries
                    memoSetup = null;
                    signerDetails = null;
                    break;
                case "Save":
                    memoSetup.Action = SAVED_STATUS;
                    memoSetup.UploadStatus = SAVED_STATUS;

                    retVal = new AppDatabase().saveMemo( memoSetup , "AppraisalDbConnectionString" , SAVED_STATUS );

                    if( retVal!=null ){
                        TempData["SaveComplete"] = "false";
                        TempData["ErrorMessage"] = retVal;
                    }
                    break;
                /*case "Deny":
                    // Good. let's send this f@*ker back
                    requestDetails.Select( c => { c.entry_key = c.employee_number+"_"+superInputTargetModel.StaffADProfile.appperiod +"_"+superInputTargetModel.StaffADProfile.branch_code; return c; } ).ToList();
                    requestDetails.Select( c => { c.workflowid = superInputTargetModel.WorkflowID; return c; } ).ToList();
                    
                    // SAVE the value to the DATABASE
                    requestdetails = superInputTargetModel.RequestDetails;
                    dataTable = DataHandlers.ToDataTable(requestdetails);

                    retVal = new AppDatabase().inputTargetEntries( dataTable , superInputTargetModel , "AppraisalDbConnectionString" , DENIED_STATUS );
                    Debug.WriteLine(retVal);

                    if( retVal!=null ){
                        TempData["UploadComplete"] = "false";
                        TempData["ErrorMessage"] = retVal;
                        TempData["superInputTargetModel"] = superInputTargetModel;
                    }else {
                        //String.format(SUMBMITTEDMSG)--add the approvers
                        var approvers = new LINQCalls().getApproverNames(superInputTargetModel.WorkflowID,-1);
                        TempData["PostBackMessage"] = DENIEDMSG;
                        TempData["Approvers"] = string.Join("\\n", approvers.ToArray());
                        return RedirectToAction( "AwaitingMyApproval","AwaitingApproval",new { UserName = Session["UserName"] as string } );
                    }
                    break;*/
                case "Submit":
                    //LET'S SUBMIT THIS SHIT

                    //Remove the current approver from the approver list
                    if( memoSetup.RequestStageId!=0 ){
                        memoSetup.SignerDetailsList.Remove(signerDetails.Single( s => s.ApproverStaffNumber.Equals(memoSetup.StaffADProfile.employee_number) ));
                        memoSetup.RequestStageId = 1;
                        if( memoSetup.SignerDetailsList.Count<=0 ){
                            memoSetup.RequestStageId = 2;
                        }
                    }
                    
                    memoSetup.Signers = getSigners( memoSetup );
                    memoSetup.Action = SUBMIT_STATUS;
                    memoSetup.UploadStatus = SUBMIT_STATUS;
                    
                    retVal = new AppDatabase().saveMemo( memoSetup , "AppraisalDbConnectionString" , SUBMIT_STATUS );                    
                    string[] retVals = retVal.Split('_');

                    if( retVals.Length>2 && retVals[2] != null ){
                        TempData["UploadComplete"]  = "false";
                        TempData["ErrorMessage"]    = retVal;
                        TempData["memoSetup"]       = memoSetup;                        
                    } else {
                        //Show the approvers                      
                        var approvers               = memoSetup.SignerDetailsList.Select( approver=>approver.ApproverStaffName ).ToArray();
                        TempData["PostBackMessage"] = SUMBMITTEDMSG;
                        TempData["Approvers"]       = string.Join("\\n", approvers);

                        return RedirectToAction( "AwaitingMyApproval","AwaitingApproval",new { UserName = Session["UserName"] as string } );
                    }
                    break;
            }

            memoSetup.SignerDetailsList = signerDetails; 
            TempData["memoSetup"] = memoSetup;
            return RedirectToAction( "NewMemo",new { UserName = Session["UserName"] as string } );
        }

        [Authorize]
        public ActionResult EditMemo( string WorkflowID , bool editMode , bool?myEntries ){

            _UserName = User.Identity.Name;
            Session["UserName"] = User.Identity.Name;

            if( _UserName == null || _UserName.Equals(String.Empty)){
                ViewBag.ErrorMessage="You must be logged in to continue.";
                return RedirectToAction("AwaitingMyApproval","AwaitingApproval");
            }

            //now resolve the user profile from AD and Xceed
            StaffADProfile staffADProfile = new StaffADProfile();
            staffADProfile.user_logon_name = _UserName;

            //staffADProfile.user_logon_name = "adebisi.olumoto";

            //AD
            ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery( staffADProfile );
            staffADProfile = activeDirectoryQuery.GetStaffProfile();
            staffADProfile.grade = new LINQCalls().getStaffGrade( staffADProfile.employee_number );     

            if( staffADProfile==null ){
                ViewBag.ErrorMessage="Your profile is not properly setup on the system. Please contact InfoTech.";
                return RedirectToAction( "AwaitingMyApproval","AwaitingApproval",new { UserName = Session["UserName"] as string } );
            }

            //Get the request identified by the Workflow ID
            MemoSetup memoSetup     = new MemoSetup();
            memoSetup               = new LINQCalls().getWorkflowEntry( WorkflowID );
            ViewBag.StaffBranch     = memoSetup.Branch;
            int requeststageid      = memoSetup.RequestStageId; 
            string requeststage     = memoSetup.RequestStage; 
            DateTime requestdate    = memoSetup.DateSubmitted;
            string initiatornumber  = new LINQCalls().getInitiatorNumber( WorkflowID ) ?? staffADProfile.employee_number;
          
            string cansave          = ( requeststage.Equals(INIT_STAGE) || requeststage.Equals(DENIED) ) && initiatornumber.Equals(staffADProfile.employee_number)
                                            ? this.CANSAVE : "false"; 

            XElement ApprovalHistory = new LINQCalls().getApprovalHistory( WorkflowID );
            XDocument  xDocument = DataHandlers.ToXDocument(ApprovalHistory);
            
            List<ApprovalDetails> approvalHistory = xDocument.Descendants("Approvals")
                .Select( det => new ApprovalDetails{
                                                        ApproverNames       = det.Element("ApproverName").Value,
                                                        ApproverStaffNumbers= det.Element("ApproverStaffNumber").Value,
                                                        ApprovedStages      = det.Element("ApprovedStage").Value,
                                                        ApproverAction      = det.Element("ApproverAction").Value,
                                                        ApprovalDateTime    = det.Element("ApprovalDateTime").Value,
                                                        ApproverComments    = det.Element("ApproverComment").Value
                                                    }).ToList();

            memoSetup.SignerDetailsList = GetApprovers( memoSetup );

            editMode = editMode && memoSetup.RequestStageId.Equals(1);

            TempData["editMode"]    = editMode; //( editMode==true ) ? null : "false";
            ViewBag.editMode        = editMode;

            if( TempData["ErrorMessage"]!=null ){
                ViewBag.ErrorMessage=TempData["ErrorMessage"] as string;
            }

            TempData["memoSetup"] = memoSetup;
            return RedirectToAction( "NewMemo",new { UserName = Session["UserName"] as string } );
        }

        private string getSigners( MemoSetup mSetup) {
            
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);     
            //tempo = regex.Replace(tempo, " ");

            List<SignerDetails> signerDetailsList = mSetup.SignerDetailsList;
            string signers = String.Empty;
            
            if( signerDetailsList!=null && signerDetailsList.Count>0 ){
                foreach (var signer in signerDetailsList){
                    signers += signer.ApproverStaffNumber+"$"+regex.Replace(signer.ApproverStaffName, " ")+"$"+signer.GradeID+"$"+signer.PayGroup_ID+"$"+signer.ApproverGrade+"$"+signer.ApproverDept+"|";
                }
                signers = signers.Remove(signers.Length - 1);
            }

            return signers;
        }

        public List<SignerDetails> GetApprovers( MemoSetup memoSetup ) {

            List<SignerDetails> signerDetails = new List<SignerDetails>();
            SignerDetails _signerDetails;
            string[] approvers_ = memoSetup.Signers.Split('|');
            foreach (string str in approvers_) {

                _signerDetails = new SignerDetails();
                string[] appr_ = str.Split('$');

                _signerDetails.ApproverStaffNumber  = appr_[0];
                _signerDetails.ApproverStaffName    = appr_[1];
                _signerDetails.GradeID              = Int32.Parse(appr_[2]);
                _signerDetails.PayGroup_ID          = Int32.Parse(appr_[3]);
                _signerDetails.ApproverGrade        = appr_[4];
                _signerDetails.ApproverDept         = appr_[5];
                signerDetails.Add(_signerDetails);
            }

            return signerDetails;
        }

        private bool AddSignatory( MemoSetup memoSetup ) {

            bool flag = true;
            string ApproverStaffName    = memoSetup.SignerDetails.ApproverStaffName;
            string ApproverStaffNumber  = memoSetup.SignerDetails.ApproverStaffNumber;
            string ApproverGrade        = memoSetup.SignerDetails.ApproverGrade;
            string ApproverDept         = memoSetup.SignerDetails.ApproverDept;
            int? GradeID                = memoSetup.SignerDetails.GradeID;
            int? PayGroup_ID            = memoSetup.SignerDetails.PayGroup_ID;

            /**Check for null values**/
            if( String.IsNullOrEmpty(ApproverStaffName) || String.IsNullOrEmpty(ApproverStaffNumber) 
                    || String.IsNullOrEmpty(ApproverGrade) || String.IsNullOrEmpty(ApproverDept) || GradeID==null 
                        || PayGroup_ID==null ){
                TempData["ErrorMessage"]="Please select a valid staff";
                flag = false;
            } else { 
                /**Check if the staff already exists in the list**/
                if( memoSetup.SignerDetailsList!=null && memoSetup.SignerDetailsList.Count>0 ) {
                    bool dupStaff = memoSetup.SignerDetailsList.Exists( x=>x.ApproverStaffNumber.ToUpper().Contains(ApproverStaffNumber) );
                    if( dupStaff ){
                        TempData["ErrorMessage"]="Staff already exists in the list";
                        flag = false;
                    }
                }

                try { 
                    SignerDetails signerDetails       = new SignerDetails();
                    signerDetails.ApproverStaffName   = ApproverStaffName;
                    signerDetails.ApproverStaffNumber = ApproverStaffNumber; 
                    signerDetails.ApproverGrade       = ApproverGrade;
                    signerDetails.ApproverDept        = ApproverDept; 
                    signerDetails.GradeID             = GradeID;
                    signerDetails.PayGroup_ID         = PayGroup_ID;
                        
                    TempData["newSignatory"] = signerDetails;
                    
                } catch ( Exception ex ) {
                    TempData["ErrorMessage"]=ex.Message;
                    flag = false;
                }
            }
            return flag;
        }

        private bool DeleteSignatory( MemoSetup memoSetup , string StaffNumber ) {

            bool flag = true;
            /**Check for null values**/
            if( String.IsNullOrEmpty(StaffNumber) ){
                TempData["ErrorMessage"]="Please select an entry to delete";
                flag = false;
            }else{
                /**delete the staff from the list**/
                try { 
                    List<SignerDetails> signerDetails = memoSetup.SignerDetailsList;
                    signerDetails.Remove(signerDetails.Single( s => s.ApproverStaffNumber.Equals(StaffNumber) ));
                    TempData["signerDetails"] = signerDetails;
                } catch (Exception ex) {
                    TempData["ErrorMessage"]=ex.Message;
                    flag = false;
                }
            }
            return flag;
        }

        public JsonResult TagSearch(string term) {

            CultureInfo ci =  new CultureInfo( "en-US" );

            List<SignerDetails> signerDetails = new LINQCalls().getBankStaff();
            var staffmembers = new System.Web.Script.Serialization.JavaScriptSerializer();
            Debug.WriteLine(signerDetails);
            return this.Json( signerDetails.Where( t => t.ApproverStaffName.ToLower().Contains(term.ToLower()) ),JsonRequestBehavior.AllowGet );
            }
    }


}