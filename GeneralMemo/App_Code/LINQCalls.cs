using GeneralMemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using MoreLinq;
using System.Web.Helpers;
using System.Web.Script.Serialization;
using System.Diagnostics;


namespace GeneralMemo.App_Code {
    class LINQCalls {

        public LINQCalls() {
            this.logWriter = new LogWriter();
        }

        private static string HOBCODE = "001";
        //private static string ABJCODE = "013";
        //private static string BRACODE = "000";

        private const string ALLENTRIES= "ALLENTRIES";
        private const string ALLAPPRVED= "ALLAPPRVED";
        private const string ALLPENDING= "ALLPENDING";
        private const string ALLDENIALS= "ALLDENIALS";
        private const string ALLHRUPLOAD="ALLHRUPLOAD";
        private LogWriter logWriter;



        public staffprofile getProfile(string StaffNumber)
        {
            staffprofile profile = new staffprofile();

            ExceedConnectionDataContext xceed = new ExceedConnectionDataContext();
            var Profileinfo = (from distinct in xceed.vw_employeeinfos
                               where (distinct.employee_number == StaffNumber)
                               select
                               new
                               {
                                   BranchName = distinct.Branch,
                                   BranchCode = distinct.Branch_code,
                                   StaffNumber = distinct.employee_number,
                                   StaffName = distinct.name,
                                   DateOfEmployment = distinct.employment_date,
                                   LastPromotionDate = distinct.last_promo_date,
                                   Level = distinct.grade_code,
                                   Email = distinct.email,
                                   Dept = distinct.dept,

                                   Dept_id = distinct.department_id,
                                   Unit = distinct.unit,
                                   unitCode = distinct.unit,
                                   jobtitle = distinct.jobtitle,
                                   confirm = distinct.emp_confirm,

                               }).Distinct();

            foreach (var Profiles in Profileinfo)
            {
                profile.branch_name = Profiles.BranchName;
                profile.branch_code = Profiles.BranchCode.ToString();
                profile.hodeptname = Profiles.Dept;
                profile.hodeptcode = Profiles.Dept_id.ToString() ;
                profile.in_StaffNumber = Profiles.StaffNumber;
        
            }
            return profile;

        }


        internal List<SignerDetails> getBankStaff() {            
            ExceedConnectionDataContext exceedcnxn = new ExceedConnectionDataContext();
            var marketers   =   from v in exceedcnxn.vw_employeeinfos
                                where v.org_id.Equals( 1 )
                                orderby v.name ascending
                                select new SignerDetails {
                                    ApproverStaffNumber = v.employee_number,
                                    ApproverStaffName   = v.name,
                                    ApproverGrade       = v.grade_code,
                                    GradeID             = v.grade_id,
                                    ApproverDept        = v.dept,
                                    PayGroup_ID         = v.paygroup_id
                                };
            return  marketers.ToList(); 
        }

        internal string getStaffGrade( string staffnumber ) {            
            ExceedConnectionDataContext exceedcnxn = new ExceedConnectionDataContext();
            string grade   =   (from v in exceedcnxn.vw_employeeinfos
                                where v.employee_number.Equals( staffnumber )
                                orderby v.name ascending
                                select v.grade_code ).First().ToString();
            return  grade; 
        }

        public StaffADProfile getEntryProfile(string AcctNo)
        {
            StaffADProfile profile = new StaffADProfile();

            AppraisalConnectionDataContext Ent = new AppraisalConnectionDataContext();
            var Profileinfo = (from distinct in Ent.zib_workflow_masters
                               where (distinct.workflowid == AcctNo)
                               select
                               new 
                               {
                                   branch_name = distinct.deptname,
                                   branch_code = distinct.deptcode,
                                   hodeptname = distinct.unitname,
                                   hodeptcode = distinct.unitcode
                                  

                               }).Distinct();

            foreach (var Profiles in Profileinfo)
            {
                profile.branch_name = Profiles.branch_name;
                profile.branch_code = Profiles.branch_code.ToString();
                profile.hodeptname = Profiles.hodeptname;
                profile.hodeptcode = Profiles.hodeptcode.ToString();
               //profile.in_StaffNumber = Profiles.StaffNumber;

            }
            return profile;

        }



        internal bool checkDuplicateEntry( string selectedAppraisalPeriod , string staffNumber , string selectedBranch ) {
            try {
                AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
                var activebranches =    from s in periodcnxn.Tb_TargetInitiators
                                        where s.appraisalperiod.Equals( selectedAppraisalPeriod )
                                        && (s.initiatornumber.Equals( staffNumber )
                                        || s.deptcode.Equals( selectedBranch ))
                                        orderby s.initiatornumber ascending
                                        select s;           
                if( activebranches.Any() ){
                    return false;  
                }      
            }catch( Exception ex){
                logWriter.WriteErrorLog(string.Format( "checkDuplicateEntry : Exception!!! / {0}",ex.Message ));
                return false;
            }
            return true;
        }

        internal HRProfile hrprofile( string initiatorLoginName , int org_id ) {
            HRProfile hr       = new HRProfile();
            try { 
                ExceedConnectionDataContext dbXceed = new ExceedConnectionDataContext();
                var hrprofiles =     from v in dbXceed.vw_employeeinfos
                                     where v.logon_name.Equals( "africa\\"+initiatorLoginName )
                                     && v.org_id.Equals( org_id )
                                     select new {    
                                                    name            = v.name ,
                                                    employee_number = v.employee_number
                                                }; 
                foreach (var staff in hrprofiles) {
                    hr.name              = staff.name;
                    hr.employee_number   = staff.employee_number;                    
                }    
                return hr;                
            } catch (Exception ex) {
                logWriter.WriteErrorLog(string.Format( "hrprofile : Exception!!! / {0}",ex.Message ));
                hr=null;
            }
            return hr;
        }

