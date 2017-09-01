﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GeneralMemo
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="ZenithTgt_Demo")]
	public partial class TargetConnectionDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    #endregion
		
		public TargetConnectionDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["ZenithTgt_DemoConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public TargetConnectionDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public TargetConnectionDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public TargetConnectionDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public TargetConnectionDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<ZenithTgt_Demo> ZenithTgt_Demos
		{
			get
			{
				return this.GetTable<ZenithTgt_Demo>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.ZenithTgt_Demo")]
	public partial class ZenithTgt_Demo
	{
		
		private string _Staff_Branch_Code;
		
		private string _Staff_ID;
		
		private string _Staff_Name;
		
		private string _Staff_Branch;
		
		private string _Staff_Grade;
		
		private string _Staff_CA;
		
		private string _Staff_CA_Legacy;
		
		private string _Staff_SA;
		
		private string _Staff_SA_Legacy;
		
		private string _Staff_FX;
		
		private string _Staff_RV;
		
		private string _Staff_FDCD;
		
		private string _Staff_INCOME;
		
		private string _Staff_INCOME_Legacy;
		
		private System.DateTime _DateCreated;
		
		private string _Target_Status;
		
		private string _HR_Uploader_Name;
		
		private string _HR_Uploader_ID;
		
		private string _AppPeriod;
		
		public ZenithTgt_Demo()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_Branch_Code", DbType="VarChar(10) NOT NULL", CanBeNull=false)]
		public string Staff_Branch_Code
		{
			get
			{
				return this._Staff_Branch_Code;
			}
			set
			{
				if ((this._Staff_Branch_Code != value))
				{
					this._Staff_Branch_Code = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_ID", DbType="VarChar(20) NOT NULL", CanBeNull=false)]
		public string Staff_ID
		{
			get
			{
				return this._Staff_ID;
			}
			set
			{
				if ((this._Staff_ID != value))
				{
					this._Staff_ID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_Name", DbType="VarChar(100) NOT NULL", CanBeNull=false)]
		public string Staff_Name
		{
			get
			{
				return this._Staff_Name;
			}
			set
			{
				if ((this._Staff_Name != value))
				{
					this._Staff_Name = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_Branch", DbType="VarChar(90) NOT NULL", CanBeNull=false)]
		public string Staff_Branch
		{
			get
			{
				return this._Staff_Branch;
			}
			set
			{
				if ((this._Staff_Branch != value))
				{
					this._Staff_Branch = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_Grade", DbType="VarChar(10) NOT NULL", CanBeNull=false)]
		public string Staff_Grade
		{
			get
			{
				return this._Staff_Grade;
			}
			set
			{
				if ((this._Staff_Grade != value))
				{
					this._Staff_Grade = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_CA", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string Staff_CA
		{
			get
			{
				return this._Staff_CA;
			}
			set
			{
				if ((this._Staff_CA != value))
				{
					this._Staff_CA = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_CA_Legacy", DbType="VarChar(50)")]
		public string Staff_CA_Legacy
		{
			get
			{
				return this._Staff_CA_Legacy;
			}
			set
			{
				if ((this._Staff_CA_Legacy != value))
				{
					this._Staff_CA_Legacy = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_SA", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string Staff_SA
		{
			get
			{
				return this._Staff_SA;
			}
			set
			{
				if ((this._Staff_SA != value))
				{
					this._Staff_SA = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_SA_Legacy", DbType="VarChar(50)")]
		public string Staff_SA_Legacy
		{
			get
			{
				return this._Staff_SA_Legacy;
			}
			set
			{
				if ((this._Staff_SA_Legacy != value))
				{
					this._Staff_SA_Legacy = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_FX", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string Staff_FX
		{
			get
			{
				return this._Staff_FX;
			}
			set
			{
				if ((this._Staff_FX != value))
				{
					this._Staff_FX = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_RV", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string Staff_RV
		{
			get
			{
				return this._Staff_RV;
			}
			set
			{
				if ((this._Staff_RV != value))
				{
					this._Staff_RV = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_FDCD", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string Staff_FDCD
		{
			get
			{
				return this._Staff_FDCD;
			}
			set
			{
				if ((this._Staff_FDCD != value))
				{
					this._Staff_FDCD = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_INCOME", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string Staff_INCOME
		{
			get
			{
				return this._Staff_INCOME;
			}
			set
			{
				if ((this._Staff_INCOME != value))
				{
					this._Staff_INCOME = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_INCOME_Legacy", DbType="VarChar(50)")]
		public string Staff_INCOME_Legacy
		{
			get
			{
				return this._Staff_INCOME_Legacy;
			}
			set
			{
				if ((this._Staff_INCOME_Legacy != value))
				{
					this._Staff_INCOME_Legacy = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DateCreated", DbType="DateTime NOT NULL")]
		public System.DateTime DateCreated
		{
			get
			{
				return this._DateCreated;
			}
			set
			{
				if ((this._DateCreated != value))
				{
					this._DateCreated = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Target_Status", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string Target_Status
		{
			get
			{
				return this._Target_Status;
			}
			set
			{
				if ((this._Target_Status != value))
				{
					this._Target_Status = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_HR_Uploader_Name", DbType="VarChar(100) NOT NULL", CanBeNull=false)]
		public string HR_Uploader_Name
		{
			get
			{
				return this._HR_Uploader_Name;
			}
			set
			{
				if ((this._HR_Uploader_Name != value))
				{
					this._HR_Uploader_Name = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_HR_Uploader_ID", DbType="VarChar(20) NOT NULL", CanBeNull=false)]
		public string HR_Uploader_ID
		{
			get
			{
				return this._HR_Uploader_ID;
			}
			set
			{
				if ((this._HR_Uploader_ID != value))
				{
					this._HR_Uploader_ID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AppPeriod", DbType="VarChar(20)")]
		public string AppPeriod
		{
			get
			{
				return this._AppPeriod;
			}
			set
			{
				if ((this._AppPeriod != value))
				{
					this._AppPeriod = value;
				}
			}
		}
	}
}
#pragma warning restore 1591