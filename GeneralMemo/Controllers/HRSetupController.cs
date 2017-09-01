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
    public class HRSetupController : Controller {
        // GET: HRSetup
        private const string MARKETING  = "MARKETING";
        private const string HOBRCODE   = "001";
        private const string OTHERS     = "OTHERS";
        private const string NA         = "NA";

        private string UPLOADEDMSG  = "You have successfully uploaded the target setup.";

        //private string UserID    = "";
        private string _UserName = "";

        [RBAC]
        [HttpGet]
        public ActionResult BHSingleSetupForm() {
            
            BHSingleSetupModel bHSingleSetupModel = new BHSingleSetupModel{ 
                BranchName      = SelectListItemHelper.GetBranches(),
                AppraisalPeriod = SelectListItemHelper.GetAppraisalPeriod(String.Empty),
                HODeptName      = SelectListItemHelper.GetDepts(String.Empty)
            };   

            BHSingleSetupModel bhSingleSetupModel   = TempData["bHSingleSetupModel"] as BHSingleSetupModel;
            ViewBag.BranchSelectList                = bHSingleSetupModel.BranchName;
            ViewBag.ApraisalPeriodSelectList        = bHSingleSetupModel.AppraisalPeriod ;
            
            if( bhSingleSetupModel == null )  {    
                
                //return View( bHSingleSetupModel );
            } else  {
                 String ErrorMessage =  TempData["ErrorMessage"] as String;
                 if ( ErrorMessage != null ) {
                     ViewBag.ErrorMessage = ErrorMessage;
                 }
                 //return View( bhSingleSetupModel );
            }

            return View( bHSingleSetupModel );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RBAC]
        public ActionResult BHSingleSetupForm( BHSingleSetupModel bHSingleSetupModel ) {

            if (!ModelState.IsValid) {
                TempData["ErrorMessage"] = "Invalid model ";
                TempData["bHSingleSetupModel"] = bHSingleSetupModel;
                return RedirectToAction("BHSingleSetupForm");  
            }
            bool duplicateEntry = new LINQCalls().checkDuplicateEntry( 
                                                                        bHSingleSetupModel.SelectedAppraisalPeriod ,  
                                                                        bHSingleSetupModel.StaffNumber ,
                                                                        bHSingleSetupModel.SelectedBranch
                                                                      );
            if (bHSingleSetupModel.SelectedBranch != "001")
            {
                if (!duplicateEntry)
                {
                    TempData["ErrorMessage"] = "Error : The staff number " + bHSingleSetupModel.StaffNumber + " OR branch " + bHSingleSetupModel.SetupBranch + "  has been setup for the selected appraisal period";
                    TempData["bHSingleSetupModel"] = bHSingleSetupModel;
                    return RedirectToAction("BHSingleSetupForm");
                }
            }
            HRProfile hrprofile = new LINQCalls().hrprofile(bHSingleSetupModel.InitiatorLoginName,1);   
            if( hrprofile==null ){
                TempData["ErrorMessage"] = "Error : You staff profile is not properly setup";
                TempData["bHSingleSetupModel"] = bHSingleSetupModel;
                return RedirectToAction("BHSingleSetupForm");  
            }
            
            //Setup the branch
            int inputMode = 0;
            int retVal = new AppDatabase().insertSingleSetup( bHSingleSetupModel , hrprofile , inputMode  ,"AppraisalDbConnectionString" );

            return RedirectToAction("BHSingleSetupForm");  
        }

        [HttpGet]
        [RBAC]
        public ActionResult BHSingleSetupFormEdit( string StaffNumber , string SelectedAppraisalPeriod ) {
            
            if (String.IsNullOrEmpty(StaffNumber) || String.IsNullOrEmpty(SelectedAppraisalPeriod) ) {
                return RedirectToAction( "BHSingleSetupForm" );
            }
            BHSingleSetupEditModel bHSingleSetupModel = new LINQCalls().bHSingleSetupModel(StaffNumber,SelectedAppraisalPeriod);
            return View( bHSingleSetupModel );
        }

        [HttpPost]
        [RBAC]
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

        [HttpGet]
        [RBAC]
        public ActionResult ViewBranchInitiators( string FilterBy="" ) {
            
            System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
            System.Configuration.KeyValueConfigurationElement CurrentAppraisalPeriod = rootWebConfig.AppSettings.Settings["CurrentAppraisalPeriod"];
            
            List<BHSingleSetupModel> bHSingleSetupModel = new LINQCalls().getBranchInitiators( CurrentAppraisalPeriod.Value.ToString() );
            if( !String.IsNullOrEmpty(FilterBy) ){
                /*bHSingleSetupModel = bHSingleSetupModel.Where(  c => c.SetupBranch.ToUpper().Contains(FilterBy)   || 
                                                                c.StaffNumber.ToUpper().Contains(FilterBy)        || 
                                                                c.StaffName.ToUpper().Contains(FilterBy)          || 
                                                                c.SetupAppPeriod.ToUpper().Contains(FilterBy)     || 
                                                                c.SelectedAppraisalPeriod.ToUpper().Contains(FilterBy))
                                                       .ToList();*/
                bHSingleSetupModel = FilterBranchInitiatorList( bHSingleSetupModel , FilterBy.ToUpper() );
            }
            return View( bHSingleSetupModel );
        }

        [HttpPost]
        [RBAC]
        public ActionResult FilterBranchInitiators( string FilterBy ) {
            return RedirectToAction( "ViewBranchInitiators" , new {FilterBy=FilterBy} );
        }

        private List<BHSingleSetupModel> FilterBranchInitiatorList( List<BHSingleSetupModel> bhList , string FilterBy ) {
                FilterBy = FilterBy.ToUpper();
                bhList = bhList.Where(  c => c.SetupBranch.ToUpper().Contains(FilterBy)   || 
                                             c.StaffNumber.ToUpper().Contains(FilterBy)     || 
                                             c.StaffName.ToUpper().Contains(FilterBy)          || 
                                             c.SetupAppPeriod.ToUpper().Contains(FilterBy)     || 
                                             c.SelectedAppraisalPeriod.ToUpper().Contains(FilterBy))
                                             .ToList();
            return bhList;
        }
        
        [HttpGet]
        [RBAC]
        public ActionResult BHBulkSetupForm( int? ActionState ) {

            SuperBulkSetupModel superBulkSetupModel;

            if( ActionState!=null && ActionState==0 ){
                BHSingleSetupModel bhs = new BHSingleSetupModel{ 
                    BranchName      = SelectListItemHelper.GetBranches(),
                    AppraisalPeriod = SelectListItemHelper.GetAppraisalPeriod( String.Empty ) 
                }; 
                BHBulkSetupFormModel bhb = new BHBulkSetupFormModel();
                List<SetupExcelModel> sem = new List<SetupExcelModel>();
                
                superBulkSetupModel = new SuperBulkSetupModel();
                superBulkSetupModel.BHBulkSetupFormModel= bhb;
                superBulkSetupModel.SetupExcelModel = sem;
                superBulkSetupModel.BHSingleSetupModel = bhs;

                return View( superBulkSetupModel );
            }

            string periodSelectedValue = (TempData[ "periodSelectedValue" ]!=null) ? TempData[ "periodSelectedValue" ].ToString() : String.Empty; 
            BHSingleSetupModel bHSingleSetupModel = new BHSingleSetupModel{ 
                BranchName      = SelectListItemHelper.GetBranches(),
                AppraisalPeriod = SelectListItemHelper.GetAppraisalPeriod( periodSelectedValue ) 
            };  

            if ( TempData[ "superBulkSetupModel" ]!=null ){   
                superBulkSetupModel = TempData["superBulkSetupModel"] as SuperBulkSetupModel;
            } else {

                if( ViewBag.HasGrid!=null ){

                    superBulkSetupModel = TempData["superBulkSetupModel"] as SuperBulkSetupModel;

                } else {
                    BHBulkSetupFormModel bHBulkSetupFormModel = new BHBulkSetupFormModel();
                    List<SetupExcelModel> setupExcelModel = new List<SetupExcelModel>();

                    superBulkSetupModel = new SuperBulkSetupModel();
                    superBulkSetupModel.BHBulkSetupFormModel= bHBulkSetupFormModel;
                    superBulkSetupModel.SetupExcelModel = setupExcelModel;

                    String ErrorMessage =  TempData["ErrorMessage"] as String;
                    if ( ErrorMessage != null ) {
                        ViewBag.ErrorMessage = ErrorMessage;
                    }
                }               
            }
            
            superBulkSetupModel.BHSingleSetupModel  = bHSingleSetupModel;
            return View( superBulkSetupModel );
        }

        [HttpPost]
        [RBAC]
        [MultipleButton(Name = "action", Argument = "Upload")]
        public ActionResult BHBulkSetupForm( SuperBulkSetupModel superBulkSetupModel ) {

            HRProfile hrprofile = new LINQCalls().hrprofile(superBulkSetupModel.BHSingleSetupModel.InitiatorLoginName,1);   
            if( hrprofile==null ){
                TempData["ErrorMessage"] = "Error : You staff profile is not properly setup";
                TempData["superBulkSetupModel"] = superBulkSetupModel;
                return RedirectToAction("BHBulkSetupForm");  
            }

            string periodSelectedValue = Request.Form["BHSingleSetupModel.SelectedAppraisalPeriod"];
            HttpPostedFileBase uploadedExcelFile = superBulkSetupModel.BHBulkSetupFormModel.UploadedExcelFile;
           
            superBulkSetupModel.BHSingleSetupModel.Comments="";

            BHBulkSetupFormModel bHBulkSetupFormModel = superBulkSetupModel.BHBulkSetupFormModel;
            List<SetupExcelModel> setupExcelModel   = GetDataTableFromSpreadsheet(bHBulkSetupFormModel.UploadedExcelFile.InputStream,false,superBulkSetupModel.BHSingleSetupModel,hrprofile);

            superBulkSetupModel.SetupExcelModel     = setupExcelModel;            
            TempData[ "periodSelectedValue" ] = periodSelectedValue;
            TempData[ "superBulkSetupModel" ] = superBulkSetupModel;
            return RedirectToAction( "BHBulkSetupForm" );
        }

        [HttpPost]
        [RBAC]
        [MultipleButton(Name = "action", Argument = "BulkSetup")]
        public ActionResult BHBulkSetupForm( SuperBulkSetupModel superBulkSetupModel, int? i ) {

            HRProfile hrprofile = new LINQCalls().hrprofile(superBulkSetupModel.BHSingleSetupModel.InitiatorLoginName,1);   
            if( hrprofile==null ){
                TempData["ErrorMessage"] = "Error : You staff profile is not properly setup";
                TempData["superBulkSetupModel"] = superBulkSetupModel;
                return RedirectToAction("BHBulkSetupForm");  
            }

            //now get the initiator model and convert it to a datatable
            List<SetupExcelModel> setupExcelModel   = GetDataTableFromSpreadsheet(superBulkSetupModel.BHBulkSetupFormModel.UploadedExcelFile.InputStream,false,superBulkSetupModel.BHSingleSetupModel,hrprofile);
            if( setupExcelModel.Count()<=0 ){
                TempData["ErrorMessage"] = "Error : Please upload an excel file before continuing";
                TempData["superBulkSetupModel"] = superBulkSetupModel;
                return RedirectToAction("BHBulkSetupForm");
            }

            var staffinputmodel = setupExcelModel.Select(entry => new StaffInputModel(){
                  BranchCode        = entry.StaffBranchCode
			    , BranchName        = entry.StaffBranch
			    , InitiatorName     = entry.StaffName
			    , InitiatorNumber   = entry.StaffNumber
			    //, InitiatorGrade    = String.Empty
			    , Comments          = String.Empty
                , AppraisalPeriod   = entry.SelectedAppraisalPeriod
                , HODeptCode        = "" //entry.HODeptCode
			    , HODeptName        = "" //entry.HODeptName
			    , HRStaffNumber     = entry.HRProfile.employee_number
			    , HRStaffName       = entry.HRProfile.name
            }).ToList(); 

            IEnumerable<StaffInputModel> staffInputModel = staffinputmodel;
            DataTable dataTable = ToDataTable(staffInputModel);

            //now upload the list to the db
            string retVal = new AppDatabase().insertBulkSetup( dataTable , "AppraisalDbConnectionString" );
           
            if( retVal!=null ){
                TempData["UploadComplete"] = "false";
                TempData["ErrorMessage"] = retVal;
                TempData["superBulkSetupModel"] = superBulkSetupModel;
                return RedirectToAction("BHBulkSetupForm");
            }

            TempData["UploadComplete"] = "true";
            return RedirectToAction( "BHBulkSetupForm" , new { ActionState = 0 });
        }

        public static DataTable ToDataTable<T>( IEnumerable<T> data) {
            
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            
            foreach (PropertyDescriptor prop in properties) { 
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            
            foreach (T item in data) {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties) { 
                    Debug.WriteLine(prop.Name);   
                    Debug.WriteLine(prop.GetValue(item)); 
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    //Debug.WriteLine(row[prop.Name]);
                }
                table.Rows.Add(row);
            }
            return table;
        }
        
        public static List<SetupExcelModel> GetDataTableFromSpreadsheet( Stream MyExcelStream , bool ReadOnly , BHSingleSetupModel bHSingleSetupModel , HRProfile hRProfile) {
            
            List<SetupExcelModel> dt = new List<SetupExcelModel>();
            using (SpreadsheetDocument sDoc = SpreadsheetDocument.Open(MyExcelStream, ReadOnly)) {
                
                WorkbookPart workbookPart = sDoc.WorkbookPart;
                IEnumerable<Sheet> sheets = sDoc.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)sDoc.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = sheetData.Descendants<Row>();

                foreach (Cell cell in rows.ElementAt(0)) {
                    //dt.Add(GetCellValue(sDoc, cell));
                }

                SetupExcelModel setupExcelModel;
                Debug.WriteLine("rows length = "+ rows.Count());

                foreach (Row row in rows)  {

                    setupExcelModel = new SetupExcelModel();
                    setupExcelModel.Id = (int)row.RowIndex.Value;
                    
                    for (int i = 0; i < row.Descendants<Cell>().Count(); i++) {           
                        Debug.WriteLine("i = "+i);
                        switch ( i ) {
                            case 0:      //StaffBranch
                                setupExcelModel.StaffBranch=GetCellValue(sDoc, row.Descendants<Cell>().ElementAt(i));
                                break;
                            case 1:     //StaffBranchCode
                                setupExcelModel.StaffBranchCode=GetCellValue(sDoc, row.Descendants<Cell>().ElementAt(i));
                                break;
                            case 2:     //StaffNumber
                                setupExcelModel.StaffNumber=GetCellValue(sDoc, row.Descendants<Cell>().ElementAt(i));
                                break;
                            case 3:     //StaffName
                                setupExcelModel.StaffName=GetCellValue(sDoc, row.Descendants<Cell>().ElementAt(i));
                                break;
                            case 4:     //StaffRole--SelectedAppraisalPeriod--SetupAppPeriod--HRProfile--Comments
                                setupExcelModel.StaffRole               = GetCellValue(sDoc, row.Descendants<Cell>().ElementAt(i));
                                setupExcelModel.SelectedAppraisalPeriod = bHSingleSetupModel.SelectedAppraisalPeriod;
                                setupExcelModel.SetupAppPeriod          = bHSingleSetupModel.SetupAppPeriod;
                                setupExcelModel.HRProfile               = hRProfile;
                                setupExcelModel.Comments                = bHSingleSetupModel.Comments;

                                dt.Add( setupExcelModel );
                                
                                break;
                        }
                    }
                }
            }
            return dt;
        }
        
        public static string GetCellValue(SpreadsheetDocument document, Cell cell) {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value = cell.CellValue.InnerXml;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString) {
                //Debug.WriteLine("stringTablePart = "+stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText);
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            } else {
                //Debug.WriteLine("value = "+value);
                return value;
            }
        }
        public static string ConvertDataTableToHTMLTable(DataTable dt) {
            string ret = "";
            ret = "<table id=" + (char)34 + "tblExcel" + (char)34 + ">";
            ret+= "<tr>";
            foreach (DataColumn col in dt.Columns) {
                ret += "<td class=" + (char)34 + "tdColumnHeader" + (char)34 + ">" + col.ColumnName + "</td>";
            }
            ret+= "</tr>";
            foreach (DataRow row in dt.Rows) {
                ret+="<tr>";
                for (int i = 0;i < dt.Columns.Count;i++) {
                    ret+= "<td class=" + (char)34 + "tdCellData" + (char)34 + ">" + row[i].ToString() + "</td>";
                }
                ret+= "</tr>";
            }
            ret+= "</table>";
            return ret;
        }

        [HttpGet]
        [RBAC]
        public ActionResult HRUpload(string UserName) {
            /**First let's check if the PostBackMessage has something
             * Very important---DO NOT DELETE!!!!!!!!!!!!!!!!!!!!!**/
            string PostBackMessage  = TempData["PostBackMessage"] as string;
            string Approvers        = TempData["Approvers"] as string;
            if(!String.IsNullOrEmpty(PostBackMessage)){
                ViewBag.PostBackMessage = "<script type='text/javascript'>alert(\""+ PostBackMessage +"\\n\\n"+ Approvers +"\");</script>";
            }    
       
            //now get the pending items
            if( UserName == null || UserName.Equals(String.Empty)){
                ViewBag.ErrorMessage="You must be logged in to continue.";
                return View();
            }
            this._UserName = UserName;

            //now resolve the user profile from AD and Xceed
            StaffADProfile staffADProfile = new StaffADProfile();
            //staffADProfile.user_logon_name = _UserName;
            staffADProfile.user_logon_name = _UserName;
            //staffADProfile.user_logon_name = "adebisi.olumoto";

            //AD
            ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery( staffADProfile );
            staffADProfile = activeDirectoryQuery.GetStaffProfile();
            if( staffADProfile==null ){
                ViewBag.ErrorMessage="Your profile is not properly setup on the system. Please contact InfoTech.";
                return View();
            }

            //**Appraisal**Initiator**Setup**\\
            //Resolve the --branchname --branchcode --department --deptcode --appperiod from Tb_TargetInitiators table
            /*
          staffADProfile = new LINQCalls().setInitiatorFields( staffADProfile );
          
          if( staffADProfile.branch_code==null ){
              ViewBag.ErrorMessage="Your profile is not properly setup for Target. Please contact Human Resources.";
              return View();
          }*/
            ViewBag.AppID=DataHandlers.APP_ID;
         //   ViewBag.StaffBranch = staffADProfile.branch_name + ( ( staffADProfile.branch_code.Equals(HOBRCODE) ) ? " | " + staffADProfile.hodeptcode : String.Empty );
            
            //Check if the approver has an existing entry for the AppraisalPeriod from the Database
            List<EntriesModel> entryDetails =  new List<EntriesModel>();
            entryDetails = new LINQCalls().getPendingHRUpload( staffADProfile );

            string filter = TempData["FilterBy"] as string;
            if (!String.IsNullOrEmpty(filter)) {
                entryDetails = FilterHRUploadList(entryDetails,filter);
            }
            Session["UserName"] = UserName;
            Session["staffADProfile"] = staffADProfile;
            return View( entryDetails );
        }

        [HttpPost]
        [RBAC]
        public ActionResult FilterHRUpload( string FilterBy , FormCollection form , string[] WorkflowID , string TargetAction ) {
            
            switch ( TargetAction ){
                case "Search":
                    TempData["FilterBy"] = FilterBy;
                    return RedirectToAction( "HRUpload" ,new { UserName = Session["UserName"] as string } );
                
                case "Upload":

                    StaffADProfile staffADProfile = new StaffADProfile();
                    staffADProfile = Session["staffADProfile"] as StaffADProfile;


                    string _retVal = string.Empty;
                    foreach (string workflowid in WorkflowID) {
                        if ( workflowid.Length > 0 ) {
                            staffADProfile.user_logon_name = User.Identity.Name;
                            ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);
                            staffADProfile = activeDirectoryQuery.GetStaffProfile();

                            staffADProfile.branch_code = new LINQCalls().getEntryProfile(workflowid).branch_code;
                            staffADProfile.branch_name = new LINQCalls().getEntryProfile(workflowid).branch_name;
                            staffADProfile.appperiod = "20150712";    
                            _retVal = new AppDatabase().inputTargetEntriesHRUpload( workflowid , staffADProfile , "AppraisalDbConnectionString" , "Submitted" );
                            if( _retVal != null ){
                                TempData["UploadComplete"]  = "false";
                                TempData["PostBackMessage"] = _retVal;                   
                            } else {
                                TempData["PostBackMessage"] = UPLOADEDMSG;
                                TempData["Approvers"] = "";                        
                            }
                        }
                    }
                    Debug.WriteLine(_retVal);
                    break;
                    //return RedirectToAction( "AwaitingMyApproval","AwaitingApproval",new { UserName = Session["UserName"] as string } );
            }
            return RedirectToAction( "AwaitingMyApproval","AwaitingApproval",new { UserName = Session["UserName"] as string } );       
        }

        private List<EntriesModel> FilterHRUploadList( List<EntriesModel> bhList , string FilterBy ) {
                FilterBy = FilterBy.ToUpper();
                bhList = bhList.Where(  c => c.Branch.ToUpper().Contains(FilterBy)              || 
                                             c.StaffNumber.ToUpper().Contains(FilterBy)         || 
                                             c.StaffName.ToUpper().Contains(FilterBy)           || 
                                             c.AppraisalPeriodName.ToUpper().Contains(FilterBy) || 
                                             c.UnitName.ToUpper().Contains(FilterBy)            || 
                                             c.StaffName.ToUpper().Contains(FilterBy)           || 
                                             c.GroupName.ToUpper().Contains(FilterBy)           || 
                                             c.SuperGroupName.ToUpper().Contains(FilterBy))
                                             .ToList();
            return bhList;
        }
        public class SelectListItemHelper {
            public static SelectList GetBranches(){                
                return new LINQCalls().getBranches();
            }
            public static SelectList GetAppraisalPeriod( string periodSelectedValue){       
                return new LINQCalls().getAppraisalPeriods( periodSelectedValue );
            }
            public static SelectList GetDepts( string branchcode) {
                //branchcode  = "001";
                string deptcodes   = "118,225,224,117,180,474,1473,166";
                return new LINQCalls().getHODepts( branchcode , deptcodes );
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