        internal BHSingleSetupEditModel bHSingleSetupModel( string staffNumber , string selectedAppraisalPeriod ) {
            
            BHSingleSetupEditModel bhssem = new BHSingleSetupEditModel();
            try {
                AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
                var activebranches =    from s in periodcnxn.Tb_TargetInitiators
                                        join t in periodcnxn.Tb_App_Periods on s.appraisalperiod equals t.AppraisalPeriod
                                        where s.initiatornumber.Equals(staffNumber)
                                        && s.appraisalperiod.Equals(selectedAppraisalPeriod)
                                        && t.CurrentStatus.Equals("ACTIVATED")
                                        && t.AppraisalStatus.Equals("ENABLED")
                                        select new BHSingleSetupEditModel 
                                            {
                                                SetupBranch     = s.deptname,
                                                SetupBranchCode = s.deptcode,
                                                HODeptCode      = s.unitcode,
                                                HODeptName      = s.unitname,
                                                StaffNumber     = s.initiatornumber,
                                                StaffName       = s.initiatorname,
                                                Comments        = s.comments,
                                                SetupAppPeriod  = t.AppraisalTitle,
                                                CreateDate      = s.createdate.ToString(),
                                                SelectedAppraisalPeriod = s.appraisalperiod
                                        
                                            };
                bhssem = activebranches.First(); 

            }catch( Exception ex){
                logWriter.WriteErrorLog(string.Format( "hrprofile : Exception!!! / {0}",ex.Message ));
                bhssem = null;
            }
            return bhssem;
        }

        internal object getBranchStaffProfile( string staffNumber , int org_id ) {

            try {
                ExceedConnectionDataContext dbXceed = new ExceedConnectionDataContext();
                var profile =   from v in dbXceed.vw_employeeinfos
                                where v.employee_number.Equals( staffNumber )
                                && ( v.org_id.Equals(1) || v.org_id.Equals(4) )
                                select new {
                                                name            = v.name ,
                                                grade_code      = v.grade_code ,
                                                branch_code     = v.Branch_code,
                                                
                                                branch          = v.Branch,
                                                employee_number = v.employee_number ?? "" ,
                                                hodept          = (v.Branch_code == "001" ? "Yes" : "No")
                                            };      
                if( profile.Any() ){
                    return profile.ToArray();
                }
            } catch ( Exception ex ) {
                logWriter.WriteErrorLog(string.Format( "hrprofile : Exception!!! / {0}",ex.Message ));
                return null;
            }
            return null;
        }

        internal List<BHSingleSetupModel> getBranchInitiators( string appraisalPeriod ){        

            List<BHSingleSetupModel> bHSingleSetupModel = new List<BHSingleSetupModel>();
            try {
                AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
                var activebranches =    from s in periodcnxn.Tb_TargetInitiators
                                        join t in periodcnxn.Tb_App_Periods on s.appraisalperiod equals t.AppraisalPeriod
                                        where s.appraisalperiod.Equals( appraisalPeriod )
                                        && t.AppraisalStatus.Equals("ENABLED")
                                        && t.CurrentStatus.Equals("ACTIVATED")
                                        orderby s.deptname ascending
                                        select new BHSingleSetupModel 
                                            {                                                
                                                SetupBranch     = s.deptname,
                                                StaffNumber     = s.initiatornumber,
                                                StaffName       = s.initiatorname,
                                                Comments        = s.comments,
                                                SetupAppPeriod  = t.AppraisalTitle,
                                                CreateDate      = s.createdate,
                                                SelectedAppraisalPeriod = s.appraisalperiod
                                        
                                            };
                int i=0;
                foreach (var ab in activebranches) {
                    ab.Id = i+1;
                    bHSingleSetupModel.Add(ab);
                    i++;
                }           
            }catch( Exception ex){
                logWriter.WriteErrorLog(string.Format( "hrprofile : Exception!!! / {0}",ex.Message ));
                return null;
            }
            return bHSingleSetupModel;
        }

        internal SelectList getBranches() {
            ExceedConnectionDataContext exceedcnxn = new ExceedConnectionDataContext();
            var branches =  from s in exceedcnxn.vw_branch_analysis
                            where s.org_id.Equals(1)
                            orderby s.description ascending
                            select s;
            return new SelectList( branches , "analysis_det_code" , "Description" );
        }

        internal SelectList getHODepts(string brcode, string depts) {
            ExceedConnectionDataContext exceedcnxn = new ExceedConnectionDataContext();
            int[] _depts =  depts.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var depts_   =  from c in exceedcnxn.cm_departments
                            where c.org_id.Equals("001") &&  _depts.Contains(c.department_id)
                            orderby c.description ascending
                            select c;
            return new SelectList( depts_ , "department_id" , "description" );
        }

        internal SelectList getAppraisalPeriods( string periodSelectedValue ) {
            AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
            var periods =   from s in periodcnxn.Tb_App_Periods
                            where s.AppraisalStatus.Equals("ENABLED")
                            && s.CurrentStatus.Equals("ACTIVATED")
                            orderby s.AppraisalPeriod descending
                            select s;            
            return new SelectList( periods , "AppraisalPeriod" , "AppraisalTitle" , periodSelectedValue );
        }
        internal SelectList getRoles( string deptcode ) {
            AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
            var roles   =  (from c in periodcnxn.zib_appraisal_approver_roles
                            where c.deptcode.Equals(deptcode)
                           orderby c.role ascending
                           select c).Distinct();
            return new SelectList( roles , "roleid" , "role" );
        }

        internal SelectList getUnits( string deptcode ) {
            AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
            var units_   =  (from c in periodcnxn.zib_appraisal_dept_structures
                            where c.deptcode.Equals(deptcode)
                            select c).DistinctBy(m=>m.unitname).OrderBy(t=>t.unitname);
            return new SelectList( units_ , "unitcode" , "unitname" );
        }
        /*class ProductComparare : IEqualityComparer<string> {
            private Func<string, object> _funcDistinct;
            public ProductComparare(Func<string, object> funcDistinct) {
                this._funcDistinct = funcDistinct;
            }
            public bool Equals(string x, string y) {
                return _funcDistinct(x).Equals(_funcDistinct(y));
            }
            public int GetHashCode(string obj) {
                return this._funcDistinct(obj).GetHashCode();
            }
        }*/

