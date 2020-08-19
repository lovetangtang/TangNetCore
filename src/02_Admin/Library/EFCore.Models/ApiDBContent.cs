using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;

namespace EFCore.Models.Models
{
    public partial class ApiDBContent : DbContext
    {
        private DBConnectionOption _readAndWrite = null;

        public ApiDBContent(IOptionsMonitor<DBConnectionOption> options)
        {
            this._readAndWrite = options.CurrentValue;
        }

        private static int _iSeed = 0;//应该long
        public ApiDBContent ToRead()
        {
            //int num = new Random(_iSeed++).Next(0, this._readAndWrite.ReadConnectionList.Count);
            this.Database.GetDbConnection().ConnectionString = this._readAndWrite.ReadConnectionList[_iSeed++ % this._readAndWrite.ReadConnectionList.Count];//轮询
            //其实可以加入负载均衡策略---
            return this;
        }
        public ApiDBContent ToWrite()
        {
            this.Database.GetDbConnection().ConnectionString = this._readAndWrite.WriteConnection;
            return this;
        }
        public virtual DbSet<BankInfoLog> BankInfoLog { get; set; }
        public virtual DbSet<CfHsPayFlowInfo> CfHsPayFlowInfo { get; set; }
        public virtual DbSet<CmNumberCtl> CmNumberCtl { get; set; }
        public virtual DbSet<CmNumberDetail> CmNumberDetail { get; set; }
        public virtual DbSet<CmNumberInfo> CmNumberInfo { get; set; }
        public virtual DbSet<CmNumberRule> CmNumberRule { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<Published> Published { get; set; }
        public virtual DbSet<Received> Received { get; set; }
        public virtual DbSet<WxCustomMsg> WxCustomMsg { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this._readAndWrite.WriteConnection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankInfoLog>(entity =>
            {
                entity.HasComment("银行账号日志添加");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Cdate)
                    .HasColumnName("CDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.EmpId).HasColumnName("EmpID");
            });

            modelBuilder.Entity<CfHsPayFlowInfo>(entity =>
            {
                entity.ToTable("cfHsPayFlowInfo");

                entity.HasComment("恒生付款审批表");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BankAccount)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasComment("开户行及账号");

                entity.Property(e => e.BeforePaidMoney)
                    .HasColumnType("decimal(8, 2)")
                    .HasDefaultValueSql("((0))")
                    .HasComment("期前累计已付金额");

                entity.Property(e => e.BeforePayMoney)
                    .HasColumnType("decimal(8, 2)")
                    .HasDefaultValueSql("((0))")
                    .HasComment("期前累计应付金额");

                entity.Property(e => e.CapMoney)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasComment("大写金额");

                entity.Property(e => e.Cdate)
                    .HasColumnName("CDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("创建时间");

                entity.Property(e => e.CempId)
                    .HasColumnName("CEmpID")
                    .HasComment("创建人");

                entity.Property(e => e.CempName)
                    .HasColumnName("CEmpName")
                    .HasMaxLength(50)
                    .HasComment("创建人姓名");

                entity.Property(e => e.CmDynamicMoney)
                    .HasColumnType("decimal(8, 2)")
                    .HasDefaultValueSql("((0))")
                    .HasComment("合同动态金额含补充协议");

                entity.Property(e => e.CmMeetScale)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("累计应占合同比");

                entity.Property(e => e.CmMoney)
                    .HasColumnType("decimal(8, 2)")
                    .HasDefaultValueSql("((0))")
                    .HasComment("合同金额");

                entity.Property(e => e.CmName)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasComment("合同名称");

                entity.Property(e => e.CmPayMoney)
                    .HasColumnType("decimal(8, 2)")
                    .HasDefaultValueSql("((0))")
                    .HasComment("累计应付金额");

                entity.Property(e => e.CmTotalScale)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("累计已付占合同比");

                entity.Property(e => e.CmType)
                    .HasMaxLength(100)
                    .HasComment("合同类别");

                entity.Property(e => e.CmUnpaidMoney)
                    .HasColumnType("decimal(8, 2)")
                    .HasDefaultValueSql("((0))")
                    .HasComment("累计应付未付金额");

                entity.Property(e => e.MoneyName)
                    .HasMaxLength(100)
                    .HasComment("款项名称");

                entity.Property(e => e.NowPayMoney)
                    .HasColumnType("decimal(8, 2)")
                    .HasDefaultValueSql("((0))")
                    .HasComment("本期应付金额");

                entity.Property(e => e.NowRealPayMoney)
                    .HasColumnType("decimal(8, 2)")
                    .HasDefaultValueSql("((0))")
                    .HasComment("本期实际拟付");

                entity.Property(e => e.PayMode)
                    .HasMaxLength(50)
                    .HasComment("支付方式");

                entity.Property(e => e.Payment)
                    .HasMaxLength(500)
                    .HasComment("本次付款说明");

                entity.Property(e => e.PlanDate)
                    .HasColumnType("datetime")
                    .HasComment("计划付款日期");

                entity.Property(e => e.ProjectName)
                    .HasMaxLength(150)
                    .HasComment("项目名称");

                entity.Property(e => e.SignCompany)
                    .HasMaxLength(150)
                    .HasComment("签约单位");
            });

            modelBuilder.Entity<CmNumberCtl>(entity =>
            {
                entity.ToTable("cmNumberCtl");

                entity.HasComment("合同编号权限");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BranchId).HasColumnName("BranchID");

                entity.Property(e => e.DepId).HasColumnName("DepID");

                entity.Property(e => e.EmpId).HasColumnName("EmpID");

                entity.Property(e => e.NumberId).HasColumnName("NumberID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");
            });

            modelBuilder.Entity<CmNumberDetail>(entity =>
            {
                entity.ToTable("cmNumberDetail");

                entity.HasComment(@"合同规则明细
   流水号累计周期 -1：不循环；0：按日；1：按月；按年");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Custom)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DateFormat)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TimeFormat)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CmNumberInfo>(entity =>
            {
                entity.ToTable("cmNumberInfo");

                entity.HasComment("合同编号规则明细");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.NumberId).HasColumnName("NumberID");

                entity.Property(e => e.RuleId).HasColumnName("RuleID");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");
            });

            modelBuilder.Entity<CmNumberRule>(entity =>
            {
                entity.ToTable("cmNumberRule");

                entity.HasComment("合同编号规则");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Cdate)
                    .HasColumnName("CDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Published>(entity =>
            {
                entity.ToTable("Published", "cap");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Version)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Received>(entity =>
            {
                entity.ToTable("Received", "cap");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Group).HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Version)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<WxCustomMsg>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("wxCustomMsg");

                entity.Property(e => e.Cdate)
                    .HasColumnName("CDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Date).HasColumnType("smalldatetime");

                entity.Property(e => e.DocId).HasColumnName("DocID");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Link)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.LoginId)
                    .HasColumnName("LoginID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ModCode).HasMaxLength(10);

                entity.Property(e => e.Note).HasMaxLength(4000);

                entity.Property(e => e.PicUrl)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Title).HasMaxLength(200);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
