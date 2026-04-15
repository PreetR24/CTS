# CareSchedule — Architecture & Kickoff Guide

**Date:** March 3, 2026  
**Team Size:** 4 Developers  
**Stack:** .NET Core (Backend) + React (Frontend — later) + SQL Server  
**Phase:** MVP / Phase 1

---

## 1. Architecture Decision

We are building a **Modular Monolith** — one .NET solution where each module is a separate class library project. All modules share a **single database** and a **single DbContext**, but each module owns its own set of tables.

**Why not Microservices?** Too complex for a 4-person team. We get the same code separation benefits without the deployment headache.

---

## 2. Solution Structure

```
CareSchedule.sln
│
├── CareSchedule.API/                              → The single runnable Web API project
│   ├── Controllers/                               → All API controllers (grouped by module)
│   ├── Middleware/                                 → Error handling, auth middleware
│   ├── Program.cs                                 → DI registration, app startup
│   └── appsettings.json                           → DB connection string, app settings
│
├── CareSchedule.Shared/                           → Shared contracts used by all modules
│   ├── Interfaces/                                → IRepository<T>, IUnitOfWork
│   ├── Enums/                                     → All status enums
│   └── Exceptions/                                → Custom exception classes
│
├── CareSchedule.Infrastructure/                   → Database setup (shared)
│   ├── AppDbContext.cs                            → Single DbContext with all 34 DbSets
│   ├── GenericRepository.cs                       → Generic repo implementation
│   └── UnitOfWork.cs                              → Unit of Work implementation
│
└── Modules/
    ├── CareSchedule.Identity/                     → Auth, users, audit logs
    ├── CareSchedule.MasterData/                   → Sites, rooms, providers, services
    ├── CareSchedule.Availability/                 → Templates, blocks, blackouts, published slots
    ├── CareSchedule.Booking/                      → Appointments, waitlist, check-in, outcomes
    ├── CareSchedule.Calendar/                     → Calendar events, resource holds
    ├── CareSchedule.Roster/                       → Shift templates, rosters, on-call
    ├── CareSchedule.Leave/                        → Leave requests, impact tracking
    ├── CareSchedule.Rules/                        → Capacity rules, SLA definitions
    ├── CareSchedule.Notification/                 → Notifications, reminder schedules
    ├── CareSchedule.Reports/                      → Operational reports
    ├── CareSchedule.Billing/                      → Charge references (optional)
    └── CareSchedule.Admin/                        → System config, holidays
```

Every module has exactly **3 folders**:

| Folder | What goes here |
|---|---|
| **Models/** | EF Core entity classes — one file per DB table. These are the scaffolded classes from the database. |
| **Repositories/** | Interface + implementation. Handles all DB queries for this module's tables. No business logic here. |
| **Services/** | Interface + implementation. Business logic lives here. Controllers call services, services call repositories. |

---

## 3. Key Concepts

### Single AppDbContext
One `AppDbContext` in Infrastructure registers all 34 tables as `DbSet<>`. Each module's model classes live in their own `Models/` folder, but they're all registered in the same context. One connection string, one database.

### The Flow (for every API)
```
HTTP Request → Controller → Service → Repository → Database
```
Controllers are thin — they just call the service and return the result. All validation and business logic goes in the Service layer.

### Cross-Module Communication (MVP approach)
If a service in one module needs data from another module, just inject that module's service directly via DI. Example: `BookingService` injects `ISlotService` from the Availability module. Simple and good enough for MVP.

---

## 4. Table Ownership by Module (all 34 tables)

| Module | Tables |
|---|---|
| **Identity** | User, AuditLog |
| **MasterData** | Site, Room, Provider, Service, ProviderService |
| **Availability** | AvailabilityTemplate, AvailabilityBlock, Blackout, PublishedSlot |
| **Booking** | Appointment, AppointmentChange, Waitlist, CheckIn, Outcome |
| **Calendar** | CalendarEvent, ResourceHold |
| **Roster** | ShiftTemplate, Roster, RosterAssignment, OnCallCoverage |
| **Leave** | LeaveRequest, LeaveImpact |
| **Rules** | CapacityRule, SLA |
| **Notification** | Notification, ReminderSchedule |
| **Reports** | OpsReport |
| **Billing** | ChargeRef |
| **Admin** | SystemConfig, Holiday |

**Rule:** Only the owning module writes to its tables. Other modules can read via the owning module's service.

---

## 5. Project References (how they're wired)

```
CareSchedule.Shared           → referenced by ALL modules + Infrastructure + API
CareSchedule.Infrastructure    → references Shared + all 12 modules (for DbContext)
CareSchedule.API               → references Shared + Infrastructure + all 12 modules
Each Module                    → references only Shared
```

---

## 6. Work Division

### Dev 1 — Foundation + Identity + Admin + Rules
**Must finish first (Day 1-2).** Everyone else depends on this.

| Task | Details |
|---|---|
| Solution structure | Already created (all 15 projects, references wired) |
| Write Shared code | IRepository<T>, IUnitOfWork, StatusEnums, AppException |
| Write Infrastructure code | AppDbContext, GenericRepository, UnitOfWork |
| Write Identity module | User.cs, AuditLog.cs, repos, services |
| Write Admin module | SystemConfig, Holiday |
| Write Rules module | CapacityRule, SLA |
| Wire up Program.cs | Register DbContext, repos, services |
| Push to GitHub | Initial commit with full structure |

### Dev 2 — MasterData + Availability
**Starts prepping Day 1, codes after Dev 1 pushes.**

| Task | Details |
|---|---|
| Day 1 (parallel) | Write model classes locally + design API endpoints on paper |
| After pull | Drop models into their module folders |
| Add DbSets | Add DbSet lines to AppDbContext |
| Build MasterData | Repos + Services + Controllers for Site, Room, Provider, Service, ProviderService |
| Build Availability | Repos + Services + Controllers for templates, blocks, blackouts, slots |

### Dev 3 — Booking + Calendar + Notification
**Heaviest business logic — needs strongest backend developer.**

| Task | Details |
|---|---|
| Day 1 (parallel) | Write model classes + study appointment booking flow and state machines |
| After pull | Build Booking module (Appointment CRUD, CheckIn, Outcome, Waitlist) |
| State transitions | Enforce: Booked→CheckedIn→Completed/NoShow/Cancelled (no going backwards) |
| Calendar module | CalendarEvent and ResourceHold — read-optimized view of bookings |
| Notification module | Notification and ReminderSchedule — write-only sink for alerts |

### Dev 4 — Roster + Leave + Reports + Billing
**Staff operations + lightweight modules.**

| Task | Details |
|---|---|
| Day 1 (parallel) | Write model classes + study leave approval cascade flow |
| After pull | Build Roster module (ShiftTemplate, Roster, RosterAssignment, OnCallCoverage) |
| Build Leave module | LeaveRequest, LeaveImpact + the approval cascade logic |
| Reports module | OpsReport — aggregation queries |
| Billing module | ChargeRef — simple CRUD (optional Phase 1) |

---

## 7. Timeline

```
Day 1:   Dev 1 ████████████████  (Foundation — create solution, push by end of day)
         Dev 2 ██░░░░░░░░░░░░░░  (Prepare models + API design offline)
         Dev 3 ██░░░░░░░░░░░░░░  (Prepare models + study flows offline)
         Dev 4 ██░░░░░░░░░░░░░░  (Prepare models + study flows offline)

Day 2+:  Dev 1 ████████████████  (Identity + Admin + Rules modules)
         Dev 2 ████████████████  (MasterData → Availability)
         Dev 3 ████████████████  (Booking → Calendar → Notification)
         Dev 4 ████████████████  (Roster → Leave → Reports → Billing)
```

---

## 8. Git Strategy

- **main** → production-ready code
- **dev** → integration branch (all PRs merge here)
- **feature/{module}/{feature-name}** → individual work branches

Example: `feature/booking/appointment-crud`, `feature/masterdata/site-crud`

---

## 9. Conventions

- Controllers go in `CareSchedule.API/Controllers/` (not inside modules)
- One controller per major entity or logical group
- Use the `GenericRepository<T>` from Infrastructure for basic CRUD; write custom repository methods only when needed
- All status fields use string enums, not booleans
- Every mutation to a core entity must write an AuditLog entry
- Never hard-delete — set Status = Inactive/Cancelled
- All timestamps in UTC

---

*End of document. Refer to the project context doc for full schema details, API conventions, and business rules.*
