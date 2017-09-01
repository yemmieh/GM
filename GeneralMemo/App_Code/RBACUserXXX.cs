using GeneralMemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralMemo.App_Code {
    public class RBACUserXXX {
        public string employee_number { get; set; }
        public bool IsSysAdmin { get; set; }
        public string Username { get; set; }
        private List<UserRole> Roles = new List<UserRole>();
 
        public RBACUserXXX(string _username) {
            this.Username = _username;
            this.IsSysAdmin = false;
            GetDatabaseUserRolesPermissions();
        }
 
        private void GetDatabaseUserRolesPermissions() {
            //Get user roles and permissions from database tables...      
            using (AppraisalConnectionDataContext _data = new AppraisalConnectionDataContext()) {            
                UserXXX _user = ( from d in _data.zib_appraisal_user_roles
                               select new UserXXX{ 
                                   entrykey = d.entrykey,
                                   roleid   = d.roleid.ToString(),
                                   role     = d.role,
                                   username = d.username,
                                   name     = d.name,
                                   status   = d.status,
                                   description      = d.description,
                                   employee_number  = d.employee_number}
                              ).Where(u => u.username == this.Username).FirstOrDefault();
                
                /*if (_user != null) {
                    this.employee_number = _user.employee_number;
                    foreach (Role _role in _user.roleid) {
                        UserRole _userRole 
                                = new UserRole {    Role_Id = _role.Id,
                                                    RoleName = _role.RoleName 
                                               };
                        foreach (Permission _permission in _role.Permissions) {
                            _userRole.Permissions.Add(new RolePermission { 
                                        Permission_Id = _permission.Id, 
				                        PermissionDescription = _permission.PermissionDescription });
                        }
                        this.Roles.Add(_userRole);
 
                        if (!this.IsSystemAdmin)
                            this.IsSystemAdmin = _role.IsSysAdmin;
                    }
                }*/
            }
        }
 
        public bool HasPermission(string requiredPermission) {
            bool bFound = false;
            foreach (UserRole role in this.Roles) {
                bFound = (role.Permissions.Where(
                          p => p.PermissionDescription == requiredPermission).ToList().Count > 0);
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
                } catch (Exception) {
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
 
    public class RolePermission {
        public int Permission_Id { get; set; }
        public string PermissionDescription { get; set; }
    }


}
