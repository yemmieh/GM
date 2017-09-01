using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeneralMemo.Models {

    public class BHSingleSetupModel {

        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Branch Name")]
        public string SelectedBranch { get; set; }
        public SelectList BranchName { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string HODeptCode { get;set;}
        public SelectList HODeptName { get;set;}

        [Required]
        [Display(Name = "Staff Number")]
        public string StaffNumber { get; set; }

        [Required]
        [Display(Name = "Staff Name")]
        public string StaffName { get; set; }

        [Required]
        [Display(Name = "Staff Grade")]
        public string StaffGrade { get; set; }

        //[Required]
        [Display(Name = "Comments")]
        public string Comments { get; set; }

        //[Required]
        [Display(Name = "Entry Date")]
        public DateTime CreateDate { get; set; }

        [Required]
        [Display(Name = "Appraisal Period")]
        public string SelectedAppraisalPeriod { get; set; }
        public SelectList AppraisalPeriod { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string InitiatorStaffNumber { get;set;}

        [HiddenInput(DisplayValue = false)]
        public string InitiatorStaffName { get;set;}
        
        [HiddenInput(DisplayValue = false)]
        public string InitiatorLoginName { get;set;}
            
        [HiddenInput(DisplayValue = false)]
        public string BranchIsHODept { get;set;}

        [HiddenInput(DisplayValue = false)]
        public string SetupAppPeriod { get; set; }

        public string AppraisalTitle { get; set; }
        
        [HiddenInput(DisplayValue = false)]
        public string SetupBranch { get; set; }
        
        [HiddenInput(DisplayValue = false)]
        public string SetupDept { get; set; }
        
    }

    public class BHSingleSetupEditModel {

        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Staff Number")]
        public string StaffNumber { get; set; }

        [Required]
        [Display(Name = "Staff Name")]
        public string StaffName { get; set; }

        [Required]
        [Display(Name = "Staff Grade")]
        public string StaffGrade { get; set; }

        //[Required]
        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string InitiatorStaffNumber { get;set;}

        [HiddenInput(DisplayValue = false)]
        public string InitiatorStaffName { get;set;}
        
        [HiddenInput(DisplayValue = false)]
        public string InitiatorLoginName { get;set;}

        [Required]
        [HiddenInput(DisplayValue = false)]
        public string SelectedAppraisalPeriod { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string SetupAppPeriod { get; set; }
        
        [Required]
        [HiddenInput(DisplayValue = false)]
        public string SetupBranch { get; set; }
        
        [Required]
        [HiddenInput(DisplayValue = false)]
        public string SetupBranchCode { get; set; }

        [Required]
        [Display(Name = "Entry Date")]
        public string CreateDate { get; set; }

        [Display(Name = "Department")]
        public string HODeptCode { get;set;}
        public string HODeptName { get;set;}
        public string SetupDept { get;set;}

    }

    public class FileTypesAttribute: ValidationAttribute{
        
        private readonly List<string> _types;
 
        public FileTypesAttribute(string types) {

            _types = types.Split(',').ToList();

        } 
        public override bool IsValid(object value) {
            
            if (value == null) return true;
            var fileExt = System.IO.Path.GetExtension((value as HttpPostedFileBase).FileName).Substring(1);
            return _types.Contains(fileExt, StringComparer.OrdinalIgnoreCase);

        } 
        public override string FormatErrorMessage(string name) {    
        
            return string.Format("Invalid file type. Only the following types {0} are supported.", String.Join(", ", _types));

        }
    }

    public class HRProfile {
        public string name { get; set; }
        public string employee_number { get; set; }
    }

    public class BHBulkSetupFormModel {
        
        [Required]
        [Display(Name = "Uploaded Excel File")]
        public HttpPostedFileBase UploadedExcelFile { get; set; }

        public string UploadedExcelFileTable { get; set; }
    }

    public class SetupExcelModel {
        
        [Key]
        public int Id { get; set; }

        [Required]
        public string StaffNumber { get; set; }

        [Required]
        public string StaffName { get; set; }
        
        [Required]
        public string StaffBranch { get; set; }
        
        [Required]
        public string StaffBranchCode { get; set; }
        
        [Required]
        public string StaffRole { get; set; }

        [Required]
        public string SelectedAppraisalPeriod { get; set; }
        
        [Required]
        public string SetupAppPeriod { get; set; }
        
        [Required]
        public HRProfile HRProfile { get;set;}

        public string  Comments { get;set;}

        [Display(Name = "Department")]
        public string HODeptCode { get;set;}
        public string HODeptName { get;set;}

    }

    public class SuperBulkSetupModel{
        public BHSingleSetupModel BHSingleSetupModel{get; set;}
        public BHBulkSetupFormModel BHBulkSetupFormModel{get; set;}

        public List<SetupExcelModel> SetupExcelModel{get;set;}
    }

    public class StaffInputModel{
        public string HODeptCode { get;set;}
        public string HODeptName { get;set;}
        public string BranchCode{get;set;}
        public string BranchName{get;set;}
        public string InitiatorName{get;set;}
        public string InitiatorNumber{get;set;}
        //public string InitiatorGrade{get;set;}
        public string Comments{get;set;}
        public string AppraisalPeriod{get;set;}	   
        public string HRStaffName{get;set;}
        public string HRStaffNumber{get;set;}
    }
}
