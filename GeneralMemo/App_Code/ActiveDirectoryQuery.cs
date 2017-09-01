using System;
using System.Collections.Generic;

using System.Security.Permissions;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Configuration;
using GeneralMemo.Models;
using System.Diagnostics;

namespace GeneralMemo.App_Code {
    class ActiveDirectoryQuery:IDisposable {

        public DirectorySearcher dirSearch = null;
        
        private StaffADProfile staffADProfile;
        private LogWriter logWriter;
        private string StaffNumber;

        public ActiveDirectoryQuery( StaffADProfile staffADProfile ) {
            this.staffADProfile = staffADProfile;
            this.logWriter = new LogWriter();
        }

        public ActiveDirectoryQuery( string _staffNumber ) {
            this.StaffNumber=_staffNumber;
            this.logWriter = new LogWriter();
        } 

        private string GetSystemDomain() {
            try {
                return Domain.GetComputerDomain().ToString().ToLower();
            }
            catch (Exception e) {
                e.Message.ToString();
                return string.Empty;
            }
        }

        internal string GetUserName() {

            logWriter.WriteErrorLog(string.Format( "Entered GetUserName"));
            string username = null;

            try { 

                System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
                System.Configuration.KeyValueConfigurationElement sADUser       = null;
                System.Configuration.KeyValueConfigurationElement sADPassword   = null;
                System.Configuration.KeyValueConfigurationElement sDomain       = null;

			    if (rootWebConfig.AppSettings.Settings.Count > 0) {
				    sADUser       = rootWebConfig.AppSettings.Settings["sADUser"];
                    sADPassword   = rootWebConfig.AppSettings.Settings["sADPassword"];
                    sDomain       = rootWebConfig.AppSettings.Settings["sDomain"];				
                    if ( sADUser == null ){
                        username = null;
					    logWriter.WriteErrorLog(string.Format( "No ad admin profile application string"));
                    } else {
                        username = SearchUserName( sADUser.Value.ToString(), sADPassword.Value.ToString(), sDomain.Value.ToString() );
                    }     
                } else {
                    username = null;
                }
            } catch (Exception ex) { 
                username = null;
                logWriter.WriteErrorLog(string.Format( " GetStaffProfile : Exception / {0} / {1}" , staffADProfile.employee_number , ex.Message ));
            }            
            return username;
        }
        private string SearchUserName(string username, string password, string domain ) {

            SearchResult rs = null;
            string uname = null;
            try {
                
                Debug.WriteLine(username);
                Debug.WriteLine(password);
                Debug.WriteLine(domain);

                rs = SearchUserByStaffNumber( GetDirectorySearcher(username, password, domain) );
            
                if (rs != null) {

                    DirectoryEntry de   = rs.GetDirectoryEntry();

                    uname  = object.ReferenceEquals(de.Properties["sAMAccountName"].Value as string,null) ? String.Empty : de.Properties["sAMAccountName"].Value.ToString();
                    logWriter.WriteErrorLog(string.Format( "SearchUserName : User found!!! / {0}",uname ));
                } else {
                    uname = null;
                    logWriter.WriteErrorLog(string.Format( "SearchUserName : User not found!!! / {0}",uname ));                    
                }                

            } catch (Exception ex) {
                logWriter.WriteErrorLog(string.Format( "SearchUserName : Exception!!! / {0}",ex.Message ));
                uname = null;
            } finally {
                Dispose();
                rs=null;
            }
            return uname;
        }

        internal StaffADProfile GetStaffProfile() {

            logWriter.WriteErrorLog(string.Format( "Entered GetStaffProfile"));

            try { 

                System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
                System.Configuration.KeyValueConfigurationElement sADUser       = null;
                System.Configuration.KeyValueConfigurationElement sADPassword   = null;
                System.Configuration.KeyValueConfigurationElement sDomain       = null;

			    if (rootWebConfig.AppSettings.Settings.Count > 0) {
				    sADUser       = rootWebConfig.AppSettings.Settings["sADUser"];
                    sADPassword   = rootWebConfig.AppSettings.Settings["sADPassword"];
                    sDomain       = rootWebConfig.AppSettings.Settings["sDomain"];				
                    if ( sADUser == null ){
                        staffADProfile = null;
					    logWriter.WriteErrorLog(string.Format( "No ad admin profile application string"));
                    } else {
                        staffADProfile = GetStaffInformation( sADUser.Value.ToString(), sADPassword.Value.ToString(), sDomain.Value.ToString() );
                    }     
                } else {
                    staffADProfile = null;
                }
            } catch (Exception ex) { 
                staffADProfile = null;
                logWriter.WriteErrorLog(string.Format( " GetStaffProfile : Exception / {0} / {1}" , staffADProfile.employee_number , ex.Message ));
            }
            
            return staffADProfile;
        }