        internal SelectList getDepts() {
            AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
            var depts_   =  (from c in periodcnxn.zib_appraisal_dept_structures
                             where !c.deptcode.Equals("000")
                            orderby c.deptname ascending
                            select c).DistinctBy(m=>m.deptname).OrderBy(t=>t.deptname);
            return new SelectList( depts_ , "deptcode" , "deptname" );
        }
        internal SelectList getRequestStages() {
            AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
            var stages_ =   from c in periodcnxn.zib_workflow_stages
                            where c.appid.ToUpper().Equals( DataHandlers.APP_ID.ToUpper() )
                            orderby c.requeststage ascending
                            select c;
            return new SelectList( stages_ , "requeststageid" , "requeststage" );
        }
        internal object getUnitsAsJSON( string deptcode , string _deptcode ) {
            AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
            var units_   =  (from c in periodcnxn.zib_appraisal_dept_structures
                            where (!deptcode.Equals( HOBCODE ) ? c.deptcode.Equals(deptcode) : c.groupcode.Equals(deptcode) && c.deptcode.Equals(_deptcode) )
                            select new{unitcode=c.unitcode,unitname=c.unitname})
                            .DistinctBy(m=>m.unitname).OrderBy(t=>t.unitname).ToList();
            if( units_.Any() ){
                return units_.ToArray();
            } else {
                return null;
            }
        }
        internal object getRolesAsJSON( string deptcode ) {
            AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
            var roles   =   (from c in periodcnxn.zib_appraisal_approver_roles
                            orderby c.role ascending
                            where c.deptcode.Equals(deptcode)
                            select new{roleid=c.roleid,role=c.role}).OrderBy(t=>t.role).ToList();
            if( roles.Any() ){
                return roles.ToArray();
            } else {
                return null;
            }
        }
        internal string getAppraisalPeriodTitle( string periodSelectedValue ) {
            AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
            var periods =   from s in periodcnxn.Tb_App_Periods
                            where s.AppraisalStatus.Equals("ENABLED")
                            && s.CurrentStatus.Equals("ACTIVATED")
                            && s.AppraisalPeriod.Equals(periodSelectedValue)
                            orderby s.AppraisalPeriod descending
                            select s.AppraisalTitle;            
            return periods.First().ToString();
        }

        internal StaffADProfile setInitiatorFields(StaffADProfile staffADProfile) {
            AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
            var setup = (from s in periodcnxn.Tb_TargetInitiators
                        join t in periodcnxn.Tb_App_Periods on s.appraisalperiod equals t.AppraisalPeriod
                        where s.initiatornumber.Equals(staffADProfile.employee_number)
                        && s.appraisalperiod.Equals(t.AppraisalPeriod)
                        && t.AppraisalStatus.Equals("ENABLED")
                        && t.CurrentStatus.Equals("ACTIVATED")
                        orderby s.deptname ascending
                        select new 
                            {                                                
                                branchname  = s.deptname,
                                branchcode  = s.deptcode.ToString(),
                                department  = s.unitname,
                                deptcode    = s.unitcode,
                                appperiod   = s.appraisalperiod
                            }).FirstOrDefault();
            if( setup!=null ){
                staffADProfile.branch_name  = setup.branchname;
                staffADProfile.branch_code  = setup.branchcode;
                staffADProfile.hodeptname   = setup.department;
                staffADProfile.hodeptcode   = setup.deptcode;
                staffADProfile.appperiod    = setup.appperiod;
            } else {
                staffADProfile.branch_name  = null;
                staffADProfile.branch_code  = null;
                staffADProfile.hodeptname   = null;
                staffADProfile.hodeptcode   = null;
                staffADProfile.appperiod    = null;
            }

            return staffADProfile;
            
        }
        
        internal StaffADProfile setApproverFields(StaffADProfile staffADProfile) {
            ExceedConnectionDataContext exceedcnxn = new ExceedConnectionDataContext();
            AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
            var setup = (from x in exceedcnxn.vw_employeeinfos
                         where x.employee_number.Equals( staffADProfile.employee_number )
                         select new 
                            {                                                
                                branchname  = x.Branch,
                                branchcode  = x.Branch_code,
                                department  = x.dept,
                                deptcode    = x.department_code
                            }).FirstOrDefault();
            if( setup!=null ){
                staffADProfile.branch_name  = setup.branchname;
                staffADProfile.branch_code  = setup.branchcode;
                staffADProfile.hodeptname   = setup.department;
                staffADProfile.hodeptcode   = setup.deptcode;
            } else {
                staffADProfile.branch_name  = null;
                staffADProfile.branch_code  = null;
                staffADProfile.hodeptname   = null;
                staffADProfile.hodeptcode   = null;
                staffADProfile.appperiod    = null;
            }
            return staffADProfile;            
        }
        
        internal List<RequestDetails> getMarketingStaff_Branch(StaffADProfile staffADProfile) {            
            ExceedConnectionDataContext exceedcnxn = new ExceedConnectionDataContext();
            var marketers   =   from v in exceedcnxn.vw_employeeinfos
                                where v.Category.Equals("marketing")
                                && v.Branch_code.Equals(staffADProfile.branch_code) 
                                && !v.jobtitle.Contains("BRANCH HEAD")
                                && !v.jobtitle.Contains("BRANCH HEAD/ZONAL HEAD")
                                && !v.jobtitle.Contains("ZONAL HEAD")
                                && !v.jobtitle.Contains("GROUP ZONAL HEAD")
                                && !v.jobtitle.Contains("GROUP HEAD")
                                && !v.jobtitle.Contains("ACTING BRANCH HEAD")
                                && ( v.org_id.Equals(1) )
                                orderby v.grade_id ascending
                                select new RequestDetails {
                                    employee_number = v.employee_number,
                                    name            = v.name,
                                    grade           = v.grade_code,
                                    entry_key       = v.employee_number+"_"+staffADProfile.appperiod

                                };
            return  marketers.ToList(); 
        }
        internal List<RequestDetails> getMarketingStaff_HO(StaffADProfile staffADProfile) {            
            ExceedConnectionDataContext exceedcnxn = new ExceedConnectionDataContext();
            var marketers   =   from v in exceedcnxn.vw_employeeinfos
                                where v.Branch_code.Equals("001")
                                //&& v.Branch_code.Equals(staffADProfile.branch_code) 
                                && v.department_id.Equals(staffADProfile.hodeptcode)
                                && !v.jobtitle.Contains("BRANCH HEAD")
                                && !v.jobtitle.Contains("BRANCH HEAD/ZONAL HEAD")
                                && !v.jobtitle.Contains("ZONAL HEAD")
                                && !v.jobtitle.Contains("GROUP ZONAL HEAD")
                                && !v.jobtitle.Contains("GROUP HEAD")
                                && !v.jobtitle.Contains("ACTING BRANCH HEAD")
                                && !v.jobtitle.Contains("BRANCH HEAD/DEPUTY ZONAL HEAD")
                                && ( v.org_id.Equals(1) )
                                orderby v.name ascending
                                select new RequestDetails {
                                    employee_number = v.employee_number,
                                    name            = v.name,
                                    grade           = v.grade_code,
                                    entry_key       = v.employee_number+"_"+staffADProfile.appperiod
                                };
            return  marketers.ToList(); 
        }

