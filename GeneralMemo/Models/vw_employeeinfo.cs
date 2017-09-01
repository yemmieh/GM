namespace GeneralMemo.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_employeeinfo
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string employee_number { get; set; }

        [StringLength(205)]
        public string name { get; set; }

        [StringLength(100)]
        public string companyname { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int employee_id { get; set; }

        public byte? linemanager { get; set; }

        public byte? emp_confirm { get; set; }

        public int? grade_id { get; set; }

        public int? department_id { get; set; }

        public DateTime? employment_date { get; set; }

        public DateTime? a_confirmdate { get; set; }

        public byte? gender { get; set; }

        public byte? marital_status { get; set; }

        [StringLength(100)]
        public string title { get; set; }

        public byte? employment_type { get; set; }

        public DateTime? p_confirm_date { get; set; }

        [StringLength(100)]
        public string unit { get; set; }

        public int? status_id { get; set; }

        public DateTime? last_promo_date { get; set; }

        [StringLength(100)]
        public string maiden_name { get; set; }

        [StringLength(100)]
        public string account_no { get; set; }

        [StringLength(100)]
        public string nsitf_no { get; set; }

        [StringLength(100)]
        public string nhf_no { get; set; }

        [StringLength(8000)]
        public string state { get; set; }

        public byte? employee_status { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int org_id { get; set; }

        [StringLength(100)]
        public string dept { get; set; }

        [StringLength(100)]
        public string paygroup { get; set; }

        [StringLength(100)]
        public string lga { get; set; }

        [Column(TypeName = "money")]
        public decimal? annual_salary { get; set; }

        public int? religion_id { get; set; }

        [StringLength(100)]
        public string department_code { get; set; }

        [StringLength(100)]
        public string grade { get; set; }

        public int? ranking { get; set; }

        public DateTime? date_of_birth { get; set; }

        public int? title_id { get; set; }

        public int? acting_title_id { get; set; }

        [StringLength(100)]
        public string title_code { get; set; }

        [StringLength(100)]
        public string jobtitle { get; set; }

        [StringLength(100)]
        public string currency_name { get; set; }

        public int? age { get; set; }

        public DateTime? wedding_date { get; set; }

        public short? len_service { get; set; }

        [StringLength(100)]
        public string code { get; set; }

        public byte? on_payroll { get; set; }

        [StringLength(100)]
        public string Branch { get; set; }

        [StringLength(100)]
        public string Branch_code { get; set; }

        [StringLength(200)]
        public string random_hash { get; set; }

        public byte? deferment { get; set; }

        public int? analysis_id { get; set; }

        [StringLength(100)]
        public string grade_code { get; set; }

        public byte? step { get; set; }

        [StringLength(100)]
        public string Category { get; set; }

        [StringLength(250)]
        public string remark { get; set; }

        public int? analysis_det_id { get; set; }

        public int? paygroup_id { get; set; }

        public int? state_id { get; set; }

        [StringLength(100)]
        public string email { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(100)]
        public string employee_surname { get; set; }

        [StringLength(100)]
        public string gsm { get; set; }

        [StringLength(100)]
        public string mobile_phone { get; set; }

        [StringLength(100)]
        public string office_ext { get; set; }

        [StringLength(100)]
        public string employee_firstname { get; set; }

        [StringLength(100)]
        public string employee_midname { get; set; }

        [StringLength(100)]
        public string HEALTH_PLAN_CATEGORY { get; set; }

        public byte? nsitf_entitled { get; set; }

        public byte? nhf_entitled { get; set; }

        [StringLength(100)]
        public string street1_r { get; set; }

        [StringLength(100)]
        public string street2_r { get; set; }

        [StringLength(100)]
        public string city_r { get; set; }

        [StringLength(100)]
        public string country_r { get; set; }

        [StringLength(100)]
        public string street1_p { get; set; }

        [StringLength(100)]
        public string street2_p { get; set; }

        [StringLength(100)]
        public string city_p { get; set; }

        [StringLength(100)]
        public string logon_name { get; set; }

        public DateTime? created_date { get; set; }

        [StringLength(100)]
        public string homeplace { get; set; }

        [StringLength(10)]
        public string employee_fileno { get; set; }

        [StringLength(100)]
        public string country { get; set; }

        public int? leaveBf { get; set; }

        public double? leaveoutstanding { get; set; }

        public int? total_leave_days { get; set; }

        [StringLength(100)]
        public string state_code { get; set; }

        [StringLength(100)]
        public string state_p { get; set; }

        [StringLength(100)]
        public string state_r { get; set; }

        [StringLength(100)]
        public string license_no { get; set; }

        [StringLength(100)]
        public string license_type { get; set; }

        public DateTime? license_date_issued { get; set; }

        public DateTime? license_expires { get; set; }

        [StringLength(100)]
        public string passport_no { get; set; }

        public DateTime? passport_date_issued { get; set; }

        public DateTime? passport_expires { get; set; }

        [StringLength(100)]
        public string phone { get; set; }

        [StringLength(100)]
        public string next_of_kin_name { get; set; }
    }
}
