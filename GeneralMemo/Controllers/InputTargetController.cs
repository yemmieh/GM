using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeneralMemo.Models;
using GeneralMemo.App_Code;
using System.Diagnostics;
using System.Reflection;
using System.Data;
using System.Web.SessionState;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;

namespace GeneralMemo.Controllers{
    public class InputTargetController : Controller{

        private const string MARKETING  = "MARKETING";
        private const string HOBRCODE   = "001";
        private const string OTHERS     = "OTHERS";
        private const string NA         = "NA";

        private const string INIT_STAGE         = "Initiate Target";
        private const string BHEAD_STAGE 		= "Branch Head Approval";
	    private const string ZONAL_HEAD_STAGE   = "Zonal Head Approval";
	    private const string DEPT_HEAD_STAGE	= "Dept Head Approval";
	    private const string GROUP_HEAD_STAGE	= "Group Head Approval";
	    private const string GROUP_ZHEAD_STAGE  = "Group Zonal Head Approval";
	    private const string HR_UPLOAD 		    = "HR Upload";
	    private const string DENIED 			= "Denied";
	    private const string APPROVED 		    = "Approved";

        private const string SAVED_STATUS       = "Saved";
        private const string SUBMIT_STATUS      = "Submitted";
        private const string DENIED_STATUS      = DENIED;

        private string SUMBMITTEDMSG= "You have successfully submitted your request for possible approval by:";
        //private string APPROVEDMSG  = "You have successfully approved this target setup.";
        //private string UPLOADEDMSG  = "You have successfully uploaded the target setup.";
        private string DENIEDMSG    = "You have successfully denied this target for review by:";

        private string CANSAVE      = "true";