        internal List<RequestDetails> getExistingTargetEntry(StaffADProfile staffADProfile) {
            AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
            var entries =   from t in periodcnxn.Tb_TargetEntries
                            join a in periodcnxn.Tb_App_Periods on t.appperiod equals a.AppraisalPeriod
                            where t.deptcode.Equals(staffADProfile.branch_code)
                            //&& s.AppraisalPeriod.Equals(t.AppraisalPeriod)
                            && a.AppraisalStatus.Equals("ENABLED")
                            && a.CurrentStatus.Equals("ACTIVATED")
                            orderby t.name ascending
                            select new RequestDetails
                                {    
                                    workflowid      = t.workflowid,
                                    requeststageid  = t.requeststageid,
                                    requeststage    = t.requeststage,
                                    requestdate     = t.requestdate,
                                    employee_number = t.employee_number,
                                    name            = t.name,
                                    grade           = t.grade,
                                    cabal           = t.cabal,
                                    cabal_l         = t.cabal_l,
                                    sabal           = t.sabal,
                                    sabal_l         = t.sabal_l,
                                    fx              = t.fx,
                                    rv              = t.rv,
                                    fd              = t.fd,
                                    inc             = t.inc,
                                    inc_l           = t.inc_l,
                                    entry_key       = t.entry_key
                                };
            return entries.ToList();
        }



        internal List<RequestDetails> getExistingHOTargetEntry(StaffADProfile staffADProfile)
        {
            AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
            var entries = from t in periodcnxn.Tb_TargetEntries
                          join a in periodcnxn.Tb_App_Periods on t.appperiod equals a.AppraisalPeriod
                          where t.deptcode.Equals(staffADProfile.hodeptname)
                              //&& s.AppraisalPeriod.Equals(t.AppraisalPeriod)
                          && a.AppraisalStatus.Equals("ENABLED")
                          && a.CurrentStatus.Equals("ACTIVATED")
                          orderby t.name ascending
                          select new RequestDetails
                          {
                              workflowid = t.workflowid,
                              requeststageid = t.requeststageid,
                              requeststage = t.requeststage,
                              requestdate = t.requestdate,
                              employee_number = t.employee_number,
                              name = t.name,
                              grade = t.grade,
                              cabal = t.cabal,
                              cabal_l = t.cabal_l,
                              sabal = t.sabal,
                              sabal_l = t.sabal_l,
                              fx = t.fx,
                              rv = t.rv,
                              fd = t.fd,
                              inc = t.inc,
                              inc_l = t.inc_l,
                              entry_key = t.entry_key
                          };
            return entries.ToList();
        }


        internal List<RequestDetails> getExistingTargetEntry(string workflowid,string staffnumber) {
            AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
            var entries =   from t in periodcnxn.Tb_TargetEntries
                            join m in periodcnxn.zib_workflow_masters on t.workflowid equals m.workflowid
                            from a in periodcnxn.zib_appraisal_approvers
                                .Where(a => a.approverid.Equals(staffnumber) && a.roleid.Equals(m.requeststageid))
                                .DefaultIfEmpty()
                            where m.workflowid.Equals(workflowid)
                            orderby t.name ascending
                            select new RequestDetails
                                {    
                                    workflowid      = m.workflowid,
                                    requeststageid  = m.requeststageid,
                                    requeststage    = m.requeststage,
                                    requestdate     = m.createdt,
                                    employee_number = t.employee_number,
                                    name            = t.name,
                                    grade           = t.grade,
                                    cabal           = t.cabal,
                                    cabal_l         = t.cabal_l,
                                    sabal           = t.sabal,
                                    sabal_l         = t.sabal_l,
                                    fx              = t.fx,
                                    rv              = t.rv,
                                    fd              = t.fd,
                                    inc             = t.inc,
                                    inc_l           = t.inc_l,
                                    entry_key       = t.entry_key
                                };
            return entries.DistinctBy(c => c.employee_number).ToList();
        }
        // && t.entry_key equals entrykey
        internal List<RequestDetails> getExistingTargetEntry(string workflowid,string staffnumber,string entry_key) {
            AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
            var entries =   from t in periodcnxn.Tb_TargetEntries
                            join m in periodcnxn.zib_workflow_masters on t.workflowid equals m.workflowid
                            //join m in periodcnxn.zib_workflow_masters.Where( x => x.workflowid == t.workflowid && x.Completed == true )
                            from a in periodcnxn.zib_appraisal_approvers
                                .Where(a => a.approverid.Equals(staffnumber) && a.roleid.Equals(m.requeststageid))
                                .DefaultIfEmpty()
                            where m.workflowid.Equals(workflowid)
                            //&& t.entry_key==entry_key
                            orderby t.name ascending
                            select new RequestDetails
                                {    
                                    workflowid      = m.workflowid,
                                    requeststageid  = m.requeststageid,
                                    requeststage    = m.requeststage,
                                    requestdate     = m.createdt,
                                    employee_number = t.employee_number,
                                    name            = t.name,
                                    grade           = t.grade,
                                    cabal           = t.cabal,
                                    cabal_l         = t.cabal_l,
                                    sabal           = t.sabal,
                                    sabal_l         = t.sabal_l,
                                    fx              = t.fx,
                                    rv              = t.rv,
                                    fd              = t.fd,
                                    inc             = t.inc,
                                    inc_l           = t.inc_l,
                                    entry_key       = t.entry_key
                                };
            return entries.DistinctBy(c=>c.employee_number).ToList();
        }

