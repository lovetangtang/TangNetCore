using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EFCore.Models.Migrations
{
    public partial class Migrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cap");

            migrationBuilder.CreateTable(
                name: "BankInfoLog",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpID = table.Column<int>(nullable: true),
                    Content = table.Column<string>(unicode: false, nullable: false),
                    OperateType = table.Column<int>(nullable: true),
                    CDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Moudel = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankInfoLog", x => x.ID);
                },
                comment: "银行账号日志添加");

            migrationBuilder.CreateTable(
                name: "cfHsPayFlowInfo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectName = table.Column<string>(maxLength: 150, nullable: true, comment: "项目名称"),
                    CmName = table.Column<string>(maxLength: 150, nullable: false, comment: "合同名称"),
                    SignCompany = table.Column<string>(maxLength: 150, nullable: true, comment: "签约单位"),
                    CmType = table.Column<string>(maxLength: 100, nullable: true, comment: "合同类别"),
                    CmMoney = table.Column<decimal>(type: "decimal(8, 2)", nullable: true, defaultValueSql: "((0))", comment: "合同金额"),
                    CmDynamicMoney = table.Column<decimal>(type: "decimal(8, 2)", nullable: true, defaultValueSql: "((0))", comment: "合同动态金额含补充协议"),
                    BankAccount = table.Column<string>(unicode: false, maxLength: 300, nullable: true, comment: "开户行及账号"),
                    BeforePayMoney = table.Column<decimal>(type: "decimal(8, 2)", nullable: true, defaultValueSql: "((0))", comment: "期前累计应付金额"),
                    BeforePaidMoney = table.Column<decimal>(type: "decimal(8, 2)", nullable: true, defaultValueSql: "((0))", comment: "期前累计已付金额"),
                    CmTotalScale = table.Column<string>(unicode: false, maxLength: 100, nullable: true, comment: "累计已付占合同比"),
                    CmPayMoney = table.Column<decimal>(type: "decimal(8, 2)", nullable: true, defaultValueSql: "((0))", comment: "累计应付金额"),
                    CmUnpaidMoney = table.Column<decimal>(type: "decimal(8, 2)", nullable: true, defaultValueSql: "((0))", comment: "累计应付未付金额"),
                    CmMeetScale = table.Column<string>(unicode: false, maxLength: 100, nullable: true, comment: "累计应占合同比"),
                    Payment = table.Column<string>(maxLength: 500, nullable: true, comment: "本次付款说明"),
                    NowPayMoney = table.Column<decimal>(type: "decimal(8, 2)", nullable: true, defaultValueSql: "((0))", comment: "本期应付金额"),
                    NowRealPayMoney = table.Column<decimal>(type: "decimal(8, 2)", nullable: true, defaultValueSql: "((0))", comment: "本期实际拟付"),
                    CapMoney = table.Column<string>(unicode: false, maxLength: 200, nullable: true, comment: "大写金额"),
                    PlanDate = table.Column<DateTime>(type: "datetime", nullable: true, comment: "计划付款日期"),
                    MoneyName = table.Column<string>(maxLength: 100, nullable: true, comment: "款项名称"),
                    PayMode = table.Column<string>(maxLength: 50, nullable: true, comment: "支付方式"),
                    CEmpID = table.Column<int>(nullable: true, comment: "创建人"),
                    CDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())", comment: "创建时间"),
                    CEmpName = table.Column<string>(maxLength: 50, nullable: true, comment: "创建人姓名")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cfHsPayFlowInfo", x => x.ID);
                },
                comment: "恒生付款审批表");

            migrationBuilder.CreateTable(
                name: "cmNumberCtl",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumberID = table.Column<int>(nullable: true),
                    RoleID = table.Column<int>(nullable: true),
                    DepID = table.Column<int>(nullable: true),
                    BranchID = table.Column<int>(nullable: true),
                    EmpID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cmNumberCtl", x => x.ID);
                },
                comment: "合同编号权限");

            migrationBuilder.CreateTable(
                name: "cmNumberDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateFormat = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    TimeFormat = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    SerialCycle = table.Column<int>(nullable: true),
                    SerialLength = table.Column<int>(nullable: true),
                    SerialStart = table.Column<int>(nullable: true),
                    SerialZero = table.Column<bool>(nullable: true),
                    Const = table.Column<int>(nullable: true),
                    Custom = table.Column<string>(unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cmNumberDetail", x => x.ID);
                },
                comment: @"合同规则明细
   流水号累计周期 -1：不循环；0：按日；1：按月；按年");

            migrationBuilder.CreateTable(
                name: "cmNumberInfo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleID = table.Column<int>(nullable: false),
                    NumberID = table.Column<int>(nullable: false),
                    TypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cmNumberInfo", x => x.ID);
                },
                comment: "合同编号规则明细");

            migrationBuilder.CreateTable(
                name: "cmNumberRule",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    CreateUser = table.Column<int>(nullable: true),
                    CDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdateUser = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cmNumberRule", x => x.ID);
                },
                comment: "合同编号规则");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    OrderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "wxCustomMsg",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginID = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    ModCode = table.Column<string>(maxLength: 10, nullable: true),
                    DocID = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 200, nullable: true),
                    Note = table.Column<string>(maxLength: 4000, nullable: true),
                    Link = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    Date = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    CDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    PicUrl = table.Column<string>(unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Published",
                schema: "cap",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Version = table.Column<string>(maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Content = table.Column<string>(nullable: true),
                    Retries = table.Column<int>(nullable: false),
                    Added = table.Column<DateTime>(nullable: false),
                    ExpiresAt = table.Column<DateTime>(nullable: true),
                    StatusName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Published", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Received",
                schema: "cap",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Version = table.Column<string>(maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Group = table.Column<string>(maxLength: 200, nullable: true),
                    Content = table.Column<string>(nullable: true),
                    Retries = table.Column<int>(nullable: false),
                    Added = table.Column<DateTime>(nullable: false),
                    ExpiresAt = table.Column<DateTime>(nullable: true),
                    StatusName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Received", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankInfoLog");

            migrationBuilder.DropTable(
                name: "cfHsPayFlowInfo");

            migrationBuilder.DropTable(
                name: "cmNumberCtl");

            migrationBuilder.DropTable(
                name: "cmNumberDetail");

            migrationBuilder.DropTable(
                name: "cmNumberInfo");

            migrationBuilder.DropTable(
                name: "cmNumberRule");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "wxCustomMsg");

            migrationBuilder.DropTable(
                name: "Published",
                schema: "cap");

            migrationBuilder.DropTable(
                name: "Received",
                schema: "cap");
        }
    }
}