        // GET: InputTarget
        [HttpGet]
        [Authorize]        
        public ActionResult TargetInputForm( string UserName ){

            if( UserName == null || UserName.Equals(String.Empty)){
                ViewBag.ErrorMessage="You must be logged in to continue.";
                return RedirectToAction("AwaitingMyApproval","AwaitingApproval");
            }            

            SuperInputTargetModel superInputTargetModel = new SuperInputTargetModel();

            if( TempData["superInputTargetModel"]!=null ){

                superInputTargetModel = TempData["superInputTargetModel"] as SuperInputTargetModel;

            } else {
                //now resolve the user profile from AD and Xceed
                StaffADProfile staffADProfile = new StaffADProfile();
                staffADProfile.user_logon_name = UserName;

              //  staffADProfile.user_logon_name = "ADAMU.LAWANI";              

                //AD
                ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery( staffADProfile );
                staffADProfile = activeDirectoryQuery.GetStaffProfile();
                if( staffADProfile==null ){
                    ViewBag.ErrorMessage="Your profile is not properly setup on the system. Please contact InfoTech.";
                    return RedirectToAction( "AwaitingMyApproval","AwaitingApproval",new { UserName = Session["UserName"] as string } );
                }

                //Appraisal Initiaor Setup
                //Resolve the --branchname --branchcode --department --deptcode --appperiod from Tb_TargetInitiators table
                staffADProfile = new LINQCalls().setInitiatorFields( staffADProfile );
                if( staffADProfile.branch_code==null ){
                    ViewBag.ErrorMessage="Your profile is not properly setup for Target. Please contact Human Resources.";
                    return RedirectToAction( "AwaitingMyApproval","AwaitingApproval",new { UserName = Session["UserName"] as string } );
                }

                ViewBag.StaffBranch = staffADProfile.branch_name + ( ( staffADProfile.branch_code.Equals(HOBRCODE) ) ? " | " + staffADProfile.hodeptcode : String.Empty );
                ViewBag.StaffNumber = staffADProfile.employee_number;
                //Check if the initiator/branch/has an existing entry for the AppraisalPeriod
                List<RequestDetails> requestDetails =  new List<RequestDetails>();               

                if (staffADProfile.branch_code != "001") {
                    requestDetails = new LINQCalls().getExistingTargetEntry(staffADProfile);
                } else {
                    ////staffprofile ho_staff_pro = new staffprofile();
                    //// ho_staff_pro = new LINQCalls().getProfile(staffADProfile.employee_number);
                     requestDetails = new LINQCalls().getExistingHOTargetEntry(staffADProfile);
                }
                

                //Great. Everything looks okay."
                //Now let's get the staff reporting to you--using branchcode and deptcode(HO only)
                //see if the requestdetails entry has anything
                if ( requestDetails.Count<=0 ){
                    requestDetails = ( staffADProfile.branch_code.Equals(HOBRCODE) ) ? new LINQCalls().getMarketingStaff_HO( staffADProfile ): new LINQCalls().getMarketingStaff_Branch( staffADProfile );
                } else {
                    //OH dear:-) You go some shit keyed in before now---we're gonna blast u to My Entries view--hold tight
                    //return RedirectToAction("MyEntries",new { UserName=UserName } );
                    return RedirectToAction("MyEntries","MyEntries",new { UserName=UserName } );
                }

                Debug.WriteLine( requestDetails );
                /**Now lets see if the list contains the others entry***/
                //if others is not found, add
                //if others exists, then send it to the bottom of the stack
                if ( !requestDetails.Exists( x=>x.name.ToUpper().Contains(OTHERS) ) ){
                    RequestDetails os= new RequestDetails();
                    os.employee_number
                                = NA;
                    os.name     = OTHERS; 
                    os.grade    = NA;
                    requestDetails.Add(os);
                }

                Debug.WriteLine(Session.SessionID.ToString().ToUpper());
                string workflowid       = ( requestDetails.ElementAt(0).workflowid.Equals(String.Empty) ) ? Guid.NewGuid().ToString().ToUpper() : requestDetails.ElementAt(0).workflowid; 
                //string workflowid     = ( requestDetails.ElementAt(0).workflowid.Equals(String.Empty) ) ? Session.SessionID.ToString().ToUpper() : requestDetails.ElementAt(0).workflowid; 
                int requeststageid      = requestDetails.ElementAt(0).requeststageid; 
                string requeststage     = requestDetails.ElementAt(0).requeststage ?? String.Empty; 
                DateTime requestdate    = requestDetails.ElementAt(0).requestdate;
                string initiatornumber  = new LINQCalls().getInitiatorNumber( workflowid ) ?? staffADProfile.employee_number;

                string cansave          = ( requeststage.Equals(INIT_STAGE) || requeststage.Equals(DENIED) ) && initiatornumber.Equals(staffADProfile.employee_number)
                                            ? this.CANSAVE : "false"; 

                superInputTargetModel = new SuperInputTargetModel{
                                                                    WorkflowID      = workflowid,
                                                                    RequestStageID  = requeststageid,
                                                                    RequestStage    = requeststage,
                                                                    RequestDate     = requestdate,
                                                                    CanSave         = cansave,
                                                                    StaffADProfile  = staffADProfile,
                                                                    RequestDetails  = requestDetails,
                                                                    EntryModel      = null,
                                                                    RequestBranch   = staffADProfile.branch_name,
                                                                    RequestBranchCode=staffADProfile.branch_code
                                                                 };

                /*if( TempData["superInputTargetModel"]!=null ){
                    superInputTargetModel = TempData["superInputTargetModel"] as SuperInputTargetModel;
                } else {
                    superInputTargetModel = new SuperInputTargetModel{
                                                                        WorkflowID      = workflowid,
                                                                        RequestStageID  = requeststageid,
                                                                        RequestStage    = requeststage,
                                                                        RequestDate     = requestdate,
                                                                        StaffADProfile  = staffADProfile,
                                                                        RequestDetails  = requestDetails,
                                                                        EntriesModel    = null
                                                                     };
                }*/
            }

            //sort the list
            var d = from x in superInputTargetModel.RequestDetails 
                    orderby x.name == OTHERS, x.name
                    select x;
            superInputTargetModel.RequestDetails = d.ToList();

            Session["requestDetails"] = superInputTargetModel.RequestDetails;
            Session["UserName"] = UserName;

            if( TempData["ErrorMessage"]!=null ){
                ViewBag.ErrorMessage=TempData["ErrorMessage"] as string;
            }
            
            superInputTargetModel.StaffADProfile.in_StaffName="";
            superInputTargetModel.StaffADProfile.in_StaffNumber="";
            superInputTargetModel.StaffADProfile.in_StaffGrade="";

            return View( superInputTargetModel );
        }