        internal List<MyMemoEntriesModel> getMyMemoWorkflows( StaffADProfile staffADProfile ) {
            AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
            var entries =   from w in workflowcnxn.zib_workflow_masters
                            from t in workflowcnxn.zib_memo_entries
                                    .Where( tt => tt.originator_number.Equals(staffADProfile.employee_number) 
                                        && tt.workflowid.Equals( w.workflowid )
                                        //&& tt.workflowid.Equals(staffADProfile.employee_number)
                                        )
                                    //.Take( 1 )
                            where w.appid.Equals(DataHandlers.APP_ID)
                            && w.initiatornumber.Equals( staffADProfile.employee_number )
                            orderby w.createdt descending
                            select new MyMemoEntriesModel
                                {    
                                    WorkflowID      = w.workflowid,
                                    StaffNumber     = w.initiatornumber,
                                    StaffName       = w.initiatorname,
                                    Branch          = w.deptname,
                                    BranchCode      = w.deptcode,
                                    DeptName        = (w.deptcode.Equals("001")) ? w.unitname: w.deptname,
                                    DeptCode        = (w.deptcode.Equals("001")) ? w.unitname : w.deptcode,
                                    //AppraisalPeriod = t.appperiod,
                                    /*AppraisalPeriodName     
                                                    = getAppraisalPeriodTitle( t.appperiod ),*/
                                    RequestStage    = w.requeststage,
                                    RequestStageId  = w.requeststageid,
                                    UploadStatus    = t.uploadstatus,
                                    DateSubmitted   = w.createdt,
                                    Approvers       = w.approvalhistory.ToString(),
                                    Audit           = w.audithistory.ToString(),
                                    Action          = "View",
                                    Subject         = t.subject,
                                    From            = t.@from,
                                    To              = t.to,
                                    DateOfMemo      = t.dateofmemo.Date

                                };
            return entries.ToList();
        }

        /*internal List<EntriesModel> getMyTargetWorkflows(StaffADProfile staffADProfile) {
            AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
            var entries =   from w in workflowcnxn.zib_workflow_masters
                            from t in workflowcnxn.Tb_TargetEntries
                                    .Where( tt => tt.hr_uploader_id.Equals(staffADProfile.employee_number) && tt.appperiod.Equals(staffADProfile.appperiod ))
                                    .Take( 1 )
                            where w.appid.Equals(DataHandlers.APP_ID)
                            && w.initiatornumber.Equals(staffADProfile.employee_number)
                            orderby w.createdt descending
                            select new EntriesModel
                                {    
                                    WorkflowID     = w.workflowid,
                                    StaffNumber     = w.initiatornumber,
                                    StaffName       = w.initiatorname,
                                    Branch          = w.deptname,
                                    BranchCode      = w.deptcode,
                                    DeptName        = (w.deptcode.Equals("001"))? w.unitname: w.deptname,
                                    DeptCode        = (w.deptcode.Equals("001")) ? w.unitname : w.deptcode,
                                    AppraisalPeriod = t.appperiod,
                                    AppraisalPeriodName     
                                                    = getAppraisalPeriodTitle( t.appperiod ),
                                    RequestStage    = w.requeststage,
                                    RequestStageId  = w.requeststageid,
                                    UploadStatus    = t.target_status,
                                    DateSubmitted   = w.createdt,
                                    Approvers       = w.approvalhistory.ToString(),
                                    Audit           = w.audithistory.ToString(),
                                    Action          = "View"
                                };
            return entries.ToList();
        } */    
   
        internal List<MyMemoEntriesModel> getMyPendingMemoWorkflows(StaffADProfile staffADProfile) {
            
            logWriter.WriteErrorLog(string.Format(" getMyPendingMemoWorkflows :inside getMyPendingMemoWorkflows!!! / {0}", staffADProfile.user_logon_name));
            List<MyMemoEntriesModel> entrym = new List<MyMemoEntriesModel>();
            
            try {
            
                AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
                var entries =   from w in workflowcnxn.zib_workflow_masters
                                from t in workflowcnxn.zib_memo_entries
                                        .Where( tt => tt.workflowid.Equals(w.workflowid) )
                                        .Take( 1 )
                                where w.appid.Equals( DataHandlers.APP_ID )
                                && w.requeststageid.Equals( 1 )
                                && ( t.approvers.Contains( staffADProfile.employee_number ) )
                                orderby w.createdt descending
                                select new MyMemoEntriesModel
                                    {    
                                        WorkflowID      = w.workflowid,
                                        StaffNumber     = w.initiatornumber,
                                        StaffName       = w.initiatorname,
                                        Branch          = w.deptname,
                                        BranchCode      = w.deptcode,
                                        DeptName        = (w.deptcode.Equals("001")) ? w.unitname: w.deptname,
                                        DeptCode        = (w.deptcode.Equals("001")) ? w.unitname : w.deptcode,
                                        RequestStage    = w.requeststage,
                                        RequestStageId  = w.requeststageid,
                                        UploadStatus    = t.uploadstatus,
                                        DateSubmitted   = w.createdt,
                                        Approvers       = w.approvalhistory.ToString(),
                                        Audit           = w.audithistory.ToString(),
                                        Action          = "View",
                                        Subject         = t.subject,
                                        From            = t.@from,
                                        To              = t.to,
                                        DateOfMemo      = t.dateofmemo.Date

                                    };
                logWriter.WriteErrorLog(string.Format("get Entry List Count : Exception!!! / {0}", entries.Count()));

                return entries.ToList();
            } catch (Exception ex) {
                logWriter.WriteErrorLog(string.Format("getMyPendingMemoWorkflows : Exception!!! / {0}", ex.Message));
                return entrym.ToList();
            }
        }

