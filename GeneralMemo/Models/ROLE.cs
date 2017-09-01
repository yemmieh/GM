namespace GeneralMemo.Models{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ROLES")]
    public partial class ROLE
    {
        public ROLE()
        {
            PERMISSIONS = new HashSet<PERMISSION>();
            USERS = new HashSet<USER>();
        }

        [Key]
        public int Role_Id { get; set; }

        [Required]
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool IsSysAdmin { get; set; }
        public virtual ICollection<PERMISSION> PERMISSIONS { get; set; }
        public virtual ICollection<USER> USERS { get; set; }
    }
}
