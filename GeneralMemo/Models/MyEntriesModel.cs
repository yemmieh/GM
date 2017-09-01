using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralMemo.Models {
    public class EntriesModel {
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

        public string UnitName { get;set;}
        public string GroupName { get;set;}
        public string SuperGroupName { get;set;}
        public string EntryKey { get;set;}

        public string Audit { get; set; }
    }


}