        internal List<EntriesModel> getMyPendingTargetWorkflows(StaffADProfile staffADProfile) {
            logWriter.WriteErrorLog(string.Format(" getMyPendingTargetWorkflows :inside getMyPendingTargetWorkflows!!! / {0}", staffADProfile.user_logon_name));
            List<EntriesModel> entrym = new List<EntriesModel>();
            try
            {

            
            AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
            var entries =   from w in workflowcnxn.zib_workflow_masters
                            from a in workflowcnxn.zib_appraisal_approvers
                            from t in workflowcnxn.Tb_TargetEntries
                                    .Where( tt => tt.workflowid.Equals(w.workflowid) )
                                    .Take( 1 )
                            where w.appid.Equals( DataHandlers.APP_ID )
                            && w.requeststageid.Equals(a.roleid)
                            && (a.approverid.Equals(staffADProfile.employee_number) && (a.deptcode.Equals(w.deptcode)|| w.unitname.Equals(a.deptname) ))
                            //&& w.deptcode.Equals(staffADProfile.branch_code)
                            orderby w.createdt descending
                            select new EntriesModel
                                {    
                                    WorkflowID      = w.workflowid,
                                    StaffNumber     = w.initiatornumber,
                                    StaffName       = w.initiatorname,
                                    Branch          = w.deptname,
                                    BranchCode      = w.deptcode,
                                    DeptName        = (w.deptcode.Equals("001")) ? w.unitname : w.deptname,
                                    DeptCode        = (w.deptcode.Equals("001")) ? w.unitname : w.deptcode,
                                    AppraisalPeriod = t.appperiod,
                                    AppraisalPeriodName     
                                                    = getAppraisalPeriodTitle( t.appperiod ),
                                    RequestStage    = w.requeststage,
                                    RequestStageId  = w.requeststageid,
                                    UploadStatus    = t.target_status,
                                    DateSubmitted   = w.createdt,
                                    Approvers       = w.approvalhistory.ToString(),
                                    Audit           = w.audithistory.ToString(),
                                    Action          = "View"
                                };
            logWriter.WriteErrorLog(string.Format("get Entry List Count : Exception!!! / {0}", entries.Count()));

            return entries.ToList();
            }
            catch (Exception ex)
            {
                logWriter.WriteErrorLog(string.Format("getMyPendingTargetWorkflows : Exception!!! / {0}", ex.Message));
                return entrym.ToList();
            }
        }

        internal List<EntriesModel> getMyApprovedTargetWorkflows(StaffADProfile staffADProfile) {
            logWriter.WriteErrorLog(string.Format(" getMyApprovedTargetWorkflows :inside getMyApprovedTargetWorkflows!!! / {0}", staffADProfile.user_logon_name));
            
            staffADProfile.employee_number ="2002205";

            List<EntriesModel> entrym = new List<EntriesModel>();
            try {
                AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
                var entries =   from w in workflowcnxn.zib_workflow_masters
                                from a in workflowcnxn.zib_appraisal_approvers
                                from t in workflowcnxn.Tb_TargetEntries
                                        .Where( tt => tt.workflowid.Equals(w.workflowid) )
                                        .Take( 1 )
                                /*from el in w.approvalhistory.Descendants("Approvals")
                                        .Where(dd => dd.Element("ApproverStaffNumber").Value.Equals(staffADProfile.employee_number))*/
                                where w.appid.Equals( DataHandlers.APP_ID )
                                //&& w.requeststageid.Equals(a.roleid)
                                //&& (a.approverid.Equals(staffADProfile.employee_number) && (a.deptcode.Equals(w.deptcode)|| w.unitname.Equals(a.deptname) ))
                                && w.approvalhistory.ToString().Contains(staffADProfile.employee_number)
                                orderby w.createdt descending
                                select new EntriesModel
                                    {    
                                        WorkflowID      = w.workflowid,
                                        StaffNumber     = w.initiatornumber,
                                        StaffName       = w.initiatorname,
                                        Branch          = w.deptname,
                                        BranchCode      = w.deptcode,
                                        DeptName        = (w.deptcode.Equals("001")) ? w.unitname : w.deptname,
                                        DeptCode        = (w.deptcode.Equals("001")) ? w.unitname : w.deptcode,
                                        AppraisalPeriod = t.appperiod,
                                        AppraisalPeriodName     
                                                        = getAppraisalPeriodTitle( t.appperiod ),
                                        RequestStage    = w.requeststage,
                                        RequestStageId  = w.requeststageid,
                                        UploadStatus    = t.target_status,
                                        DateSubmitted   = w.createdt,
                                        Approvers       = w.approvalhistory.ToString(),
                                        Audit           = w.audithistory.ToString(),
                                        Action          = "View"
                                    };
                logWriter.WriteErrorLog(string.Format("get Entry List Count : Exception!!! / {0}", entries.Count()));

                return entries.ToList();
            } catch (Exception ex) {
                logWriter.WriteErrorLog(string.Format("getMyPendingTargetWorkflows : Exception!!! / {0}", ex.Message));
                return entrym.ToList();
            }
        }
        internal MemoSetup getWorkflowEntry( string workflowid ) {
            AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
            var entries =   from w in workflowcnxn.zib_workflow_masters
                            from t in workflowcnxn.zib_memo_entries
                                    .Where( tt => tt.workflowid.Equals(workflowid ))
                                    .Take( 1 )
                            where w.workflowid.Equals(workflowid)
                            orderby w.createdt descending
                            select new MemoSetup
                                {    
                                    WorkflowID      = w.workflowid,
                                    OriginatorNumber= w.initiatornumber,
                                    OriginatorName  = w.initiatorname,
                                    Branch          = w.deptname,
                                    BranchCode      = w.deptcode,
                                    DeptName        = (w.deptcode.Equals("001")) ? w.unitname : w.deptname,
                                    DeptCode        = (w.deptcode.Equals("001")) ? w.unitname : w.deptcode,

                                    From            = t.@from,
                                    To              = t.to,
                                    Attn            = t.attn,
                                    Subject         = t.subject,
                                    MemoBody        = t.memobody,
                                    CCFields        = t.ccfields,
                                    DateOfMemo      = t.dateofmemo.Date.ToString(),

                                    RequestStage    = w.requeststage,
                                    RequestStageId  = w.requeststageid,
                                    UploadStatus    = t.uploadstatus,
                                    DateSubmitted   = w.createdt,
                                    Approvers       = w.approvalhistory.ToString(),
                                    Audit           = w.audithistory.ToString(),
                                    Action          = "View",

                                    Signers         = t.approvers
                                };
            return entries.FirstOrDefault();
           
        }