        private StaffADProfile GetStaffInformation(string username, string password, string domain ) {

            SearchResult rs = null;

            try {
                
                Debug.WriteLine(username);
                Debug.WriteLine(password);
                Debug.WriteLine(domain);

                rs = SearchUserByUserName( GetDirectorySearcher(username, password, domain) );
            
                if (rs != null) {

                    DirectoryEntry de   = rs.GetDirectoryEntry();

                    staffADProfile.employee_number  = object.ReferenceEquals(de.Properties["description"].Value as string ,null)    ? String.Empty : de.Properties["description"].Value.ToString();
                    staffADProfile.branch_name      = object.ReferenceEquals(de.Properties["physicalDeliveryOfficeName"].Value as string,null)   
                                                                                            ? String.Empty : de.Properties["physicalDeliveryOfficeName"].Value.ToString();
                    staffADProfile.branch_address   = object.ReferenceEquals(de.Properties["streetAddress"].Value as string,null)  ? String.Empty : de.Properties["streetAddress"].Value.ToString();
                    staffADProfile.mobile_phone     = object.ReferenceEquals(de.Properties["mobile"].Value as string,null)         ? String.Empty : de.Properties["mobile"].Value.ToString();
                    staffADProfile.gsm              = object.ReferenceEquals(de.Properties["telephoneNumber"].Value as string,null)? String.Empty : de.Properties["telephoneNumber"].Value.ToString();
                    staffADProfile.jobtitle         = object.ReferenceEquals(de.Properties["title"].Value as string,null)          ? String.Empty : de.Properties["title"].Value.ToString();
                    staffADProfile.office_ext       = object.ReferenceEquals(de.Properties["pager"].Value as string,null)         ? String.Empty : de.Properties["pager"].Value.ToString();      
                    staffADProfile.department       = object.ReferenceEquals(de.Properties["department"].Value as string,null)     ? String.Empty : de.Properties["department"].Value.ToString();
                    staffADProfile.user_logon_name  = object.ReferenceEquals(de.Properties["sAMAccountName"].Value as string,null) ? String.Empty : de.Properties["sAMAccountName"].Value.ToString();
                    staffADProfile.email            = object.ReferenceEquals(de.Properties["mail"].Value as string,null)          ? String.Empty : de.Properties["mail"].Value.ToString();
                    staffADProfile.membership       = GetMemberships( de );

                    staffADProfile.branch_code      = GetBranchCode( de );

                    logWriter.WriteErrorLog(string.Format( "GetStaffInformation : User found!!! / {0}",staffADProfile.user_logon_name ));
                } else {
                    staffADProfile = null;
                    logWriter.WriteErrorLog(string.Format( "GetStaffInformation : User not found!!! / {0}",staffADProfile.user_logon_name ));                    
                }                

            } catch (Exception ex) {
                logWriter.WriteErrorLog(string.Format( "GetStaffInformation : Exception!!! / {0}",ex.Message ));
                staffADProfile = null;
            } finally {
                Dispose();
                rs=null;
            }

            return staffADProfile;
        }

        private List<string> GetMemberships(DirectoryEntry de ) {
            
            string dn;
            int equalsIndex, commaIndex;
            int propertyCount = de.Properties["memberOf"].Count;

            List<string> memberships = new List<string>();

            for (int propertyCounter = 0 ; propertyCounter < propertyCount ; propertyCounter++ ) {
                    
                dn =  String.IsNullOrEmpty(de.Properties["memberOf"][propertyCounter].ToString()) ? String.Empty : (string) de.Properties["memberOf"][propertyCounter].ToString();
                
                equalsIndex = dn.IndexOf("=", 1);
                commaIndex = dn.IndexOf(",", 1);

                Debug.WriteLine(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));

                if ( -1 == equalsIndex || dn.Equals(String.Empty) ) {
                    return null;
                } else {
                    memberships.Add(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
                }
            }

            return memberships;
        }

        private string GetBranchCode( DirectoryEntry de ) {

            string userContainer  = String.Empty;
            DirectoryEntry deUserContainer;

            try { 
                deUserContainer  = de.Parent;
                userContainer   = object.ReferenceEquals(deUserContainer.Properties["description"].Value as string,null)  ? String.Empty : deUserContainer.Properties["description"].Value.ToString();
            }catch (Exception ex) { 
                userContainer = String.Empty;
                logWriter.WriteErrorLog(string.Format( " GetBranchCode : Exception / {0} / {1}" , staffADProfile.employee_number , ex.Message ));
            }

            return userContainer;
        }