        private bool AddStaff( SuperInputTargetModel superInputTargetModel ) {

            bool flag = true;
            string staff_name   = superInputTargetModel.StaffADProfile.in_StaffName;
            string staff_id     = superInputTargetModel.StaffADProfile.in_StaffNumber;
            string staff_grade  = superInputTargetModel.StaffADProfile.in_StaffGrade;

            /**Check for null values**/
            if( String.IsNullOrEmpty(staff_id) || String.IsNullOrEmpty(staff_name) || String.IsNullOrEmpty(staff_grade) ){
                TempData["ErrorMessage"]="Please provide a valid staff number";
                flag = false;
            } else { 
                /**Check if the staff already exists in the list**/
                bool dupStaff = superInputTargetModel.RequestDetails.Exists( x=>x.employee_number.ToUpper().Contains(staff_id) );
                if( dupStaff ){
                    TempData["ErrorMessage"]="Staff already exists in the list";
                    flag = false;
                } else {
                    /**now add the staff to the model**/
                    try { 
                        RequestDetails newStaff= new RequestDetails();
                        newStaff.employee_number
                                            = staff_id;
                        newStaff.name       = staff_name; 
                        newStaff.grade      = staff_grade;
                        TempData["newStaff"] = newStaff;
                    
                    } catch ( Exception ex ) {
                        TempData["ErrorMessage"]=ex.Message;
                        flag = false;
                    }
                }    
            }
            return flag;
        }

        private bool DeleteStaff( SuperInputTargetModel superInputTargetModel , string StaffNumber ) {

            bool flag = true;

            /**Check for null values**/
            if( String.IsNullOrEmpty(StaffNumber) ){
                TempData["ErrorMessage"]="Please select an entry to delete";
                flag = false;
            }else{
                /**delete the staff from the list**/
                try { 
                    List<RequestDetails> requestDetails = superInputTargetModel.RequestDetails;
                    requestDetails.Remove(requestDetails.Single( s => s.employee_number.Equals(StaffNumber) ));
                    //superInputTargetModel.RequestDetails = requestDetails;
                    TempData["requestDetails"] = requestDetails;
                } catch (Exception ex) {
                    TempData["ErrorMessage"]=ex.Message;
                    flag = false;
                }
            }
            return flag;
        }