        internal string getInitiatorNumber(string workflowid) {
            AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
            var entries =   from w in workflowcnxn.zib_workflow_masters
                            where w.workflowid.Equals(workflowid)
                            select w.initiatornumber;
            if( entries.Count()>0 ){
                return entries.First().ToString() ?? null;
            } else {
                return null;
            }
        }
        
        internal string getApprovers(string workflowid,int requeststageid) {
            AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
            var entries =   (from w in workflowcnxn.zib_memo_entries
                            where w.workflowid.Equals(workflowid)
                            select w.approvers).First().ToString();
            return entries;
        }
        internal string getMemoApproverNames(string workflowid,int requeststageid) {
            AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
            var entries =   from w in workflowcnxn.zib_memo_entries
                            //join a in workflowcnxn.zib_appraisal_user_roles on w.requeststageid equals a.roleid
                            where w.workflowid.Equals(workflowid)
                            orderby w.approvers ascending
                            select w.approvers;
            return entries.First().ToString();
        }
        internal XElement getApprovalHistory(string workflowid) {
            AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
            var entries =   (from w in workflowcnxn.zib_workflow_masters
                            where w.workflowid.Equals(workflowid)
                            select w.approvalhistory).First();
            return entries;
        }
        internal XElement getAuditHistory(string workflowid) {
            AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
            var entries =   (from w in workflowcnxn.zib_workflow_masters
                            where w.workflowid.Equals(workflowid)
                            select w.audithistory).First();
            return entries;
        }

        internal List<string> getApproverNumbersToNames(string workflowid,int requeststageid) {
            AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
            var entries =   ( requeststageid.Equals(-1) || requeststageid.Equals(0) ) ?                
                            from w in workflowcnxn.zib_workflow_masters
                            where w.workflowid.Equals(workflowid)
                            select w.initiatornumber
                            :
                            from w in workflowcnxn.zib_workflow_masters
                            join a in workflowcnxn.zib_appraisal_approvers on w.requeststageid equals a.roleid
                            where w.workflowid.Equals(workflowid)
                            && (w.deptcode.Equals(a.deptcode) || w.unitname.Equals(a.deptname) 
                                 /*&& w.unitcode.Equals(a.unitcode) 
                                 && w.groupcode.Equals(a.groupcode)
                                 && w.supergroupcode.Equals(a.supergroupcode)*/
                                )
                            select a.approverid;
            Debug.WriteLine(getStaffNames(entries.ToList()));
            return getStaffNames(entries.ToList());
        }

        private static List<string> getStaffNames( List<string> empnos ) {
            ExceedConnectionDataContext exceedcnxn      = new ExceedConnectionDataContext(); 
            return (from e in exceedcnxn.vw_employeeinfos where empnos.Contains(e.employee_number) select e.name).ToList();
        }

        internal bool checkDupApproverSetup( string entrykey ) {
            AppraisalConnectionDataContext periodcnxn = new AppraisalConnectionDataContext();
            var exists   =  from c in periodcnxn.zib_appraisal_approvers
                            where c.entrykey.ToUpper().Equals(entrykey)
                            select c;
            if( exists.Any() ){
                return false;
            } else {
                return true;
            }
        }

        internal AppraisalApproverModel getApproverSetupEntry(string entrykey) {
            AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
            var entry =   (from w in workflowcnxn.zib_appraisal_approvers
                            from t in workflowcnxn.zib_appraisal_approver_roles
                            where w.entrykey.Equals(entrykey)
                            orderby w.createdt
                            select new AppraisalApproverModel
                                {    
                                    EntryKey    = w.entrykey,
                                    StaffNumber = w.approverid,
                                    StaffName   = w.approvername,
                                    UnitCode    = w.unitcode,
                                    UnitTitle   = w.unitname,
                                    DeptCode    = w.deptcode,
                                    DeptTitle   = w.deptname,
                                    GroupCode   = w.groupcode,
                                    GroupTitle  = w.groupname,
                                    SuperGroupCode = w.supergroupcode,
                                    SuperGroupTitle= w.supergroupname,
                                    RoleTitle   = w.role,
                                    RoleID      = w.roleid
                                }).First();
            return entry;
        }

        internal List<AppraisalApproverModel> getApproverSetupList() {
            AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
            var entries =   (from w in workflowcnxn.zib_appraisal_approvers
                            from t in workflowcnxn.zib_appraisal_approver_roles
                            where w.roleid.Equals( t.roleid )
                            orderby w.createdt
                            select new AppraisalApproverModel
                                {    
                                    EntryKey    = w.entrykey,
                                    StaffNumber = w.approverid,
                                    StaffName   = w.approvername,
                                    UnitCode    = w.unitcode,
                                    UnitTitle   = w.unitname,
                                    DeptCode    = w.deptcode,
                                    DeptTitle   = w.deptname,
                                    GroupCode   = w.groupcode,
                                    GroupTitle  = w.groupname,
                                    SuperGroupCode = w.supergroupcode,
                                    SuperGroupTitle= w.supergroupname,
                                    RoleTitle   = w.role,
                                    RoleID      = w.roleid
                                }).DistinctBy(c=>c.EntryKey).ToList();
            return entries;
        }

        internal SelectList getHRRoles() {
            AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
            var hrroles =   from s in workflowcnxn.zib_appraisal_approver_roles
                            where s.deptcode.Equals("020")
                            orderby s.role ascending
                            select s;
            return new SelectList( hrroles , "roleid" , "role" );
        }