        private SearchResult SearchUserByUserName( DirectorySearcher ds ) {

            SearchResult userObject =null;

            try { 

                ds.Filter = "(&((&(objectCategory=Person)(objectClass=User)))(samaccountname=" + staffADProfile.user_logon_name + "))";

                ds.SearchScope = SearchScope.Subtree;
                ds.ServerTimeLimit = TimeSpan.FromSeconds(90);

                userObject = ds.FindOne();

                if (userObject != null){
                    return userObject;
                } else {
                    logWriter.WriteErrorLog(string.Format( "SearchUserByUserName : None Found {0} " , staffADProfile.user_logon_name ));
                }
            } catch ( Exception ex ) {
                logWriter.WriteErrorLog(string.Format( "SearchUserByUserName : Exception!!! {0} / {1}" , ex.Message , staffADProfile.user_logon_name ));
            } finally {
                Dispose();
            }

            return userObject;
        }

        private SearchResult SearchUserByEmail(DirectorySearcher ds, string email) {

            SearchResult userObject = null;
            try { 
                ds.Filter = "(&((&(objectCategory=Person)(objectClass=User)))(mail=" + email + "))";

                ds.SearchScope = SearchScope.Subtree;
                ds.ServerTimeLimit = TimeSpan.FromSeconds(90);

                userObject = ds.FindOne();

                if (userObject != null){
                    return userObject;
                } else {
                    logWriter.WriteErrorLog(string.Format( "SearchUserByEmail : None Found {0} " , staffADProfile.user_logon_name ));
                }
            } catch (Exception ex) {
                logWriter.WriteErrorLog(string.Format( "SearchUserByEmail : Exception!!! {0} / {1}" , ex.Message , staffADProfile.employee_number ));
            } finally {
                Dispose();
            }

            return userObject;
        }

        private SearchResult SearchUserByStaffNumber( DirectorySearcher ds ) {
            
            SearchResult userObject = null;

            try { 
                ds.Filter = "(&((&(objectCategory=Person)(objectClass=User)))( description=" + StaffNumber + "))";

                ds.SearchScope = SearchScope.Subtree;
                ds.ServerTimeLimit = TimeSpan.FromSeconds(90);

                userObject = ds.FindOne();  
              
                if ( userObject != null ){
                    logWriter.WriteErrorLog(string.Format( "SearchUserByStaffNumber : One Found {0} " , StaffNumber ));
                } else { 
                    logWriter.WriteErrorLog(string.Format( "SearchUserByStaffNumber : None Found {0} " , StaffNumber ));
                }  
            } catch (Exception ex) {                
                logWriter.WriteErrorLog(string.Format( "SearchUserByStaffNumber : Exception!!! {0} / {1}" , ex.Message , StaffNumber ));
            }

            return userObject;
                      
        }

        public DirectorySearcher GetDirectorySearcher(string username, string password, string domain) {       
    
            //username = "africa\\admWorkflow";
            //password = "p@ssw0rd";
            //domain   = "africa.int.zenithbank.com";
            
            if( dirSearch == null ) {
                
                try {                    
                    dirSearch = new DirectorySearcher( new DirectoryEntry("LDAP://" + domain.Trim(), username.Trim(), password.Trim() ));                   
                } catch (DirectoryServicesCOMException ex ) {  
                    logWriter.WriteErrorLog(string.Format( "Connection Creditial is Wrong!!!, please Check." + ex.Message ));
                }
                return dirSearch;

            } else{
                return dirSearch;
            }
        }
        public void Dispose(){
            // If this function is being called the user wants to release the
            // resources. lets call the Dispose which will do this for us.
            Dispose(true);

            // Now since we have done the cleanup already there is nothing left
            // for the Finalizer to do. So lets tell the GC not to call it later.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {            
            if (disposing == true) {
                //someone want the deterministic release of all resources
                //Let us release all the managed resources
                ReleaseManagedResources();
            } else {
                // Do nothing, no one asked a dispose, the object went out of
                // scope and finalized is called so lets next round of GC 
                // release these resources
            }

            // Release the unmanaged resource in any case as they will not be 
            // released by GC
            ReleaseUnmangedResources();
        }

        ~ActiveDirectoryQuery() {
            // The object went out of scope and finalized is called
            // Lets call dispose in to release unmanaged resources 
            // the managed resources will anyways be released when GC 
            // runs the next time.
            Dispose(false);
        }

        void ReleaseManagedResources() {
            Console.WriteLine("Releasing Managed Resources");
            if ( dirSearch != null) {
                dirSearch.Dispose();
            }
        }
        void ReleaseUnmangedResources() {
            Console.WriteLine("Releasing Unmanaged Resources");
        }
    }
}
