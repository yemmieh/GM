using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GeneralMemo.Models {
    public class StaffADProfile {
        public string employee_number { get; set; }
        public string branch_name { get;set;}
        public string branch_code { get; set; }
        public string branch_address { get; set; }
        public string mobile_phone { get; set; }
        public string gsm { get; set; }
        public string grade { get; set; }
        public string jobtitle { get; set; }
        public string office_ext { get; set; }        
        public string department { get;set;}
        public string user_logon_name { get;set;}
        public string email { get;set;}
        public List<string> membership { get;set;}
        public string hodeptcode { get;set;}
        public string hodeptname { get;set;}
        public string appperiod { get;set;}  

        /***New Staff Input Fields***/
        [Display(Name = "Staff Number")]
        public string in_StaffNumber { get; set; }
        
        [Display(Name = "Staff Name")]
        public string in_StaffName { get; set; }
        
        [Display(Name = "Staff Grade")]
        public string in_StaffGrade { get; set; }
    }

    public class staffprofile
    {
        public string employee_number { get; set; }
        public string branch_name { get; set; }
        public string branch_code { get; set; }
        public string branch_address { get; set; }
        public string mobile_phone { get; set; }
        public string gsm { get; set; }
        public string jobtitle { get; set; }
        public string office_ext { get; set; }
        public string department { get; set; }
        public string user_logon_name { get; set; }
        public string email { get; set; }
        public List<string> membership { get; set; }
        public string hodeptcode { get; set; }
        public string hodeptname { get; set; }
        public string appperiod { get; set; }

        /***New Staff Input Fields***/
        [Display(Name = "Staff Number")]
        public string in_StaffNumber { get; set; }

        [Display(Name = "Staff Name")]
        public string in_StaffName { get; set; }

        [Display(Name = "Staff Grade")]
        public string in_StaffGrade { get; set; }
    }
    public class RequestDetails {
        public RequestDetails() { 
            workflowid      = String.Empty;
            requeststageid
                            = 0;
            requeststage    = INIT_STAGE;
            requestdate     = DateTime.Now;
            cabal           = "0";
            cabal_l         = "0";
            sabal           = "0";
            sabal_l         = "0";
            fx              = "0";
            rv              = "0";
            fd              = "0";
            inc             = "0";
            inc_l           = "0";
        }
        private const string INIT_STAGE = "Initiate Target";
        public string entry_key { get;set;}
        public string workflowid { get;set;}
        public int requeststageid { get;set;}
        public string requeststage { get;set;}
        public DateTime requestdate { get;set;}
        public string employee_number { get;set;}
        public string name { get;set;}
        public string grade { get;set;}
        public string  cabal { get;set;}
        public string  cabal_l { get;set;}
        public string  sabal { get;set;}
        public string  sabal_l { get;set;}
        public string  fx { get;set;}
        public string  rv { get;set;}
        public string  fd { get;set;}
        public string  inc { get;set;}
        public string  inc_l { get;set;}
    }
    public class ApprovalDetails {
        //public string Approver{ get;set;}
        public string ApproverNames{ get;set;}
        public string ApproverStaffNumbers{ get;set;}
        public string ApprovedStages{ get;set;}
        public string ApproverAction{ get;set;}
        public string ApproverComments{ get;set;}
        public string ApprovalDateTime{ get;set;}
    }
    public class AuditDetails {
        public string approver{ get;set;}
        public string stageprocessed{ get;set;}
        public string totca{ get;set;}
        public string totca_l{ get;set;}
        public string totsa{ get;set;}
        public string totsa_l{ get;set;}
        public string totfx{ get;set;}
        public string totrv{ get;set;}
        public string totfd{ get;set;}
        public string totinc{ get;set;}
        public string totinc_l{ get;set;}
    }
    public class EntryModel {
        public string WorkflowID { get;set;}
        public string StaffNumber { get;set;}
        public string StaffName { get;set;}
        public string Branch { get;set;}
        public string BranchCode { get;set;}
        public string DeptName { get;set;}
        public string DeptCode { get;set;}
        public string AppraisalPeriod { get;set;}
        public string AppraisalPeriodName { get;set;}
        public string RequestStage { get;set;}
        public int RequestStageId { get;set;}
        public string UploadStatus { get;set;}
        public DateTime DateSubmitted { get;set;}
        public string Approvers { get;set;}
        public string Action { get;set;}
        public string Audit { get; set; }
    }
    public class SuperInputTargetModel{
        public string WorkflowID { get;set; }
        public int RequestStageID { get;set; }
        public string RequestStage { get;set; }
        public string RequestBranch { get;set; }
        public string CanSave { get;set; }
        public string RequestBranchCode { get;set; }
        public DateTime RequestDate { get;set; }
        public StaffADProfile StaffADProfile{get; set;}
        public List<RequestDetails> RequestDetails{get; set;}
        //public List<StaffTargetProfile> StaffTargetProfiles{get;set;}
        public List<ApprovalDetails> ApprovalDetails{get;set;}
        public EntryModel EntryModel { get;set; }
    }
}
