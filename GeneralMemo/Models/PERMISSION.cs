namespace GeneralMemo.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PERMISSIONS")]
    public partial class PERMISSION
    {
        public PERMISSION()
        {
            ROLES = new HashSet<ROLE>();
        }

        [Key]
        public int Permission_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PermissionDescription { get; set; }

        public virtual ICollection<ROLE> ROLES { get; set; }
    }
}
