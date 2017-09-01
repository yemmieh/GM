using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using FastMember;
using GeneralMemo.App_Code;
using GeneralMemo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace GeneralMemo.Controllers {
    public class OrgStructureController : Controller {
        // GET: OrgStructure

        private static string HOBCODE = "001";
        private static string ABJCODE = "013";
        private static string BRACODE = "000";

        private string _UserName ="";

        [RBAC]
        public ActionResult ViewStructure( ) {

            this._UserName = User.Identity.Name;
            Session["UserName"] = _UserName;

            if( _UserName == null || _UserName.Equals(String.Empty)){
                ViewBag.ErrorMessage="You must be logged in to continue.";
                return RedirectToAction("AwaitingMyApproval","AwaitingApproval");
            }

            

            //query the db and get the organisational structure

            return View();
        }

        [HttpGet]
        [RBAC]
        public ActionResult SetupAppraisalApprovers() {  
    
            AppraisalApproverModel appraisalApproverModel = new AppraisalApproverModel();
            string deptcode = BRACODE;
            string origdeptcode="";
            ViewBag.hasdata="false";
            
            if (TempData["appraisalApproverModel"] as AppraisalApproverModel  != null) {
                appraisalApproverModel = TempData["appraisalApproverModel"] as AppraisalApproverModel;
                deptcode = String.IsNullOrEmpty(appraisalApproverModel.DeptCode) ? deptcode : appraisalApproverModel.DeptCode ;
                origdeptcode = appraisalApproverModel.DeptCode;
                int j;
                if ( !Int32.TryParse(deptcode, out j) ){
                    deptcode = HOBCODE;
                } else {
                    deptcode = ( deptcode.Equals(ABJCODE) ) ? ABJCODE : BRACODE;
                }
                ViewBag.hasdata="true";
            }

            appraisalApproverModel.DeptName = SelectListItemHelper.GetDepts();
            appraisalApproverModel.UnitName = SelectListItemHelper.GetUnits( deptcode );
            /*int k;
            if ( Int32.TryParse(deptcode, out k) ){
                deptcode = ( deptcode.Equals(ABJCODE) ) ? ABJCODE : BRACODE;
            }else {
                deptcode = HOBCODE;
            }*/
            origdeptcode=String.IsNullOrEmpty(origdeptcode)?deptcode:origdeptcode;
            appraisalApproverModel.Role     = SelectListItemHelper.GetRoles( deptcode );     
            
            if( !String.IsNullOrEmpty(TempData["ErrorMessage"] as string) )  {  
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            }
            return View( appraisalApproverModel );
        }

        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult SetupAppraisalApprovers( AppraisalApproverModel appraisalApproverModel ) {  

            appraisalApproverModel.EntryKey = getEntryKey(appraisalApproverModel);

            /*bool duplicateEntry = new LINQCalls().checkDupApproverSetup( appraisalApproverModel.EntryKey.ToUpper() );
            if (!duplicateEntry) {
                TempData["ErrorMessage"] = "Error : The staff : "+ appraisalApproverModel.StaffName +" has an existing setup with the same identity";
            }*/

            HRProfile hrprofile = new LINQCalls().hrprofile(appraisalApproverModel.HRStaffName,1);   
            if( hrprofile==null ){
                TempData["ErrorMessage"] = "Error : You staff profile is not properly setup";
            }
            
            //Setup the branch
            int inputMode = 0;
            string retVal = new AppDatabase().insertApproverSetup( appraisalApproverModel , hrprofile , inputMode  ,"AppraisalDbConnectionString" );
            if( !String.IsNullOrEmpty(retVal) && !retVal.Split('|')[0].Equals("0")){
                TempData["ErrorMessage"] = "Error :"+retVal.Split('|')[1];
            } else {
                appraisalApproverModel = null;
            }

            TempData["appraisalApproverModel"] = appraisalApproverModel;
            return RedirectToAction( "SetupAppraisalApprovers" );
        }
        private string getEntryKey(AppraisalApproverModel ap) {
            return ap.StaffNumber+"_"+ap.RoleID+"_"+ap.UnitCode+"_"+ap.DeptCode+"_"+ap.GroupCode+"_"+ap.SuperGroupCode;
        }

        [RBAC]
        public ActionResult ViewAppraisalApprovers( string FilterBy="" ) {

            System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
            System.Configuration.KeyValueConfigurationElement CurrentAppraisalPeriod = rootWebConfig.AppSettings.Settings["CurrentAppraisalPeriod"];

            List<AppraisalApproverModel> appraisalApproverModel = new LINQCalls().getApproverSetupList();
            if( !String.IsNullOrEmpty(FilterBy) ){
                appraisalApproverModel = FilterAppraisalApproverList( appraisalApproverModel , FilterBy.ToUpper() );
            }
            return View( appraisalApproverModel );
        }

        [RBAC]
        [HttpPost]
        public ActionResult FilterApprovers( string FilterBy = "" ) {
            return RedirectToAction( "ViewAppraisalApprovers" , new {FilterBy=FilterBy} );
        }

        private List<AppraisalApproverModel> FilterAppraisalApproverList( List<AppraisalApproverModel> bhList , string FilterBy ) {
                FilterBy = FilterBy.ToUpper();
                bhList = bhList.Where(  c => c.DeptTitle.ToUpper().Contains(FilterBy)     || 
                                             c.GroupTitle.ToUpper().Contains(FilterBy)    ||
                                             c.StaffName.ToUpper().Contains(FilterBy)     || 
                                             c.StaffNumber.ToUpper().Contains(FilterBy)   ||
                                             c.RoleTitle.ToUpper().Contains(FilterBy)     || 
                                             c.UnitTitle.ToUpper().Contains(FilterBy))
                                             .ToList();
            return bhList;
        }

        [HttpGet]
        [RBAC]
        public ActionResult ApproverSetupEdit( string EntryKey, string Func ) {  

            AppraisalApproverModel appraisalApproverModel = new AppraisalApproverModel();

            if (String.IsNullOrEmpty(EntryKey) || String.IsNullOrEmpty(Func) ) {
                TempData["ErrorMessage"] = "Error : Please access the page properly.";
            } else {
                if( Func.Equals("Edit") ){
                    appraisalApproverModel = new LINQCalls().getApproverSetupEntry(EntryKey);
                } else {
                    string retVal = new AppDatabase().deleteApproverSetup( EntryKey ,"AppraisalDbConnectionString" );
                    if( !String.IsNullOrEmpty(retVal) && !retVal.Split('|')[0].Equals("0")){
                        TempData["ErrorMessage"] = "Error : "+retVal.Split('|')[1];
                    } else {
                        return RedirectToAction( "ViewAppraisalApprovers" );
                    }
                }                
            }
            TempData["appraisalApproverModel"] = appraisalApproverModel;
            return RedirectToAction( "SetupAppraisalApprovers" );
        }
        
        //Setup HR Roles
        [HttpGet]
        [RBAC]
        public ActionResult HRRoleSetup() {  
    
            AppraisalApproverModel appraisalApproverModel = new AppraisalApproverModel();
            ViewBag.hasdata = "false";
            
            if (TempData["appraisalApproverModel"] as AppraisalApproverModel  != null) {
                appraisalApproverModel = TempData["appraisalApproverModel"] as AppraisalApproverModel;
                ViewBag.hasdata = "true";
            }
            
            appraisalApproverModel.Role = SelectListItemHelper.GetHRRoles(); 
            appraisalApproverModel.StatusName = SelectListItemHelper.GetHRStatus();

            if( !String.IsNullOrEmpty(TempData["ErrorMessage"] as string) )  {  
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            }

            return View( appraisalApproverModel );
        }
        
        [HttpPost]
        [RBAC]
        public ActionResult HRRoleSetup( AppraisalApproverModel appraisalApproverModel ) {  
    
            appraisalApproverModel.EntryKey = (String.IsNullOrEmpty(appraisalApproverModel.EntryKey)) ? getHREntryKey(appraisalApproverModel) : appraisalApproverModel.EntryKey ;

            HRProfile hrprofile = new LINQCalls().hrprofile(appraisalApproverModel.HRStaffName,1);   
            if( hrprofile==null ){
                TempData["ErrorMessage"] = "Error : You staff profile is not properly setup";
            }

            //Get the staff's username from the staff number
            //AD
            ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(appraisalApproverModel.StaffNumber );
            string _username = activeDirectoryQuery.GetUserName();
            if( _username==null ){
                ViewBag.ErrorMessage="The user's profile is not properly setup on the system. Please contact InfoTech.";
                return View();
            }

            appraisalApproverModel.UserName = _username;
            
            //Setup the staff
            string retVal = new AppDatabase().insertRoleSetup( appraisalApproverModel , hrprofile  ,"AppraisalDbConnectionString" );
            if( !String.IsNullOrEmpty(retVal) && !retVal.Split('|')[0].Equals("0")){
                TempData["ErrorMessage"] = "Error :"+retVal.Split('|')[1];
            } else {
                appraisalApproverModel = null;
            }

            TempData["appraisalApproverModel"] = appraisalApproverModel;
            return RedirectToAction( "HRRoleSetup" );
        }
        private string getHREntryKey(AppraisalApproverModel ap) {
            return ap.StaffNumber+"_"+ap.RoleID;
        }

        [HttpGet]
        [RBAC]
        public ActionResult ViewHRUsers() {
            List<AppraisalApproverModel> appraisalApproverModel = new LINQCalls().getHRUsers();
            return View( appraisalApproverModel );
        }
        
        [HttpGet]
        [RBAC]
        public ActionResult RoleEdit( string EntryKey, string Func ) {  

            AppraisalApproverModel appraisalApproverModel = new AppraisalApproverModel();

            if (String.IsNullOrEmpty(EntryKey) || String.IsNullOrEmpty(Func) ) {
                TempData["ErrorMessage"] = "Error : Please access the page properly.";
            } else {
                if( Func.Equals("Edit") ){
                    appraisalApproverModel = new LINQCalls().getRoleSetupEntry(EntryKey);
                } else {
                    string retVal = new AppDatabase().deleteRoleSetup( EntryKey ,"AppraisalDbConnectionString" );
                    if( !String.IsNullOrEmpty(retVal) && !retVal.Split('|')[0].Equals("0")){
                        TempData["ErrorMessage"] = "Error : "+retVal.Split('|')[1];
                    } else {
                        return RedirectToAction( "ViewHRUsers" );
                    }
                }                
            }
            TempData["appraisalApproverModel"] = appraisalApproverModel;
            return RedirectToAction( "HRRoleSetup" );
        }
        /**

        [HttpGet]
        public ActionResult BHSingleSetupFormEdit( string StaffNumber , string SelectedAppraisalPeriod ) {
            
            if (String.IsNullOrEmpty(StaffNumber) || String.IsNullOrEmpty(SelectedAppraisalPeriod) ) {
                return RedirectToAction( "BHSingleSetupForm" );
            }
            BHSingleSetupEditModel bHSingleSetupModel = new LINQCalls().bHSingleSetupModel(StaffNumber,SelectedAppraisalPeriod);
            return View( bHSingleSetupModel );
        }

        [HttpPost]
        public ActionResult BHSingleSetupFormEdit( BHSingleSetupEditModel bHSingleSetupEditModel ) {
            
            if (!ModelState.IsValid) {
                TempData["ErrorMessage"] = "Invalid model ";
                TempData["bHSingleSetupEditModel"] = bHSingleSetupEditModel;
                return RedirectToAction("BHSingleSetupFormEdit");  
            }

            HRProfile hrprofile = new LINQCalls().hrprofile(bHSingleSetupEditModel.InitiatorLoginName,1);   
            if( hrprofile==null ){
                TempData["ErrorMessage"] = "Error : You staff profile is not properly setup";
                TempData["bHSingleSetupEditModel"] = bHSingleSetupEditModel;
                return RedirectToAction("BHSingleSetupFormEdit");  
            }

            int inputMode = 1;
            int retVal = new AppDatabase().insertSingleSetup( bHSingleSetupEditModel , hrprofile , inputMode  ,"AppraisalDbConnectionString" );

            return RedirectToAction("ViewBranchInitiators");  
        }

        [HttpPost]
        public ActionResult GetStaffProfile( String StaffNumber ) {

            string errorResult = "{{\"employee_number\":\"{0}\",\"name\":\"{1}\"}}";
            if( string.IsNullOrEmpty( StaffNumber ) ) {
                errorResult = string.Format(errorResult , "Error" , "Invalid staff number");        
                return Content(errorResult, "application/json");
            }

            var profile = new LINQCalls().getBranchStaffProfile(StaffNumber,1);    
            if( profile==null ){
                errorResult = string.Format(errorResult , "Error" , "No records found for the staff number");        
                return Content(errorResult, "application/json");
            } else {
                return Json( profile , JsonRequestBehavior.AllowGet );
            }
        }

        public ActionResult ViewAppriasalApprovers() {
            return View();
        }
        */
        
        [RBAC]
        public ActionResult GetUnitsAndRoles( string deptcode,string _type ) {
            
            string _deptcode = deptcode;
            int j;
            if ( Int32.TryParse(deptcode, out j) ){
                deptcode = ( deptcode.Equals(ABJCODE) ) ? ABJCODE : BRACODE;
            }else {
                deptcode = HOBCODE;
            }
            if( _type.Equals("unit") ){
                return GetUnitsForDept( deptcode , _deptcode );
            }else{
                return GetRolesForDept(deptcode);
            }
        }
        
        [RBAC]

        public ActionResult GetUnitsForDept( string deptcode , string _deptcode ) {
            var units_ = new LINQCalls().getUnitsAsJSON( deptcode , _deptcode );    
            return Json( units_ , JsonRequestBehavior.AllowGet );
        }

        [RBAC]
        public ActionResult GetRolesForDept( string deptcode) {
            var units_ = new LINQCalls().getRolesAsJSON(deptcode );    
            return Json( units_ , JsonRequestBehavior.AllowGet );
        }
        public class SelectListItemHelper {
            public static SelectList GetRoles( string deptcode ) {
                return new LINQCalls().getRoles( deptcode );
            }
            public static SelectList GetUnits( string deptcode ) {
                return new LINQCalls().getUnits( deptcode );
            }
            public static SelectList GetDepts() {
                return new LINQCalls().getDepts();
            }

            internal static SelectList GetHRRoles() {
                return new LINQCalls().getHRRoles();
            }

            internal static SelectList GetHRStatus() {
                return new SelectList(new[] { "Enabled", "Disabled" }
                .Select(x => new {value = x, text = x}),"value", "text");
            }
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

