using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GeneralMemo.Models {
    public class RerouteModel {
        
        public string WorkflowID{get;set;}

        [Display(Name = "Current Request Stage")]
        public string CurrentRequestStage { get;set;}
        
        [Required]
        public string Comments { get;set;}
        
        [Required]
        [Display(Name = "New Request Stage")]
        public string NewRequestStageCode { get;set;}
        public SelectList NewRequestStage { get;set;}
        public string NewRequestStageTitle { get;set;}
        
        public EntryModel EntryModel { get;set;}

    }
}