        internal List<AppraisalApproverModel> getHRUsers() {
            AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
            var entries =   (from w in workflowcnxn.zib_appraisal_user_roles
                            from t in workflowcnxn.zib_appraisal_approver_roles
                            where w.roleid.Equals( t.roleid )
                            && t.deptcode.Equals("020")
                            orderby w.createdt
                            select new AppraisalApproverModel
                                {    
                                    EntryKey    = w.entrykey,
                                    StaffName   = w.name,
                                    StaffNumber = w.employee_number,                                    
                                    RoleTitle   = w.role,
                                    RoleID      = w.roleid,
                                    StatusCode  = w.status,
                                    StatusTitle = w.status,
                                    HRStaffNumber = w.editedbyid,
                                    CreateDate  = w.createdt
                                }).DistinctBy(c=>c.EntryKey).ToList();
            return entries;
        }

        internal AppraisalApproverModel getRoleSetupEntry( string entrykey ) {
            AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
            var entry =   (from w in workflowcnxn.zib_appraisal_user_roles
                            from t in workflowcnxn.zib_appraisal_approver_roles
                            where w.entrykey.Equals(entrykey)
                            orderby w.createdt
                            select new AppraisalApproverModel
                                {    
                                    EntryKey    = w.entrykey,
                                    StaffName   = w.name,
                                    StaffNumber = w.employee_number,                                    
                                    RoleTitle   = w.role,
                                    RoleID      = w.roleid,
                                    StatusCode  = w.status,
                                    StatusTitle = w.status,
                                    HRStaffNumber = w.editedbyid,
                                    CreateDate  = w.createdt
                                }).First();
            return entry;
        }

        internal List<EntriesModel> getWorkflowReport( string ReportMode ) {

            AppraisalConnectionDataContext workflowcnxn = new AppraisalConnectionDataContext();
            var entries =   from w in workflowcnxn.zib_workflow_masters
                            //from a in workflowcnxn.zib_appraisal_approvers
                            from t in workflowcnxn.Tb_TargetEntries
                                    .Where( tt => tt.workflowid.Equals(w.workflowid ))
                                    .Take( 1 )
                            where w.appid.Equals(DataHandlers.APP_ID)
                            orderby w.createdt descending
                            select new EntriesModel
                                {    
                                    WorkflowID      = w.workflowid,
                                    StaffNumber     = w.initiatornumber,
                                    StaffName       = w.initiatorname,
                                    Branch          = w.deptname,
                                    BranchCode      = w.deptcode,
                                    UnitName        = w.unitname,
                                    DeptName = (w.deptcode.Equals("001")) ? w.unitname : w.deptname,
                                    DeptCode = (w.deptcode.Equals("001")) ? w.unitname : w.deptcode,
                                    GroupName       = w.groupname,
                                    SuperGroupName  = w.supergroupname,
                                    AppraisalPeriod = t.appperiod,
                                    AppraisalPeriodName     
                                                    = getAppraisalPeriodTitle( t.appperiod ),
                                    RequestStage    = w.requeststage,
                                    RequestStageId  = w.requeststageid,
                                    UploadStatus    = t.target_status,
                                    DateSubmitted   = w.createdt,
                                    Approvers       = w.approvalhistory.ToString(),
                                    Audit           = w.audithistory.ToString(),
                                    Action          = "View",
                                    EntryKey        = t.entry_key
                                };

            if( ReportMode.Equals(ALLAPPRVED) ){
                entries = entries.Where(r=>r.RequestStageId.Equals(100));
            }else if( ReportMode.Equals(ALLDENIALS) ){
                entries = entries.Where(r=>r.RequestStageId.Equals(-1));
            }else if( ReportMode.Equals(ALLHRUPLOAD) ){
                entries = entries.Where(r=>r.RequestStageId.Equals(20));
            }else if( ReportMode.Equals(ALLPENDING) ){
                entries = entries.Where( r=>!r.RequestStageId.Equals(100) && !r.RequestStageId.Equals(-1)  );
            }
            return entries.ToList();
        }

        internal List<EntriesModel> getWorkflowQueryReport(ReportModel reportModel) {
            
            List<EntriesModel> workflowReport = getWorkflowReport( reportModel.ReportMode );            
            switch (reportModel.QueryFieldTitle) {
                case "deptname":
                    workflowReport = workflowReport.Where(s=>s.DeptName.ToLower().Contains(reportModel.QueryText.ToLower())).ToList();
                    break;
                case "groupname":
                    workflowReport = workflowReport.Where(s=>s.GroupName.ToLower().Contains(reportModel.QueryText.ToLower())).ToList();
                    break;
                case "supergroupname":
                    workflowReport = workflowReport.Where(s=>s.SuperGroupName.ToLower().Contains(reportModel.QueryText.ToLower())).ToList();
                    break;
                case "staffnumber":
                    workflowReport = workflowReport.Where(s=>s.StaffNumber.ToLower().Contains(reportModel.QueryText.ToLower())).ToList();
                    break;
                case "staffname":
                    workflowReport = workflowReport.Where(s=>s.StaffName.ToLower().Contains(reportModel.QueryText.ToLower())).ToList();
                    break;
                case "requeststage":
                    workflowReport = workflowReport.Where(s=>s.RequestStage.ToLower().Contains(reportModel.QueryText.ToLower())).ToList();
                    break;
                case "appraisalperiod":
                    workflowReport = workflowReport.Where(s=>s.AppraisalPeriod.ToLower().Contains(reportModel.QueryText.ToLower())).ToList();
                    break;
                case "approverlist":
                    workflowReport = workflowReport.Where(s=>s.Approvers.ToLower().Contains(reportModel.QueryText.ToLower())).ToList();
                    break;
            }
            return workflowReport;
        }
        
        internal List<EntriesModel> getPendingHRUpload(StaffADProfile staffADProfile) {
            List<EntriesModel> workflowReport = getWorkflowReport( ALLHRUPLOAD );  
            return workflowReport;
        }
    }
}