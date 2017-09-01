using GeneralMemo.App_Code;
//using RBACModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeneralMemo.Models {
    public class RBACUser {
        public int User_Id { get; set; }
        public string employee_number { get;set;}
        public bool IsSysAdmin { get; set; }
        public string Username { get; set; }
        private List<UserRole> Roles = new List<UserRole>();

        public RBACUser( string _username ) {
            this.Username = _username;
            this.IsSysAdmin = false;
            //GetDatabaseUserRolesPermissions();
            GetUserStaffNumber();
            GetCurrentUserRolesPermissions();
        }

        private void GetUserStaffNumber(){
            //now resolve the user profile from AD and Xceed
            StaffADProfile staffADProfile = new StaffADProfile();
            staffADProfile.user_logon_name = this.Username;

            //AD
            ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery( staffADProfile );
            staffADProfile = activeDirectoryQuery.GetStaffProfile();
            this.employee_number = staffADProfile.employee_number;
        }
        private void GetDatabaseUserRolesPermissions() {
            using ( RBAC_Model _data = new RBAC_Model() ) {   
                USER _user = _data.USERS.Where(u => u.Username == this.Username).FirstOrDefault();
                if (_user != null) {
                    this.User_Id = _user.User_Id;
                    foreach (ROLE _role in _user.ROLES) {
                        UserRole _userRole = new UserRole { Role_Id = _role.Role_Id, RoleName = _role.RoleName };
                        foreach (PERMISSION _permission in _role.PERMISSIONS) {
                            _userRole.Permissions.Add(new RolePermission { Permission_Id = _permission.Permission_Id, PermissionDescription = _permission.PermissionDescription });
                        }
                        this.Roles.Add(_userRole);

                        if (!this.IsSysAdmin) { 
                            this.IsSysAdmin = _role.IsSysAdmin;
                        }
                    }
                }
            }
        }

        private void GetCurrentUserRolesPermissions() {
            using ( AppraisalConnectionDataContext _apprsaisalctxt = new AppraisalConnectionDataContext() ) {
                List<AppraisalApproverModel> _user =    (   from a in _apprsaisalctxt.zib_appraisal_user_roles
                                                            join t in _apprsaisalctxt.zib_appraisal_link_role_permissions on a.roleid equals t.role_id
                                                            join x in _apprsaisalctxt.zib_appraisal_user_permissions on t.permission_id equals x.permission_id
                                                            where a.employee_number == this.employee_number
                                                            select new AppraisalApproverModel{
                                                            StaffName               = a.name,
                                                            StaffNumber             = a.employee_number,
                                                            RoleTitle               = a.role,
                                                            RoleID                  = a.roleid,
                                                            Permission_ID           = x.permission_id,
                                                            PermissionDescription   = x.permission_description
                                                       }).ToList();
                if ( _user != null ) {
                    //this.employee_number = _user.First().StaffNumber;
                    foreach (AppraisalApproverModel user in _user) {
                        UserRole _userRole = new UserRole { Role_Id = user.RoleID, RoleName = user.RoleTitle };
                        _userRole.Permissions.Add(new RolePermission { Permission_Id = user.Permission_ID, PermissionDescription = user.PermissionDescription });
                        this.Roles.Add(_userRole);
                    }
                }
            }
        }

        public bool HasPermission(string requiredPermission) {
            bool bFound = false;
            foreach (UserRole role in this.Roles) {
                bFound = (role.Permissions.Where(p => p.PermissionDescription.ToLower() == requiredPermission.ToLower()).ToList().Count > 0);
                if (bFound)
                    break;
            }
            return bFound;
        }

        public bool HasRole(string role) {
            return (Roles.Where(p => p.RoleName == role).ToList().Count > 0);
        }

        public bool HasRoles(string roles) {
            bool bFound = false;
            string[] _roles = roles.ToLower().Split(';');
            foreach (UserRole role in this.Roles) {
                try {
                    bFound = _roles.Contains(role.RoleName.ToLower());
                    if (bFound)
                        return bFound;
                }
                catch (Exception) {
                }
            }
            return bFound;
        }
    }

    public class UserRole {
        public int Role_Id { get; set; }
        public string RoleName { get; set; }
        public List<RolePermission> Permissions = new List<RolePermission>();
    }

    public class RolePermission{
        public int Permission_Id { get; set; }
        public string PermissionDescription { get; set; }
    }
}