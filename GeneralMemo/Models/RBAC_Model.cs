namespace GeneralMemo.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class RBAC_Model : DbContext
    {
        public RBAC_Model() : base("name=RBAC_Model") { }

        public virtual DbSet<PERMISSION> PERMISSIONS { get; set; }
        public virtual DbSet<ROLE> ROLES { get; set; }
        public virtual DbSet<USER> USERS { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PERMISSION>()
                .HasMany(e => e.ROLES)
                .WithMany(e => e.PERMISSIONS)
                .Map(m => m.ToTable("LNK_ROLE_PERMISSION").MapLeftKey("Permission_Id").MapRightKey("Role_Id"));

            modelBuilder.Entity<ROLE>()
                .HasMany(e => e.USERS)
                .WithMany(e => e.ROLES)
                .Map(m => m.ToTable("LNK_USER_ROLE").MapLeftKey("Role_Id").MapRightKey("User_Id"));
        }
    }
}
