using GeneralMemo.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralMemo.App_Code {
    class AppDatabase {

        private LogWriter logWriter;
        public AppDatabase() {
            this.logWriter = new LogWriter();
        }
        internal string getConnectionString( string serverName ) {
            string connectionString = "";
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[serverName];
            if (settings != null) {
                connectionString = settings.ConnectionString;
            }
            return connectionString;
        }

        internal string saveMemo( MemoSetup memoSetup, string ConnString, string Status ) {

            string retVal       = String.Empty;            
            string connString   = getConnectionString(ConnString);
            
            SqlConnection conn  = new SqlConnection(connString);
            SqlCommand cmnd     = new SqlCommand();
            
            cmnd.Connection     = conn;
            cmnd.CommandType    = CommandType.StoredProcedure;
            cmnd.CommandText    = "zsp_insert_memo";

            cmnd.Parameters.Add("@entry_key"        , SqlDbType.VarChar).Value  = memoSetup.EntryKey;
            cmnd.Parameters.Add("@workflowid"       , SqlDbType.VarChar).Value  = memoSetup.WorkflowID;
            
            cmnd.Parameters.Add("@requeststageid"   , SqlDbType.SmallInt).Value = memoSetup.RequestStageId;
            cmnd.Parameters.Add("@requeststage"     , SqlDbType.VarChar).Value  = memoSetup.RequestStage;
            cmnd.Parameters.Add("@originator_number", SqlDbType.VarChar).Value  = memoSetup.OriginatorNumber;
            cmnd.Parameters.Add("@originator_name"  , SqlDbType.VarChar).Value  = memoSetup.OriginatorName;
            cmnd.Parameters.Add("@grade"            , SqlDbType.VarChar).Value  = memoSetup.StaffADProfile.grade;
            cmnd.Parameters.Add("@deptname"         , SqlDbType.VarChar).Value  = memoSetup.DeptName;
            cmnd.Parameters.Add("@deptcode"         , SqlDbType.VarChar).Value  = memoSetup.DeptCode;
            cmnd.Parameters.Add("@branchname"       , SqlDbType.VarChar).Value  = memoSetup.Branch;
            cmnd.Parameters.Add("@branchcode"       , SqlDbType.VarChar).Value  = memoSetup.BranchCode;
            cmnd.Parameters.Add("@unitname"         , SqlDbType.VarChar).Value  = memoSetup.UnitName;
            cmnd.Parameters.Add("@unitcode"         , SqlDbType.VarChar).Value  = memoSetup.UnitCode;

            cmnd.Parameters.Add("@from"             , SqlDbType.VarChar).Value  = memoSetup.From.ToUpper();
            cmnd.Parameters.Add("@to"               , SqlDbType.VarChar).Value  = memoSetup.To.ToUpper();
            cmnd.Parameters.Add("@attn"             , SqlDbType.VarChar).Value  = memoSetup.Attn.ToUpper();

            Debug.WriteLine(DateTime.ParseExact(memoSetup.DateOfMemo,"dddd MMMM d, yyyy",CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss.mm"));
            cmnd.Parameters.Add("@dateofmemo"       , SqlDbType.DateTime).Value = DateTime.ParseExact(memoSetup.DateOfMemo,"dddd MMMM d, yyyy",CultureInfo.InvariantCulture).ToString("s");

            /*
            DateTime myDateTime = DateTime.ParseExact(memoSetup.DateOfMemo,"dddd MMMM d, yyyy",CultureInfo.InvariantCulture);
            Debug.WriteLine(myDateTime);
            myDateTime = DateTime.Now;
            Debug.WriteLine(myDateTime);
            string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            Debug.WriteLine(sqlFormattedDate);
            cmnd.Parameters.Add("@dateofmemo"       , SqlDbType.DateTime).Value = sqlFormattedDate;
            */
            
            cmnd.Parameters.Add("@subject"          , SqlDbType.VarChar).Value  = memoSetup.Subject.ToUpper();
            cmnd.Parameters.Add("@memobody"         , SqlDbType.NVarChar).Value = memoSetup.MemoBody;
            cmnd.Parameters.Add("@uploadstatus"     , SqlDbType.VarChar).Value  = memoSetup.UploadStatus;

            cmnd.Parameters.Add("@approvers"        , SqlDbType.VarChar).Value  = memoSetup.Signers.ToUpper() ?? "";

            string CCFields = memoSetup.CCFields ?? "";
            cmnd.Parameters.Add("@ccfields"         , SqlDbType.VarChar).Value  = CCFields.ToUpper();
            //cmnd.Parameters.Add("@ccfields"         , SqlDbType.VarChar).Value  = "";
            cmnd.Parameters.Add("@comments"         , SqlDbType.VarChar).Value  = memoSetup.Comments ?? "";

            cmnd.Parameters.Add("@appid"            , SqlDbType.VarChar).Value  = DataHandlers.APP_ID;
            
            cmnd.Parameters.Add("@rErrorCode"       , SqlDbType.Int,2).Direction        = ParameterDirection.Output;
            cmnd.Parameters.Add("@rErrorMsg"        , SqlDbType.VarChar,255).Direction  = ParameterDirection.Output;
            
            Debug.WriteLine(memoSetup);

            SqlDataReader dr;
               
            try {
                // Open the data connection
                cmnd.Connection = conn;
                conn.Open();

                dr = cmnd.ExecuteReader(); 

                int retCode = int.Parse(cmnd.Parameters["@rErrorCode"].Value.ToString());
                if ( retCode!=0 ) {
                    retVal = retCode+"_"+cmnd.Parameters["@rErrorMsg"].Value.ToString();
                }
            
            } catch (SqlException ex) {
                if ( ex.Number!=0 ) {
                    //retVal = ex.Number+"|"+ex.Message;
                    retVal +="_true";
                    logWriter.WriteErrorLog(string.Format( "saveMemo : Exception!!! / {0}",retVal));
                }
            } finally {
                conn.Close();
                cmnd.Dispose();
                dr=null;
            }
            return retVal;
        }

        internal int insertSingleSetup( BHSingleSetupModel bHSingleSetupModel , HRProfile hrprofile  , int inputMode , string ConnString ) {

            Debug.WriteLine(hrprofile.name);
            Debug.WriteLine(hrprofile.employee_number); 

            string retVal       = "";            
            string connString   = getConnectionString(ConnString);
            
            SqlConnection conn  = new SqlConnection(connString);
            SqlCommand cmnd     = new SqlCommand();
            
            cmnd.Connection     = conn;
            cmnd.CommandType    = CommandType.StoredProcedure;
            cmnd.CommandText    = "zsp_insert_single_setup";

            cmnd.Parameters.Add("@unitcode"         , SqlDbType.VarChar).Value = bHSingleSetupModel.HODeptCode;
            cmnd.Parameters.Add("@unitname"         , SqlDbType.VarChar).Value  = bHSingleSetupModel.SetupDept;
            cmnd.Parameters.Add("@deptcode"         , SqlDbType.VarChar).Value = bHSingleSetupModel.SelectedBranch;
            cmnd.Parameters.Add("@deptname"         , SqlDbType.VarChar).Value  = bHSingleSetupModel.SetupBranch;
            cmnd.Parameters.Add("@initiatorname"    , SqlDbType.VarChar).Value  = bHSingleSetupModel.StaffName;
            cmnd.Parameters.Add("@initiatornumber"  , SqlDbType.VarChar).Value  = bHSingleSetupModel.StaffNumber;
            cmnd.Parameters.Add("@comments"         , SqlDbType.VarChar).Value  = bHSingleSetupModel.Comments ?? "";
            cmnd.Parameters.Add("@hrstaffnumber"    , SqlDbType.VarChar).Value  = hrprofile.employee_number;
            cmnd.Parameters.Add("@hrstaffname"      , SqlDbType.VarChar).Value  = hrprofile.name;
            cmnd.Parameters.Add("@appraisalperiod"  , SqlDbType.VarChar).Value  = bHSingleSetupModel.SelectedAppraisalPeriod;
            cmnd.Parameters.Add("@InputMode"        , SqlDbType.SmallInt).Value = inputMode;
            
            cmnd.Parameters.Add("@rErrorCode"       , SqlDbType.Int,2).Direction=ParameterDirection.Output;
            cmnd.Parameters.Add("@rErrorMsg"        , SqlDbType.VarChar,255).Direction=ParameterDirection.Output;
            

            SqlDataReader dr;
               
            try {
                // Open the data connection
                cmnd.Connection = conn;
                conn.Open();

                dr = cmnd.ExecuteReader(); 

                int retCode = int.Parse(cmnd.Parameters["@rErrorCode"].Value.ToString());
                if ( retCode!=0 ) {
                    retVal = retCode+"|"+cmnd.Parameters["@rErrorMsg"].Value.ToString();
                }
            
            } catch (SqlException ex) {

                if ( ex.Number!=0 ) {
                    retVal = ex.Number+"|"+ex.Message;
                }
                logWriter.WriteErrorLog(string.Format( "insertSingleSetup : Exception!!! / {0}",retVal));
            }

            return 0;
        }

        internal int insertSingleSetup( BHSingleSetupEditModel bHSingleSetupEditModel , HRProfile hrprofile  , int inputMode , string ConnString ) {

            Debug.WriteLine(hrprofile.name);
            Debug.WriteLine(hrprofile.employee_number); 

            string retVal       = "";            
            string connString   = getConnectionString(ConnString);
            
            SqlConnection conn  = new SqlConnection(connString);
            SqlCommand cmnd     = new SqlCommand();
            
            cmnd.Connection     = conn;
            cmnd.CommandType    = CommandType.StoredProcedure;
            cmnd.CommandText    = "zsp_insert_single_setup";

            cmnd.Parameters.Add("@deptcode"         , SqlDbType.VarChar).Value = bHSingleSetupEditModel.SetupBranchCode;
            cmnd.Parameters.Add("@deptname"         , SqlDbType.VarChar).Value  = bHSingleSetupEditModel.SetupBranch;
            cmnd.Parameters.Add("@unitcode"         , SqlDbType.VarChar).Value  = bHSingleSetupEditModel.HODeptCode;
            cmnd.Parameters.Add("@unitname"         , SqlDbType.VarChar).Value  = bHSingleSetupEditModel.SetupDept;
            cmnd.Parameters.Add("@initiatorname"    , SqlDbType.VarChar).Value  = bHSingleSetupEditModel.StaffName;
            cmnd.Parameters.Add("@initiatornumber"  , SqlDbType.VarChar).Value  = bHSingleSetupEditModel.StaffNumber;
            cmnd.Parameters.Add("@comments"         , SqlDbType.VarChar).Value  = bHSingleSetupEditModel.Comments ?? "";
            cmnd.Parameters.Add("@hrstaffnumber"    , SqlDbType.VarChar).Value  = hrprofile.employee_number;
            cmnd.Parameters.Add("@hrstaffname"      , SqlDbType.VarChar).Value  = hrprofile.name;
            cmnd.Parameters.Add("@appraisalperiod"  , SqlDbType.VarChar).Value  = bHSingleSetupEditModel.SelectedAppraisalPeriod;
            cmnd.Parameters.Add("@inputmode"        , SqlDbType.SmallInt).Value = inputMode;
            
            cmnd.Parameters.Add("@rErrorCode"       , SqlDbType.Int,2).Direction=ParameterDirection.Output;
            cmnd.Parameters.Add("@rErrorMsg"        , SqlDbType.VarChar,255).Direction=ParameterDirection.Output;
            

            SqlDataReader dr;
               
            try {
                // Open the data connection
                cmnd.Connection = conn;
                conn.Open();

                dr = cmnd.ExecuteReader(); 

                int retCode = int.Parse(cmnd.Parameters["@rErrorCode"].Value.ToString());
                if ( retCode!=0 ) {
                    retVal = retCode+"|"+cmnd.Parameters["@rErrorMsg"].Value.ToString();
                }
            
            } catch (SqlException ex) {
                if ( ex.Number!=0 ) {
                    retVal = ex.Number+"|"+ex.Message;
                    logWriter.WriteErrorLog(string.Format( "insertSingleSetup : Exception!!! / {0}",retVal));
                }
            } finally {
                conn.Close();
                cmnd.Dispose();
                dr=null;
            }

            return 0;
        }


        internal string insertBulkSetup( DataTable dataTable , string ConnString ) {

            string retVal       = null;            
            string connString   = getConnectionString(ConnString);
            
            SqlConnection conn  = new SqlConnection(connString);
            SqlCommand cmnd     = new SqlCommand();
            
            cmnd.Connection     = conn;
            cmnd.CommandType    = CommandType.StoredProcedure;
            cmnd.CommandText    = "zsp_insert_bulk_setup";

            SqlParameter parameter  = cmnd.CreateParameter();
            parameter.ParameterName = "@tvpTargetInitiators";
            parameter.Value         = dataTable;
            parameter.SqlDbType     = SqlDbType.Structured;
            parameter.TypeName      = "dbo.TargetInitiatorsType";
            cmnd.Parameters.Add(parameter);
            
            cmnd.Parameters.Add("@rErrorCode"   , SqlDbType.Int,2).Direction=ParameterDirection.Output;
            cmnd.Parameters.Add("@rErrorMsg"    , SqlDbType.VarChar,255).Direction=ParameterDirection.Output;            

            SqlDataReader dr;
               
            try {
                // Open the data connection
                cmnd.Connection = conn;
                conn.Open();

                dr = cmnd.ExecuteReader(); 

                int retCode = int.Parse(cmnd.Parameters["@rErrorCode"].Value.ToString());
                if ( retCode!=0 ) {
                    retVal = retCode+"|"+cmnd.Parameters["@rErrorMsg"].Value.ToString();
                }
            
            } catch (SqlException ex) {
                if ( ex.Number!=0 ) {
                    retVal = ex.Number+"|"+ex.Message;
                    logWriter.WriteErrorLog(string.Format( "insertBulkSetup : Exception!!! / {0}",retVal));
                }
            } finally {
                conn.Close();
                cmnd.Dispose();
                dr=null;
            }
            return retVal;
        }

        internal string inputTargetEntries( DataTable dataTable , SuperInputTargetModel superInputTargetModel , string ConnString , string Status) {
            
            string retVal       = null;          
            string connString   = getConnectionString(ConnString);
            
            SqlConnection conn  = new SqlConnection(connString);
            SqlCommand cmnd     = new SqlCommand();
            
            cmnd.Connection     = conn;
            cmnd.CommandType    = CommandType.StoredProcedure;
            cmnd.CommandText    = "zsp_insert_target_entries";

            SqlParameter parameter  = cmnd.CreateParameter();
            parameter.ParameterName = "@tvpTargetEntries";
            parameter.Value         = dataTable;
            parameter.SqlDbType     = SqlDbType.Structured;
            parameter.TypeName      = "dbo.TargetEntriesType";
            cmnd.Parameters.Add(parameter);
            //if (superInputTargetModel.RequestStage != "Initiate Target")
            //{
            //    cmnd.Parameters.Add("@deptname", SqlDbType.VarChar).Value = superInputTargetModel.EntryModel.DeptName;
            //    cmnd.Parameters.Add("@deptcode", SqlDbType.VarChar).Value = superInputTargetModel.EntryModel.DeptCode;
            //}
            //else
            //{
                
            //        if (superInputTargetModel.StaffADProfile.branch_code != "001")
            //        {
            //            //if()
            //            cmnd.Parameters.Add("@deptname", SqlDbType.VarChar).Value = superInputTargetModel.StaffADProfile.branch_name;
            //            cmnd.Parameters.Add("@deptcode", SqlDbType.VarChar).Value = superInputTargetModel.StaffADProfile.branch_code;
            //        }
            //        else
            //        {
            //            cmnd.Parameters.Add("@deptname", SqlDbType.VarChar).Value = superInputTargetModel.StaffADProfile.hodeptname;
            //            cmnd.Parameters.Add("@deptcode", SqlDbType.VarChar).Value = superInputTargetModel.StaffADProfile.hodeptname;
            //        }
              
                   
              
            //}

            cmnd.Parameters.Add("@deptname", SqlDbType.VarChar).Value = superInputTargetModel.StaffADProfile.branch_name;
            cmnd.Parameters.Add("@deptcode", SqlDbType.VarChar).Value = superInputTargetModel.StaffADProfile.branch_code;
            cmnd.Parameters.Add("@target_status"    , SqlDbType.VarChar).Value  = Status;
            cmnd.Parameters.Add("@hr_uploader_name" , SqlDbType.VarChar).Value  = superInputTargetModel.StaffADProfile.user_logon_name;
            cmnd.Parameters.Add("@hr_uploader_id"   , SqlDbType.VarChar).Value  = superInputTargetModel.StaffADProfile.employee_number;
            cmnd.Parameters.Add("@appperiod"        , SqlDbType.VarChar).Value  = superInputTargetModel.StaffADProfile.appperiod;
            cmnd.Parameters.Add("@appid"            , SqlDbType.VarChar).Value  = DataHandlers.APP_ID;
            
            cmnd.Parameters.Add("@rErrorCode"       , SqlDbType.Int,2).Direction=ParameterDirection.Output;
            cmnd.Parameters.Add("@rErrorMsg"        , SqlDbType.VarChar,255).Direction=ParameterDirection.Output;            

            SqlDataReader dr;
               
            try {
                // Open the data connection
                cmnd.Connection = conn;
                conn.Open();

                dr = cmnd.ExecuteReader(); 

                int retCode = int.Parse(cmnd.Parameters["@rErrorCode"].Value.ToString());
                if ( retCode!=0 ) {
                    retVal = retCode+"|"+cmnd.Parameters["@rErrorMsg"].Value.ToString();
                }
            
            } catch (SqlException ex) {
                if ( ex.Number!=0 ) {
                    retVal = ex.Number+"|"+ex.Message;
                    logWriter.WriteErrorLog(string.Format( "inpuTargetEntries : Exception!!! / {0}",retVal));
                }
            } finally {
                conn.Close();
                cmnd.Dispose();
                dr=null;
            }
            return retVal;
        }

        internal string inputTargetEntriesHRUpload( string workflowid , StaffADProfile staffADProfile , string ConnString , string Status) {
            
            string retVal       = null;          
            string connString   = getConnectionString(ConnString);
            
            SqlConnection conn  = new SqlConnection(connString);
            SqlCommand cmnd     = new SqlCommand();
            
            cmnd.Connection     = conn;
            cmnd.CommandType    = CommandType.StoredProcedure;
            cmnd.CommandText    = "zsp_insert_target_entries_hrupload";

            cmnd.Parameters.Add("@workflowids"      , SqlDbType.VarChar).Value  = workflowid;
            cmnd.Parameters.Add("@deptname"         , SqlDbType.VarChar).Value  = staffADProfile.branch_name;
            cmnd.Parameters.Add("@deptcode"         , SqlDbType.VarChar).Value  = staffADProfile.branch_code;
            cmnd.Parameters.Add("@target_status"    , SqlDbType.VarChar).Value  = Status;
            cmnd.Parameters.Add("@hr_uploader_name" , SqlDbType.VarChar).Value  = staffADProfile.user_logon_name;
            cmnd.Parameters.Add("@hr_uploader_id"   , SqlDbType.VarChar).Value  = staffADProfile.employee_number;
            cmnd.Parameters.Add("@appperiod"        , SqlDbType.VarChar).Value  = staffADProfile.appperiod;
            cmnd.Parameters.Add("@appid"            , SqlDbType.VarChar).Value  = DataHandlers.APP_ID;
            
            cmnd.Parameters.Add("@rErrorCode"       , SqlDbType.Int,2).Direction=ParameterDirection.Output;
            cmnd.Parameters.Add("@rErrorMsg"        , SqlDbType.VarChar,255).Direction=ParameterDirection.Output;            

            SqlDataReader dr;
               
            try {
                // Open the data connection
                cmnd.Connection = conn;
                conn.Open();

                dr = cmnd.ExecuteReader(); 

                int retCode = int.Parse(cmnd.Parameters["@rErrorCode"].Value.ToString());
                if ( retCode!=0 ) {
                    retVal = retCode+"|"+cmnd.Parameters["@rErrorMsg"].Value.ToString();
                }
            
            } catch (SqlException ex) {
                if ( ex.Number!=0 ) {
                    retVal = ex.Number+"|"+ex.Message;
                    logWriter.WriteErrorLog(string.Format( "inpuTargetEntries : Exception!!! / {0}",retVal));
                }
            } finally {
                conn.Close();
                cmnd.Dispose();
                dr=null;
            }
            return retVal;
        }

        internal string routeTargetEntries(RerouteModel rerouteModel, StaffADProfile staffADProfile , string ConnString ) {
            string retVal       = null;          
            string connString   = getConnectionString(ConnString);
            
            SqlConnection conn  = new SqlConnection(connString);
            SqlCommand cmnd     = new SqlCommand();
            
            cmnd.Connection     = conn;
            cmnd.CommandType    = CommandType.StoredProcedure;
            cmnd.CommandText    = "zsp_reroute_target_entries";

            cmnd.Parameters.Add("@workflowid"       , SqlDbType.VarChar).Value  = rerouteModel.WorkflowID;
            cmnd.Parameters.Add("@newrequeststageid", SqlDbType.Int).Value      = Int32.Parse(rerouteModel.NewRequestStageCode);
            cmnd.Parameters.Add("@comments"         , SqlDbType.VarChar).Value  = rerouteModel.Comments;
            cmnd.Parameters.Add("@target_status"    , SqlDbType.VarChar).Value  = "Rerouted";
            cmnd.Parameters.Add("@hr_uploader_name" , SqlDbType.VarChar).Value  = staffADProfile.user_logon_name;
            cmnd.Parameters.Add("@hr_uploader_id"   , SqlDbType.VarChar).Value  = staffADProfile.employee_number;
            cmnd.Parameters.Add("@appperiod"        , SqlDbType.VarChar).Value  = staffADProfile.appperiod;
            cmnd.Parameters.Add("@appid"            , SqlDbType.VarChar).Value  = DataHandlers.APP_ID;
            
            cmnd.Parameters.Add("@rErrorCode"       , SqlDbType.Int,2).Direction=ParameterDirection.Output;
            cmnd.Parameters.Add("@rErrorMsg"        , SqlDbType.VarChar,255).Direction=ParameterDirection.Output;            

            SqlDataReader dr;
               
            try {
                // Open the data connection
                cmnd.Connection = conn;
                conn.Open();

                dr = cmnd.ExecuteReader(); 

                int retCode = int.Parse(cmnd.Parameters["@rErrorCode"].Value.ToString());
                if ( retCode!=0 ) {
                    retVal = retCode+"|"+cmnd.Parameters["@rErrorMsg"].Value.ToString();
                }
            
            } catch (SqlException ex) {
                if ( ex.Number!=0 ) {
                    retVal = ex.Number+"|"+ex.Message;
                    logWriter.WriteErrorLog(string.Format( "routeTargetEntries : Exception!!! / {0}",retVal));
                }
            } finally {
                conn.Close();
                cmnd.Dispose();
                dr=null;
            }
            return retVal;
        }
        internal string insertApproverSetup(AppraisalApproverModel appr, HRProfile hrprofile, int inputMode, string ConnString ) {
            string retVal       = "";            
            string connString   = getConnectionString( ConnString );
            
            SqlConnection conn  = new SqlConnection(connString);
            SqlCommand cmnd     = new SqlCommand();
            
            cmnd.Connection     = conn;
            cmnd.CommandType    = CommandType.StoredProcedure;
            cmnd.CommandText    = "zsp_insert_approver_setup";

            cmnd.Parameters.Add("@entrykey"     , SqlDbType.VarChar).Value  = appr.EntryKey;
            cmnd.Parameters.Add("@unitcode"     , SqlDbType.VarChar).Value  = appr.UnitCode;
            cmnd.Parameters.Add("@unitname"     , SqlDbType.VarChar).Value  = appr.UnitTitle;
            cmnd.Parameters.Add("@deptcode"     , SqlDbType.VarChar).Value  = appr.DeptCode;
            cmnd.Parameters.Add("@deptname"     , SqlDbType.VarChar).Value  = appr.DeptTitle;
            cmnd.Parameters.Add("@roleid"       , SqlDbType.VarChar).Value  = appr.RoleID;
            cmnd.Parameters.Add("@role"         , SqlDbType.VarChar).Value  = appr.RoleTitle;
            cmnd.Parameters.Add("@approverid"   , SqlDbType.VarChar).Value  = appr.StaffNumber;
            cmnd.Parameters.Add("@approvername" , SqlDbType.VarChar).Value  = appr.StaffName;
            cmnd.Parameters.Add("@createdbyid"  , SqlDbType.VarChar).Value  = hrprofile.employee_number;
            cmnd.Parameters.Add("@edittedbyid"  , SqlDbType.VarChar).Value  = hrprofile.name;
            cmnd.Parameters.Add("@comments"     , SqlDbType.VarChar).Value  = "";
            
            cmnd.Parameters.Add("@rErrorCode"   , SqlDbType.Int,2).Direction=ParameterDirection.Output;
            cmnd.Parameters.Add("@rErrorMsg"    , SqlDbType.VarChar,255).Direction=ParameterDirection.Output;
            

            SqlDataReader dr;
               
            try {
                // Open the data connection
                cmnd.Connection = conn;
                conn.Open();

                dr = cmnd.ExecuteReader(); 

                int retCode = int.Parse(cmnd.Parameters["@rErrorCode"].Value.ToString());
                if ( retCode!=0 ) {
                    retVal = retCode+"|"+cmnd.Parameters["@rErrorMsg"].Value.ToString();
                }
            
            } catch (SqlException ex) {
                if ( ex.Number!=0 ) {
                    retVal = ex.Number+"|"+ex.Message;
                    logWriter.WriteErrorLog(string.Format( "insertApproverSetup : Exception!!! / {0}",retVal));
                }
            }
            return retVal;
        }
        internal string deleteApproverSetup(string entrykey , string ConnString ) {
            
            string retVal       = "";            
            string connString   = getConnectionString( ConnString );
            
            SqlConnection conn  = new SqlConnection(connString);
            SqlCommand cmnd     = new SqlCommand();
            
            cmnd.Connection     = conn;
            cmnd.CommandType    = CommandType.StoredProcedure;
            cmnd.CommandText    = "zsp_delete_approver_setup";

            cmnd.Parameters.Add("@entrykey"   , SqlDbType.VarChar).Value  = entrykey;                        
            cmnd.Parameters.Add("@rErrorCode" , SqlDbType.Int,2).Direction=ParameterDirection.Output;
            cmnd.Parameters.Add("@rErrorMsg"  , SqlDbType.VarChar,255).Direction=ParameterDirection.Output;            

            SqlDataReader dr;
               
            try {
                // Open the data connection
                cmnd.Connection = conn;
                conn.Open();

                dr = cmnd.ExecuteReader(); 

                int retCode = int.Parse(cmnd.Parameters["@rErrorCode"].Value.ToString());
                if ( retCode!=0 ) {
                    retVal = retCode+"|"+cmnd.Parameters["@rErrorMsg"].Value.ToString();
                }
            
            } catch (SqlException ex) {

                if ( ex.Number!=0 ) {
                    retVal = ex.Number+"|"+ex.Message;
                    logWriter.WriteErrorLog(string.Format( "deleteApproverSetup : Exception!!! / {0}",retVal));
                }

            }

            return retVal;
        }

        internal string insertRoleSetup(AppraisalApproverModel appr, HRProfile hrprofile, string ConnString ) {
            
            string retVal       = "";            
            string connString   = getConnectionString( ConnString );
            
            SqlConnection conn  = new SqlConnection(connString);
            SqlCommand cmnd     = new SqlCommand();
            
            cmnd.Connection     = conn;
            cmnd.CommandType    = CommandType.StoredProcedure;
            cmnd.CommandText    = "zsp_insert_appraisal_user_roles";

            cmnd.Parameters.Add("@entrykey"         , SqlDbType.VarChar).Value  = appr.EntryKey;
            cmnd.Parameters.Add("@roleid"           , SqlDbType.Int).Value      = appr.RoleID;
            cmnd.Parameters.Add("@role"             , SqlDbType.VarChar).Value  = appr.RoleTitle;
            cmnd.Parameters.Add("@username"         , SqlDbType.VarChar).Value  = appr.UserName;
            cmnd.Parameters.Add("@employee_number"  , SqlDbType.VarChar).Value  = appr.StaffNumber;
            cmnd.Parameters.Add("@name"             , SqlDbType.VarChar).Value  = appr.StaffName;
            cmnd.Parameters.Add("@status"           , SqlDbType.VarChar).Value  = appr.StatusCode;
            cmnd.Parameters.Add("@createdbyid"      , SqlDbType.VarChar).Value  = hrprofile.employee_number;
            
            cmnd.Parameters.Add("@rErrorCode"       , SqlDbType.Int,2).Direction=ParameterDirection.Output;
            cmnd.Parameters.Add("@rErrorMsg"        , SqlDbType.VarChar,255).Direction=ParameterDirection.Output;            

            SqlDataReader dr;
               
            try {
                cmnd.Connection = conn;
                conn.Open();

                dr = cmnd.ExecuteReader(); 

                int retCode = int.Parse(cmnd.Parameters["@rErrorCode"].Value.ToString());
                if ( retCode!=0 ) {
                    retVal = retCode+"|"+cmnd.Parameters["@rErrorMsg"].Value.ToString();
                }
            
            } catch (SqlException ex) {
                if ( ex.Number!=0 ) {
                    retVal = ex.Number+"|"+ex.Message;
                    logWriter.WriteErrorLog(string.Format( "insertRoleSetup : Exception!!! / {0}",retVal));
                }
            }
            return retVal;
        }

        internal string deleteRoleSetup(string entrykey, string ConnString ) {
            string retVal       = "";            
            string connString   = getConnectionString( ConnString );
            
            SqlConnection conn  = new SqlConnection(connString);
            SqlCommand cmnd     = new SqlCommand();
            
            cmnd.Connection     = conn;
            cmnd.CommandType    = CommandType.StoredProcedure;
            cmnd.CommandText    = "zsp_delete_role_setup";

            cmnd.Parameters.Add("@entrykey"   , SqlDbType.VarChar).Value  = entrykey;                        
            cmnd.Parameters.Add("@rErrorCode" , SqlDbType.Int,2).Direction=ParameterDirection.Output;
            cmnd.Parameters.Add("@rErrorMsg"  , SqlDbType.VarChar,255).Direction=ParameterDirection.Output;            

            SqlDataReader dr;
               
            try {
                // Open the data connection
                cmnd.Connection = conn;
                conn.Open();

                dr = cmnd.ExecuteReader(); 

                int retCode = int.Parse(cmnd.Parameters["@rErrorCode"].Value.ToString());
                if ( retCode!=0 ) {
                    retVal = retCode+"|"+cmnd.Parameters["@rErrorMsg"].Value.ToString();
                }
            
            } catch (SqlException ex) {
                if ( ex.Number!=0 ) {
                    retVal = ex.Number+"|"+ex.Message;
                    logWriter.WriteErrorLog(string.Format( "deleteRoleSetup : Exception!!! / {0}",retVal));
                }
            }

            return retVal;
        }
    }
}
