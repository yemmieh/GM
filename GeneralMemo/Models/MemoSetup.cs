
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace GeneralMemo.Models {
    public class MemoSetup {
        public MemoSetup() { 
            WorkflowID      = String.Empty;
            RequestStageId  = 0;
            RequestStage    = INIT_STAGE;
            DateSubmitted   = DateTime.Now;
        }

        private const string INIT_STAGE = "Initiate Memo";

        [Required]
        [Display(Name = "From")]
        public string From { get;set;}
        
        [Required]
        [Display(Name = "To")]
        public string To { get;set;}

        [Display(Name = "Attn")]
        public string Attn { get;set;}

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public string DateOfMemo { get;set;}
        
        [Required]
        [Display(Name = "Subject")]
        public string Subject { get;set;}

        [Required]
        [AllowHtml]
        [Display(Name = "Memo Content")]
        [DataType(DataType.MultilineText)]
        public string MemoBody { get;set;}
        
        [Display(Name = "Comments")]
        public string Comments { get;set;}
        public List<SignerDetails> SignerDetailsList{get; set;}
        public SignerDetails SignerDetails{get; set;}
        public string WorkflowID { get;set;}
        public string EntryKey { get;set;}
        public string CCFields { get;set;}
        public string OriginatorNumber { get;set;}
        public string OriginatorName { get;set;}
        public string Branch { get;set;}
        public string BranchCode { get;set;}
        public string DeptName { get;set;}
        public string DeptCode { get;set;}
        public string UnitName { get;set;}
        public string UnitCode { get;set;}
        public string RequestStage { get;set;}
        public int RequestStageId { get;set;}
        public string UploadStatus { get;set;}
        public DateTime DateSubmitted { get;set;}
        public StaffADProfile StaffADProfile{get; set;}
        public string Signers { get;set;}
        public string Approvers { get;set;}
        public string Action { get;set;}
        public string Audit { get; set; }
        public ApprovalDetail ApprovalDetail { get; set; }
        
    }

    public class SignerDetails {
        [Required]
        [Display(Name = "Approver Staff Number")]
        public string ApproverStaffNumber { get;set;}
        
        [Required]
        [Display(Name = "Name")]
        public string ApproverStaffName { get;set;}
        
        [Required]
        [Display(Name = "Designation")]
        public string ApproverGrade { get;set;}

        [Required]
        [Display(Name = "Department")]
        public string ApproverDept { get;set;}

        public int? GradeID { get;set;}
        public int? PayGroup_ID { get;set;}
    }

    public class ApprovalDetail {
        //public string Approver{ get;set;}
        public string ApproverNames{ get;set;}
        public string ApproverStaffNumbers{ get;set;}
        public string ApprovedStages{ get;set;}
        public string ApproverAction{ get;set;}
        public string ApproverComments{ get;set;}
        public string ApprovalDateTime{ get;set;}
    }
}
