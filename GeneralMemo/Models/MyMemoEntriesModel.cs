using System;

namespace GeneralMemo.Models {
    public class MyMemoEntriesModel {
        public string WorkflowID { get;set;}
        public string StaffNumber { get;set;}
        public string StaffName { get;set;}
        public string Branch { get;set;}
        public string BranchCode { get;set;}
        public string DeptName { get;set;}
        public string DeptCode { get;set;}

        public string From { get;set;}        
        public string To { get;set;}
        public string Attn { get;set;}
        public DateTime DateOfMemo { get;set;}
        public string Subject { get;set;}

        public string RequestStage { get;set;}
        public int RequestStageId { get;set;}
        public string UploadStatus { get;set;}
        public DateTime DateSubmitted { get;set;}
        public string Approvers { get;set;}
        public string Action { get;set;}

        public string UnitName { get;set;}
        public string GroupName { get;set;}
        public string SuperGroupName { get;set;}
        public string EntryKey { get;set;}

        public string Audit { get; set; }
    }


}
