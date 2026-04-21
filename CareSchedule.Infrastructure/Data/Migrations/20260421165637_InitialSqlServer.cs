using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareSchedule.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CapacityRule",
                columns: table => new
                {
                    RuleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scope = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ScopeID = table.Column<int>(type: "int", nullable: true),
                    MaxApptsPerDay = table.Column<int>(type: "int", nullable: true),
                    MaxConcurrentRooms = table.Column<int>(type: "int", nullable: true),
                    BufferMin = table.Column<int>(type: "int", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Capacity__110458C257D8B4CC", x => x.RuleID);
                });

            migrationBuilder.CreateTable(
                name: "OpsReport",
                columns: table => new
                {
                    ReportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scope = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MetricsJSON = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneratedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OpsRepor__D5BD48E5832B3C85", x => x.ReportID);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    ServiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    VisitType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DefaultDurationMin = table.Column<int>(type: "int", nullable: false, defaultValue: 30),
                    BufferBeforeMin = table.Column<int>(type: "int", nullable: false),
                    BufferAfterMin = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Service__C51BB0EA83FCDB6E", x => x.ServiceID);
                });

            migrationBuilder.CreateTable(
                name: "Site",
                columns: table => new
                {
                    SiteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AddressJSON = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timezone = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false, defaultValue: "UTC"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Site__B9DCB9037BADE0A3", x => x.SiteID);
                });

            migrationBuilder.CreateTable(
                name: "SLA",
                columns: table => new
                {
                    SLAID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scope = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Metric = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TargetValue = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Minutes"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SLA__2848A229D3AB2322", x => x.SLAID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active"),
                    ProviderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__1788CCACA628FF66", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Blackout",
                columns: table => new
                {
                    BlackoutID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Blackout__B868459A22B30DDD", x => x.BlackoutID);
                    table.ForeignKey(
                        name: "FK__Blackout__SiteID__41EDCAC5",
                        column: x => x.SiteID,
                        principalTable: "Site",
                        principalColumn: "SiteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Holiday",
                columns: table => new
                {
                    HolidayID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Holiday__2D35D59AE05337BA", x => x.HolidayID);
                    table.ForeignKey(
                        name: "FK__Holiday__SiteID__4589517F",
                        column: x => x.SiteID,
                        principalTable: "Site",
                        principalColumn: "SiteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceHold",
                columns: table => new
                {
                    HoldID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResourceType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ResourceID = table.Column<int>(type: "int", nullable: false),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Held")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Resource__6E24DA24CC77EF60", x => x.HoldID);
                    table.ForeignKey(
                        name: "FK__ResourceH__SiteI__756D6ECB",
                        column: x => x.SiteID,
                        principalTable: "Site",
                        principalColumn: "SiteID");
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    RoomID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    RoomName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoomType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AttributesJSON = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Room__328639196693EC23", x => x.RoomID);
                    table.ForeignKey(
                        name: "FK__Room__SiteID__1CBC4616",
                        column: x => x.SiteID,
                        principalTable: "Site",
                        principalColumn: "SiteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftTemplate",
                columns: table => new
                {
                    ShiftTemplateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    BreakMinutes = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ShiftTem__FA9F704FCAB8D6EC", x => x.ShiftTemplateID);
                    table.ForeignKey(
                        name: "FK__ShiftTemp__SiteI__02C769E9",
                        column: x => x.SiteID,
                        principalTable: "Site",
                        principalColumn: "SiteID");
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    AuditID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Resource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AuditLog__A17F23B8C0A06457", x => x.AuditID);
                    table.ForeignKey(
                        name: "FK__AuditLog__UserID__14270015",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequest",
                columns: table => new
                {
                    LeaveID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    LeaveType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LeaveReq__796DB9793791323E", x => x.LeaveID);
                    table.ForeignKey(
                        name: "FK__LeaveRequ__UserI__1A9EF37A",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Unread"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__20CF2E320934C661", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK__Notificat__UserI__29E1370A",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "OnCallCoverage",
                columns: table => new
                {
                    OnCallID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    PrimaryUserID = table.Column<int>(type: "int", nullable: false),
                    BackupUserID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OnCallCo__B7A67633E4910699", x => x.OnCallID);
                    table.ForeignKey(
                        name: "FK__OnCallCov__Backu__15DA3E5D",
                        column: x => x.BackupUserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__OnCallCov__Prima__14E61A24",
                        column: x => x.PrimaryUserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__OnCallCov__SiteI__13F1F5EB",
                        column: x => x.SiteID,
                        principalTable: "Site",
                        principalColumn: "SiteID");
                });

            migrationBuilder.CreateTable(
                name: "Provider",
                columns: table => new
                {
                    ProviderID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Credentials = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactInfo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Provider__B54C689D9E0B5E1B", x => x.ProviderID);
                    table.ForeignKey(
                        name: "FK_Provider_User_ProviderID",
                        column: x => x.ProviderID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Roster",
                columns: table => new
                {
                    RosterID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    PublishedBy = table.Column<int>(type: "int", nullable: true),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Draft")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roster__66F6BAAA409A6564", x => x.RosterID);
                    table.ForeignKey(
                        name: "FK__Roster__Publishe__0880433F",
                        column: x => x.PublishedBy,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK__Roster__SiteID__078C1F06",
                        column: x => x.SiteID,
                        principalTable: "Site",
                        principalColumn: "SiteID");
                });

            migrationBuilder.CreateTable(
                name: "SystemConfig",
                columns: table => new
                {
                    ConfigID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Global"),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SystemCo__C3BC333C2E0516F0", x => x.ConfigID);
                    table.ForeignKey(
                        name: "FK__SystemCon__Updat__41B8C09B",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LeaveImpact",
                columns: table => new
                {
                    ImpactID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaveID = table.Column<int>(type: "int", nullable: false),
                    ImpactType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ImpactJSON = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResolvedBy = table.Column<int>(type: "int", nullable: true),
                    ResolvedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Open")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LeaveImp__2297C5DD3AFBF3F9", x => x.ImpactID);
                    table.ForeignKey(
                        name: "FK__LeaveImpa__Leave__214BF109",
                        column: x => x.LeaveID,
                        principalTable: "LeaveRequest",
                        principalColumn: "LeaveID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__LeaveImpa__Resol__2334397B",
                        column: x => x.ResolvedBy,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    AppointmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    ProviderID = table.Column<int>(type: "int", nullable: false),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    ServiceID = table.Column<int>(type: "int", nullable: false),
                    RoomID = table.Column<int>(type: "int", nullable: true),
                    SlotDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    BookingChannel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Booked")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Appointm__8ECDFCA2E65367EE", x => x.AppointmentID);
                    table.ForeignKey(
                        name: "FK__Appointme__Patie__4D5F7D71",
                        column: x => x.PatientID,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__Appointme__Provi__4E53A1AA",
                        column: x => x.ProviderID,
                        principalTable: "Provider",
                        principalColumn: "ProviderID");
                    table.ForeignKey(
                        name: "FK__Appointme__RoomI__51300E55",
                        column: x => x.RoomID,
                        principalTable: "Room",
                        principalColumn: "RoomID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK__Appointme__Servi__503BEA1C",
                        column: x => x.ServiceID,
                        principalTable: "Service",
                        principalColumn: "ServiceID");
                    table.ForeignKey(
                        name: "FK__Appointme__SiteI__4F47C5E3",
                        column: x => x.SiteID,
                        principalTable: "Site",
                        principalColumn: "SiteID");
                });

            migrationBuilder.CreateTable(
                name: "AvailabilityBlock",
                columns: table => new
                {
                    BlockID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderID = table.Column<int>(type: "int", nullable: false),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Availabi__1442151191DB61C9", x => x.BlockID);
                    table.ForeignKey(
                        name: "FK__Availabil__Provi__3C34F16F",
                        column: x => x.ProviderID,
                        principalTable: "Provider",
                        principalColumn: "ProviderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Availabil__SiteI__3D2915A8",
                        column: x => x.SiteID,
                        principalTable: "Site",
                        principalColumn: "SiteID");
                });

            migrationBuilder.CreateTable(
                name: "AvailabilityTemplate",
                columns: table => new
                {
                    TemplateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderID = table.Column<int>(type: "int", nullable: false),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<byte>(type: "tinyint", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    SlotDurationMin = table.Column<int>(type: "int", nullable: false, defaultValue: 15),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Availabi__F87ADD0781DF26FF", x => x.TemplateID);
                    table.ForeignKey(
                        name: "FK__Availabil__Provi__3493CFA7",
                        column: x => x.ProviderID,
                        principalTable: "Provider",
                        principalColumn: "ProviderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Availabil__SiteI__3587F3E0",
                        column: x => x.SiteID,
                        principalTable: "Site",
                        principalColumn: "SiteID");
                });

            migrationBuilder.CreateTable(
                name: "CalendarEvent",
                columns: table => new
                {
                    EventID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EntityID = table.Column<int>(type: "int", nullable: false),
                    ProviderID = table.Column<int>(type: "int", nullable: true),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    RoomID = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Calendar__7944C87055CCF4C3", x => x.EventID);
                    table.ForeignKey(
                        name: "FK__CalendarE__Provi__7B264821",
                        column: x => x.ProviderID,
                        principalTable: "Provider",
                        principalColumn: "ProviderID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK__CalendarE__RoomI__7D0E9093",
                        column: x => x.RoomID,
                        principalTable: "Room",
                        principalColumn: "RoomID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK__CalendarE__SiteI__7C1A6C5A",
                        column: x => x.SiteID,
                        principalTable: "Site",
                        principalColumn: "SiteID");
                });

            migrationBuilder.CreateTable(
                name: "ProviderService",
                columns: table => new
                {
                    PSID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderID = table.Column<int>(type: "int", nullable: false),
                    ServiceID = table.Column<int>(type: "int", nullable: false),
                    CustomDurationMin = table.Column<int>(type: "int", nullable: true),
                    CustomBufferBeforeMin = table.Column<int>(type: "int", nullable: true),
                    CustomBufferAfterMin = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Provider__BC00097697A34DEC", x => x.PSID);
                    table.ForeignKey(
                        name: "FK__ProviderS__Provi__2EDAF651",
                        column: x => x.ProviderID,
                        principalTable: "Provider",
                        principalColumn: "ProviderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__ProviderS__Servi__2FCF1A8A",
                        column: x => x.ServiceID,
                        principalTable: "Service",
                        principalColumn: "ServiceID");
                });

            migrationBuilder.CreateTable(
                name: "PublishedSlot",
                columns: table => new
                {
                    PubSlotID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderID = table.Column<int>(type: "int", nullable: false),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    ServiceID = table.Column<int>(type: "int", nullable: false),
                    SlotDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Open")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Publishe__3CFE843623B80371", x => x.PubSlotID);
                    table.ForeignKey(
                        name: "FK__Published__Provi__46B27FE2",
                        column: x => x.ProviderID,
                        principalTable: "Provider",
                        principalColumn: "ProviderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Published__Servi__489AC854",
                        column: x => x.ServiceID,
                        principalTable: "Service",
                        principalColumn: "ServiceID");
                    table.ForeignKey(
                        name: "FK__Published__SiteI__47A6A41B",
                        column: x => x.SiteID,
                        principalTable: "Site",
                        principalColumn: "SiteID");
                });

            migrationBuilder.CreateTable(
                name: "Waitlist",
                columns: table => new
                {
                    WaitID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    ProviderID = table.Column<int>(type: "int", nullable: false),
                    ServiceID = table.Column<int>(type: "int", nullable: false),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "Normal"),
                    RequestedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Open")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Waitlist__815F96AC4369AB39", x => x.WaitID);
                    table.ForeignKey(
                        name: "FK__Waitlist__Patien__5F7E2DAC",
                        column: x => x.PatientID,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__Waitlist__Provid__5D95E53A",
                        column: x => x.ProviderID,
                        principalTable: "Provider",
                        principalColumn: "ProviderID");
                    table.ForeignKey(
                        name: "FK__Waitlist__Servic__5E8A0973",
                        column: x => x.ServiceID,
                        principalTable: "Service",
                        principalColumn: "ServiceID");
                    table.ForeignKey(
                        name: "FK__Waitlist__SiteID__5CA1C101",
                        column: x => x.SiteID,
                        principalTable: "Site",
                        principalColumn: "SiteID");
                });

            migrationBuilder.CreateTable(
                name: "RosterAssignment",
                columns: table => new
                {
                    AssignmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RosterID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    ShiftTemplateID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Assigned")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RosterAs__32499E57E16BA1A5", x => x.AssignmentID);
                    table.ForeignKey(
                        name: "FK__RosterAss__Roste__0D44F85C",
                        column: x => x.RosterID,
                        principalTable: "Roster",
                        principalColumn: "RosterID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__RosterAss__Shift__0F2D40CE",
                        column: x => x.ShiftTemplateID,
                        principalTable: "ShiftTemplate",
                        principalColumn: "ShiftTemplateID");
                    table.ForeignKey(
                        name: "FK__RosterAss__UserI__0E391C95",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "AppointmentChange",
                columns: table => new
                {
                    ChangeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentID = table.Column<int>(type: "int", nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    OldValuesJSON = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValuesJSON = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedBy = table.Column<int>(type: "int", nullable: true),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    Reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Appointm__0E05C5B7E222AEF0", x => x.ChangeID);
                    table.ForeignKey(
                        name: "FK__Appointme__Appoi__56E8E7AB",
                        column: x => x.AppointmentID,
                        principalTable: "Appointment",
                        principalColumn: "AppointmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Appointme__Chang__58D1301D",
                        column: x => x.ChangedBy,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ChargeRef",
                columns: table => new
                {
                    ChargeRefID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentID = table.Column<int>(type: "int", nullable: false),
                    ServiceID = table.Column<int>(type: "int", nullable: false),
                    ProviderID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "char(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: false, defaultValue: "INR"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Open")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChargeRe__635BEE2674BCA41A", x => x.ChargeRefID);
                    table.ForeignKey(
                        name: "FK__ChargeRef__Appoi__373B3228",
                        column: x => x.AppointmentID,
                        principalTable: "Appointment",
                        principalColumn: "AppointmentID");
                    table.ForeignKey(
                        name: "FK__ChargeRef__Provi__39237A9A",
                        column: x => x.ProviderID,
                        principalTable: "Provider",
                        principalColumn: "ProviderID");
                    table.ForeignKey(
                        name: "FK__ChargeRef__Servi__382F5661",
                        column: x => x.ServiceID,
                        principalTable: "Service",
                        principalColumn: "ServiceID");
                });

            migrationBuilder.CreateTable(
                name: "CheckIn",
                columns: table => new
                {
                    CheckInID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentID = table.Column<int>(type: "int", nullable: false),
                    TokenNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CheckInTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    RoomAssigned = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Waiting")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CheckIn__E64976A4A6077818", x => x.CheckInID);
                    table.ForeignKey(
                        name: "FK__CheckIn__Appoint__671F4F74",
                        column: x => x.AppointmentID,
                        principalTable: "Appointment",
                        principalColumn: "AppointmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__CheckIn__RoomAss__690797E6",
                        column: x => x.RoomAssigned,
                        principalTable: "Room",
                        principalColumn: "RoomID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Outcome",
                columns: table => new
                {
                    OutcomeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentID = table.Column<int>(type: "int", nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarkedBy = table.Column<int>(type: "int", nullable: true),
                    MarkedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Outcome__113E6AFCC95A983E", x => x.OutcomeID);
                    table.ForeignKey(
                        name: "FK__Outcome__Appoint__6EC0713C",
                        column: x => x.AppointmentID,
                        principalTable: "Appointment",
                        principalColumn: "AppointmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Outcome__MarkedB__70A8B9AE",
                        column: x => x.MarkedBy,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ReminderSchedule",
                columns: table => new
                {
                    RemindID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentID = table.Column<int>(type: "int", nullable: false),
                    RemindOffsetMin = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reminder__C0874AB5911F58AF", x => x.RemindID);
                    table.ForeignKey(
                        name: "FK__ReminderS__Appoi__308E3499",
                        column: x => x.AppointmentID,
                        principalTable: "Appointment",
                        principalColumn: "AppointmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_appointment_patient",
                table: "Appointment",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "idx_appointment_provider_date",
                table: "Appointment",
                columns: new[] { "ProviderID", "SlotDate" });

            migrationBuilder.CreateIndex(
                name: "idx_appointment_site_date",
                table: "Appointment",
                columns: new[] { "SiteID", "SlotDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_RoomID",
                table: "Appointment",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_ServiceID",
                table: "Appointment",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentChange_AppointmentID",
                table: "AppointmentChange",
                column: "AppointmentID");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentChange_ChangedBy",
                table: "AppointmentChange",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "idx_auditlog_user_time",
                table: "AuditLog",
                columns: new[] { "UserID", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityBlock_ProviderID",
                table: "AvailabilityBlock",
                column: "ProviderID");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityBlock_SiteID",
                table: "AvailabilityBlock",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityTemplate_ProviderID",
                table: "AvailabilityTemplate",
                column: "ProviderID");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityTemplate_SiteID",
                table: "AvailabilityTemplate",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_Blackout_SiteID",
                table: "Blackout",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvent_ProviderID",
                table: "CalendarEvent",
                column: "ProviderID");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvent_RoomID",
                table: "CalendarEvent",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvent_SiteID",
                table: "CalendarEvent",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeRef_AppointmentID",
                table: "ChargeRef",
                column: "AppointmentID");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeRef_ProviderID",
                table: "ChargeRef",
                column: "ProviderID");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeRef_ServiceID",
                table: "ChargeRef",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIn_RoomAssigned",
                table: "CheckIn",
                column: "RoomAssigned");

            migrationBuilder.CreateIndex(
                name: "UQ__CheckIn__8ECDFCA3CEF40C5B",
                table: "CheckIn",
                column: "AppointmentID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Holiday_SiteID",
                table: "Holiday",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveImpact_LeaveID",
                table: "LeaveImpact",
                column: "LeaveID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveImpact_ResolvedBy",
                table: "LeaveImpact",
                column: "ResolvedBy");

            migrationBuilder.CreateIndex(
                name: "idx_leaverequest_user",
                table: "LeaveRequest",
                columns: new[] { "UserID", "Status" });

            migrationBuilder.CreateIndex(
                name: "idx_notification_user",
                table: "Notification",
                columns: new[] { "UserID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_OnCallCoverage_BackupUserID",
                table: "OnCallCoverage",
                column: "BackupUserID");

            migrationBuilder.CreateIndex(
                name: "IX_OnCallCoverage_PrimaryUserID",
                table: "OnCallCoverage",
                column: "PrimaryUserID");

            migrationBuilder.CreateIndex(
                name: "IX_OnCallCoverage_SiteID",
                table: "OnCallCoverage",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_Outcome_MarkedBy",
                table: "Outcome",
                column: "MarkedBy");

            migrationBuilder.CreateIndex(
                name: "UQ__Outcome__8ECDFCA383D3EA6B",
                table: "Outcome",
                column: "AppointmentID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderService_ServiceID",
                table: "ProviderService",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "UQ__Provider__091DD39287307AAF",
                table: "ProviderService",
                columns: new[] { "ProviderID", "ServiceID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_publishedslot_provider_date",
                table: "PublishedSlot",
                columns: new[] { "ProviderID", "SlotDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PublishedSlot_ServiceID",
                table: "PublishedSlot",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedSlot_SiteID",
                table: "PublishedSlot",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_ReminderSchedule_AppointmentID",
                table: "ReminderSchedule",
                column: "AppointmentID");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceHold_SiteID",
                table: "ResourceHold",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_Room_SiteID",
                table: "Room",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_Roster_PublishedBy",
                table: "Roster",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Roster_SiteID",
                table: "Roster",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "idx_rosterassignment_user_date",
                table: "RosterAssignment",
                columns: new[] { "UserID", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_RosterAssignment_RosterID",
                table: "RosterAssignment",
                column: "RosterID");

            migrationBuilder.CreateIndex(
                name: "IX_RosterAssignment_ShiftTemplateID",
                table: "RosterAssignment",
                column: "ShiftTemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftTemplate_SiteID",
                table: "ShiftTemplate",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_SystemConfig_UpdatedBy",
                table: "SystemConfig",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UQ__SystemCo__C41E02899276EC84",
                table: "SystemConfig",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__User__A9D1053460A18078",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Waitlist_PatientID",
                table: "Waitlist",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_Waitlist_ProviderID",
                table: "Waitlist",
                column: "ProviderID");

            migrationBuilder.CreateIndex(
                name: "IX_Waitlist_ServiceID",
                table: "Waitlist",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_Waitlist_SiteID",
                table: "Waitlist",
                column: "SiteID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentChange");

            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "AvailabilityBlock");

            migrationBuilder.DropTable(
                name: "AvailabilityTemplate");

            migrationBuilder.DropTable(
                name: "Blackout");

            migrationBuilder.DropTable(
                name: "CalendarEvent");

            migrationBuilder.DropTable(
                name: "CapacityRule");

            migrationBuilder.DropTable(
                name: "ChargeRef");

            migrationBuilder.DropTable(
                name: "CheckIn");

            migrationBuilder.DropTable(
                name: "Holiday");

            migrationBuilder.DropTable(
                name: "LeaveImpact");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "OnCallCoverage");

            migrationBuilder.DropTable(
                name: "OpsReport");

            migrationBuilder.DropTable(
                name: "Outcome");

            migrationBuilder.DropTable(
                name: "ProviderService");

            migrationBuilder.DropTable(
                name: "PublishedSlot");

            migrationBuilder.DropTable(
                name: "ReminderSchedule");

            migrationBuilder.DropTable(
                name: "ResourceHold");

            migrationBuilder.DropTable(
                name: "RosterAssignment");

            migrationBuilder.DropTable(
                name: "SLA");

            migrationBuilder.DropTable(
                name: "SystemConfig");

            migrationBuilder.DropTable(
                name: "Waitlist");

            migrationBuilder.DropTable(
                name: "LeaveRequest");

            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "Roster");

            migrationBuilder.DropTable(
                name: "ShiftTemplate");

            migrationBuilder.DropTable(
                name: "Provider");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Site");
        }
    }
}
