namespace GeneralMemo.Models {
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class BranchHeadStaffProfile : DbContext {
        public BranchHeadStaffProfile()
            : base("name=BranchHeadStaffProfile") {
        }

        public virtual DbSet<vw_employeeinfo> vw_employeeinfo { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.employee_number)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.companyname)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.title)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.unit)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.maiden_name)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.account_no)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.nsitf_no)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.nhf_no)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.state)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.dept)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.paygroup)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.lga)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.annual_salary)
                .HasPrecision(19, 4);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.department_code)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.grade)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.title_code)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.jobtitle)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.currency_name)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.Branch)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.Branch_code)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.random_hash)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.grade_code)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.Category)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.remark)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.employee_surname)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.gsm)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.mobile_phone)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.office_ext)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.employee_firstname)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.employee_midname)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.HEALTH_PLAN_CATEGORY)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.street1_r)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.street2_r)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.city_r)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.country_r)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.street1_p)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.street2_p)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.city_p)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.logon_name)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.homeplace)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.employee_fileno)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.country)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.state_code)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.state_p)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.state_r)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.license_no)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.license_type)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.passport_no)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.phone)
                .IsUnicode(false);

            modelBuilder.Entity<vw_employeeinfo>()
                .Property(e => e.next_of_kin_name)
                .IsUnicode(false);
        }
    }
}