        [HttpPost]
        [Authorize]        
        public ActionResult TargetInputForm( SuperInputTargetModel superInputTargetModel , string TargetAction , string StaffNumber) {

            List<RequestDetails> requestDetails = superInputTargetModel.RequestDetails;

            IEnumerable<RequestDetails> requestdetails = superInputTargetModel.RequestDetails;
            DataTable dataTable = DataHandlers.ToDataTable(requestdetails);
            string retVal="";

            switch ( TargetAction ) {
                case "AddStaff":
                    //ADD the new Staff from the list
                    if( AddStaff( superInputTargetModel ) ){
                        requestDetails.Add( TempData["newStaff"] as RequestDetails );
                    }
                    break;
                case "DeleteStaff":
                    //DELETE the selected Staff from the list
                    if( DeleteStaff( superInputTargetModel , StaffNumber ) ){
                        requestDetails = TempData["requestDetails"] as List<RequestDetails>;
                    }                    
                    break;
                case "Reset":
                    // RESET all entries to ZERO
                    requestDetails.Select( c => { c.cabal = c.cabal_l = c.sabal = c.sabal_l = c.fd = c.fx =c.inc = c.inc_l = c.rv = "0"; return c; } ).ToList();
                    break;
                case "Save":
                    // GOSH!!! I CAN'T HIDE THE entry_key & workflow_id COLUMN---REBUILD IT AGAIN
                    requestDetails.Select( c => { c.entry_key = c.employee_number+"_"+superInputTargetModel.StaffADProfile.appperiod +"_"+superInputTargetModel.StaffADProfile.branch_code; return c; } ).ToList();
                    requestDetails.Select( c => { c.workflowid = superInputTargetModel.WorkflowID; return c; } ).ToList();
                    
                    // SAVE the value to the DATABASE
                    requestdetails = superInputTargetModel.RequestDetails;
                    dataTable = DataHandlers.ToDataTable(requestdetails);

                    retVal = new AppDatabase().inputTargetEntries( dataTable , superInputTargetModel , "AppraisalDbConnectionString" , SAVED_STATUS );
                    Debug.WriteLine(retVal);

                    if( retVal!=null ){
                        TempData["UploadComplete"] = "false";
                        TempData["ErrorMessage"] = retVal;
                        TempData["superInputTargetModel"] = superInputTargetModel;
                    } else {
                        return RedirectToAction("MyEntries","MyEntries",new {UserName=Session["UserName"]});
                    }
                    break;
                case "Deny":
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
                    break;
                case "Submit":
                    //LET'S SUBMIT THIS SHIT
                    requestDetails.Select( c => { c.entry_key = c.employee_number+"_"+superInputTargetModel.StaffADProfile.appperiod +"_"+superInputTargetModel.StaffADProfile.branch_code; return c; } ).ToList();
                    requestDetails.Select( c => { c.workflowid = superInputTargetModel.WorkflowID; return c; } ).ToList();          
                    // SAVE the value to the DATABASE
                    IEnumerable<RequestDetails> _requestdetails = superInputTargetModel.RequestDetails;
                    if( superInputTargetModel.EntryModel!=null ){
                        //EDITTED REQUEST---APPROVAL OR RESUBMISSION
                        requestDetails.Select( c => { c.requeststageid = superInputTargetModel.RequestStageID; c.requeststage=superInputTargetModel.RequestStage; return c; } ).ToList();
                    }
                    DataTable _dataTable = DataHandlers.ToDataTable( _requestdetails );
                   
                    StaffADProfile staffADProfile = new StaffADProfile();
                    staffADProfile.branch_code = new LINQCalls().getEntryProfile(superInputTargetModel.WorkflowID).branch_code;
                    staffADProfile.branch_name = new LINQCalls().getEntryProfile(superInputTargetModel.WorkflowID).branch_name;
                    string _retVal = new AppDatabase().inputTargetEntries( _dataTable , superInputTargetModel , "AppraisalDbConnectionString" , SUBMIT_STATUS );
                    
                    Debug.WriteLine(_retVal);

                    if( _retVal!=null ){
                        TempData["UploadComplete"]          = "false";
                        TempData["ErrorMessage"]            = _retVal;
                        TempData["superInputTargetModel"]   = superInputTargetModel;                        
                    } else {
                        //String.format(SUMBMITTEDMSG)--add the approvers
                        int newstageid=0;
                        switch (superInputTargetModel.RequestStageID) {
                            case 0:
                                newstageid=3;
                                break;
                            case -1:
                                newstageid=3;
                                break;
                            case 3:
                                newstageid=20;
                                break;
                            case 20:
                                newstageid=100;
                                break;    
                        }

                        /*EntryModel EntryModel = new LINQCalls().getWorkflowEntry( superInputTargetModel.WorkflowID );                        
                        var approvers = new LINQCalls().getApproverNames(superInputTargetModel.WorkflowID,EntryModel.RequestStageId);*/
                        var approvers = new LINQCalls().getApproverNames(superInputTargetModel.WorkflowID,newstageid);
                        TempData["PostBackMessage"] = SUMBMITTEDMSG;
                        TempData["Approvers"] = string.Join("\\n", approvers.ToArray());
                        return RedirectToAction( "AwaitingMyApproval","AwaitingApproval",new { UserName = Session["UserName"] as string } );
                    }
                    break;
            }

            superInputTargetModel.RequestDetails = requestDetails; 
            TempData["superInputTargetModel"] = superInputTargetModel;
            return RedirectToAction( "TargetInputForm",new { UserName = Session["UserName"] as string } );
        }

