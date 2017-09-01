using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace GeneralMemo.Models {
    public class AppraisalApproverModel {
       
        public string EntryKey { get;set;}
        
        [Required]
        [Display(Name = "Staff Name")]
        public string StaffName { get;set;}

        public string UserName { get;set;}

        [Required]
        [Display(Name = "Staff Number")]
        public string StaffNumber { get;set;}
        
        [Required]
        [Display(Name = "Role")]
        public int RoleID { get;set;}
        public SelectList Role { get;set;}
        public string RoleTitle { get;set;}
        
        [Required]
        [Display(Name = "Sector/Unit Name")]
        public string UnitCode { get;set;}
        public SelectList UnitName { get;set;}
        public string UnitTitle { get;set;}

        [Required]
        [Display(Name = "Branch/Department Name")]
        public string DeptCode { get;set;}
        public SelectList DeptName { get;set;}      
        public string DeptTitle { get;set;}
        
        //[Required]
        [Display(Name = "Zone/Group Name")]
        public SelectList GroupName { get;set;}
        public string GroupCode { get;set;}
        public string GroupTitle { get;set;}

        public bool HasSuperGroup { get;set;}
        
        //[Required]
        [Display(Name = "Super Zone/Group Name")]
        public string SuperGroupCode { get;set;}
        public SelectList SuperGroupName { get;set;}
        public string SuperGroupTitle { get;set;}
        
        [Display(Name = "Edit Date")]
        public DateTime EditDate { get; set; }
        
        [Display(Name = "Create Date")]
        public DateTime CreateDate { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string HRStaffName { get;set;}
        
        [HiddenInput(DisplayValue = false)]
        public string HRStaffNumber { get;set;}

        [Required]
        [Display(Name = "Status")]
        public string StatusCode { get;set;}     
        public SelectList StatusName { get;set;}
        public string StatusTitle { get;set;}

        public int Permission_ID { get;set;}
        public string PermissionDescription { get;set;}

    }

}