        [Authorize]
        public ActionResult EditTarget( string UserName , string WorkflowID , bool editMode , bool?myEntries ){

          //  editMode = true;
            if( UserName == null || UserName.Equals(String.Empty)){
                ViewBag.ErrorMessage="You must be logged in to continue.";
                return RedirectToAction("AwaitingMyApproval","AwaitingApproval");
            }

            //now resolve the user profile from AD and Xceed
            StaffADProfile staffADProfile = new StaffADProfile();
            staffADProfile.user_logon_name = UserName;

            //staffADProfile.user_logon_name = "adebisi.olumoto";

            //AD
            ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery( staffADProfile );
            staffADProfile = activeDirectoryQuery.GetStaffProfile();
            //GET ENTERY BRANC CODE

            staffADProfile.branch_code = new LINQCalls().getEntryProfile(WorkflowID).branch_code;
            staffADProfile.branch_name = new LINQCalls().getEntryProfile(WorkflowID).branch_name;

            if( staffADProfile==null ){
                ViewBag.ErrorMessage="Your profile is not properly setup on the system. Please contact InfoTech.";
                return RedirectToAction( "AwaitingMyApproval","AwaitingApproval",new { UserName = Session["UserName"] as string } );
            }     
       
            //GET THE APPROVERS DETAILS FROM EXCEED //Approver Setup
            //Resolve the --branchname --branchcode --department --deptcode
            /*
            staffADProfile = new LINQCalls().setInitiatorFields( staffADProfile );
            if( staffADProfile.branch_code==null ){
                ViewBag.ErrorMessage="Your profile is not properly setup on Exceed. Please contact Human Resources.";
                return RedirectToAction( "AwaitingMyApproval","AwaitingApproval",new { UserName = Session["UserName"] as string } );
            }
             * */

            //Get the request identified by the workflow id
            List<RequestDetails> requestDetails =  new List<RequestDetails>();
            if( myEntries!=null && myEntries==true ){
                string entrykey = staffADProfile.employee_number+"_"+staffADProfile.appperiod +"_"+staffADProfile.branch_code;
                requestDetails = new LINQCalls().getExistingTargetEntry( WorkflowID , staffADProfile.employee_number , entrykey );
            } else {
                requestDetails = new LINQCalls().getExistingTargetEntry( WorkflowID , staffADProfile.employee_number );
            }
            

            EntryModel entryModel =  new EntryModel();
            entryModel = new LINQCalls().getWorkflowEntry( WorkflowID );
            ViewBag.StaffBranch = entryModel.Branch;
            int requeststageid  = entryModel.RequestStageId; 
            string requeststage = entryModel.RequestStage; 
            DateTime requestdate= entryModel.DateSubmitted;
            string initiatornumber  = new LINQCalls().getInitiatorNumber( WorkflowID ) ?? staffADProfile.employee_number;


            //staffADProfile.branch_code = new LINQCalls().getProfile(staffADProfile.employee_number).branch_code;
            //if (requeststage.Equals(INIT_STAGE))
            //{
             //   staffADProfile.branch_code = new LINQCalls().setInitiatorFields(staffADProfile).branch_code;
            //}
          

            string cansave          = ( requeststage.Equals(INIT_STAGE) || requeststage.Equals(DENIED) ) && initiatornumber.Equals(staffADProfile.employee_number)
                                            ? this.CANSAVE : "false"; 

            staffADProfile.appperiod = entryModel.AppraisalPeriod;

            XElement ApprovalHistory = new LINQCalls().getApprovalHistory( WorkflowID );
            XDocument  xDocument = DataHandlers.ToXDocument(ApprovalHistory);
            
            List<ApprovalDetails> approvalHistory = xDocument.Descendants("Approvals")
                .Select( det => new ApprovalDetails{
                                                        ApproverNames       = det.Element("ApproverName").Value,
                                                        ApproverStaffNumbers= det.Element("ApproverStaffNumber").Value,
                                                        ApprovedStages      = det.Element("ApprovedStage").Value,
                                                        ApproverAction      = det.Element("ApproverAction").Value,
                                                        ApprovalDateTime    = det.Element("ApprovalDateTime").Value
                                                    })
                .ToList();

            SuperInputTargetModel superInputTargetModel = new SuperInputTargetModel();

            if( TempData["superInputTargetModel"]!=null ){
                superInputTargetModel = TempData["superInputTargetModel"] as SuperInputTargetModel;
            } else {
                superInputTargetModel = new SuperInputTargetModel{
                                                                    WorkflowID      = WorkflowID,
                                                                    RequestStageID  = entryModel.RequestStageId,
                                                                    RequestStage    = entryModel.RequestStage,
                                                                    RequestDate     = entryModel.DateSubmitted,
                                                                    StaffADProfile  = staffADProfile,
                                                                    RequestDetails  = requestDetails,
                                                                    CanSave         = cansave,
                                                                    ApprovalDetails = approvalHistory,
                                                                    EntryModel      = entryModel,
                                                                    RequestBranch   = entryModel.Branch,
                                                                    RequestBranchCode = entryModel.BranchCode
                                                                 };
            }

            //sort the list
            var d = from x in superInputTargetModel.RequestDetails 
                    orderby x.name == OTHERS, x.name
                    select x;
            superInputTargetModel.RequestDetails = d.ToList();

            Session["requestDetails"]   = superInputTargetModel.RequestDetails;
            Session["UserName"]         = UserName;
            TempData["editMode"]        = ( editMode==true ) ? null : "false";

            if( TempData["ErrorMessage"]!=null ){
                ViewBag.ErrorMessage=TempData["ErrorMessage"] as string;
            }

            TempData["superInputTargetModel"] = superInputTargetModel;
            return RedirectToAction( "TargetInputForm",new { UserName = Session["UserName"] as string } );
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class MultipleButtonAttribute : ActionNameSelectorAttribute {
            public string Name { get; set; }
            public string Argument { get; set; }

            public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo) {
                var isValidName = false;
                var keyValue = string.Format("{0}:{1}", Name, Argument);
                var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);

                if (value != null) {
                    controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Argument;
                    isValidName = true;
                }
                return isValidName;
            }
        }
    }
}