# HelpDesk API Contract

> **Base URL:** `https://{host}:{port}`  
> **Authentication:** JWT Bearer Token — Include `Authorization: Bearer <token>` on all protected endpoints.  
> **Standard Response Envelope:**
> ```json
> { "success": true, "message": "...", "data": { ... } }
> ```
> **Roles:** `Admin` · `Agent` · `User`

---

## Project Overview

The HelpDesk backend is a multi-tenant, role-based IT support ticketing system built with **ASP.NET Core 8**, **Entity Framework Core**, and **SQL Server**. It follows **Clean Architecture** (Domain → Application → Infrastructure → API) and implements:

- **Ticket Lifecycle Management** — Create, assign, escalate, resolve, close, reopen tickets.
- **SLA Engine** — Policy-based deadlines, breach tracking, and admin override.
- **Email Notifications** — Razor-templated HTML emails for every ticket lifecycle event, with per-user preference controls.
- **CSAT Surveys** — Automated satisfaction surveys delivered via email 1 minute (test) / 30 minutes (production) after ticket closure.
- **Knowledge Base** — Versioned KB articles, search, suggestions, and feedback.
- **Recurring Templates** — Scheduled/manual ticket auto-generation.
- **Reporting & Exports** — Ticket volume, agent performance, SLA compliance, CSV/PDF exports.
- **Audit Trail** — Full change history captured on every entity save.

---

## 1. Authentication

### POST `/api/auth/login`
> Authenticate a user and receive a JWT access token.

**Auth:** Public

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "P@ssword1"
}
```

**Response `200 OK`:**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "user@example.com",
    "role": "Admin"
  }
}
```

**Response `401 Unauthorized`:**
```json
{ "success": false, "message": "Invalid credentials." }
```

---

## 2. Tickets

> **Base route:** `/api/tickets`  
> **Default Auth:** Bearer JWT (all roles)

---

### POST `/api/tickets/createTicket`
> Create a new support ticket.

**Auth:** Admin, User

**Request Body:**
```json
{
  "title": "Laptop not booting",
  "description": "My laptop shows a blue screen on startup.",
  "priority": "High",
  "categoryId": "guid",
  "affectedAsset": "Dell XPS 15"
}
```

**Response `200 OK`:**
```json
{
  "success": true,
  "data": {
    "ticketId": "guid",
    "title": "Laptop not booting",
    "status": "Open",
    "priority": "High"
  }
}
```

---

### POST `/api/tickets/GetAllTicket`
> Fetch paginated list of tickets (filtered by role automatically).

**Auth:** All Roles

**Request Body:**
```json
{
  "page": 1,
  "pageSize": 10,
  "search": "laptop",
  "statusFilter": "Open",
  "priorityFilter": "High",
  "sortBy": "CreatedAt",
  "sortDescending": true
}
```

**Response `200 OK`:**
```json
{
  "success": true,
  "data": {
    "items": [ { "ticketId": "guid", "title": "...", "status": "Open", "priority": "High" } ],
    "totalCount": 42,
    "page": 1,
    "pageSize": 10
  }
}
```

---

### GET `/api/tickets/getByIdTicket`
> Get full details of a single ticket.

**Auth:** All Roles

**Request Body:**
```json
{ "id": "ticket-guid" }
```

**Response `200 OK`:**
```json
{
  "success": true,
  "data": {
    "id": "guid",
    "title": "...",
    "description": "...",
    "status": "InProgress",
    "priority": "High",
    "category": "...",
    "assignedAgent": "John Doe",
    "slaDeadline": "2026-05-06T10:00:00Z",
    "slaStatus": "WithinSla",
    "comments": []
  }
}
```

---

### PUT `/api/tickets/Agent-assign`
> Assign a ticket to an agent.

**Auth:** Admin

**Request Body:**
```json
{
  "ticketId": "ticket-guid",
  "agentId": "agent-user-guid"
}
```

**Response `200 OK`:**
```json
{ "success": true, "message": "Ticket assigned successfully.", "data": { ... } }
```

---

### PUT `/api/tickets/UpdateTicketStatus`
> Change the status of a ticket.

**Auth:** Admin, Agent

**Request Body:**
```json
{
  "ticketId": "ticket-guid",
  "newStatus": "InProgress",
  "comment": "Started investigating."
}
```
> **Valid statuses:** `Open` · `InProgress` · `OnHold` · `Resolved` · `Closed`

**Response `200 OK`:**
```json
{ "success": true, "data": { "status": "InProgress" } }
```

---

### PUT `/api/tickets/UpdatePriority`
> Change the priority of a ticket.

**Auth:** Admin

**Request Body:**
```json
{
  "ticketId": "ticket-guid",
  "priority": "Critical"
}
```
> **Valid priorities:** `Low` · `Medium` · `High` · `Critical`

---

### PUT `/api/tickets/CloseTicket`
> Close a ticket and trigger a CSAT survey email.

**Auth:** Admin

**Request Body:**
```json
{ "id": "ticket-guid" }
```

**Response `200 OK`:**
```json
{ "success": true, "message": "Ticket closed.", "data": { "status": "Closed" } }
```

> ℹ️ A CSAT survey email is automatically sent to the raiser after a configured delay (1 min test / 30 min prod).

---

### PUT `/api/tickets/Ticket-reopen`
> Reopen a previously closed or resolved ticket.

**Auth:** Admin

**Request Body:**
```json
{ "id": "ticket-guid" }
```

---

### POST `/api/tickets/Add-Comment`
> Add a comment to a ticket thread.

**Auth:** All Roles

**Request Body:**
```json
{
  "ticketId": "ticket-guid",
  "content": "Can you provide more details?",
  "isInternal": false
}
```

**Response `200 OK`:**
```json
{
  "success": true,
  "data": {
    "commentId": "guid",
    "content": "...",
    "authorName": "Jane Smith",
    "createdAt": "2026-05-05T10:00:00Z"
  }
}
```

---

### POST `/api/tickets/escalate`
> Escalate a ticket to admin attention.

**Auth:** Admin, Agent

**Request Body:**
```json
{
  "ticketId": "ticket-guid",
  "reason": "Customer is a VIP and SLA is about to breach."
}
```

---

### POST `/api/tickets/acknowledgeEscalation`
> Acknowledge an escalation on a ticket.

**Auth:** Admin

**Request Body:**
```json
{ "id": "ticket-guid" }
```

---

### POST `/api/tickets/overrideSla`
> Override the SLA deadline for a ticket.

**Auth:** Admin

**Request Body:**
```json
{
  "ticketId": "ticket-guid",
  "newDeadline": "2026-05-10T18:00:00Z",
  "reason": "Customer requested extended timeline."
}
```

---

### POST `/api/tickets/markResolvedViaKb`
> Mark a ticket as resolved using a Knowledge Base article.

**Auth:** Admin, Agent

**Request Body:**
```json
{
  "ticketId": "ticket-guid",
  "articleId": "kb-article-guid"
}
```

---

### POST `/api/tickets/getArchived`
> Retrieve paginated list of archived tickets.

**Auth:** Admin

**Request Body:**
```json
{ "page": 1, "pageSize": 20 }
```

---

### GET `/csat/{id}`
> Render the CSAT survey web page (linked from email). Returns HTML.

**Auth:** Public (Anonymous)

**URL Params:** `id` — Ticket GUID

**Response:** `text/html` — Interactive survey page

---

### POST `/api/tickets/submit-survey`
> Submit a customer satisfaction rating for a resolved ticket.

**Auth:** Public (Anonymous)

**Request Body:**
```json
{
  "ticketId": "ticket-guid",
  "rating": 5,
  "comments": "The agent was very helpful and resolved the issue quickly."
}
```

**Response `200 OK`:**
```json
{ "success": true, "message": "Thank you for your feedback!" }
```

**Response `400 Bad Request`:**
```json
{ "success": false, "message": "Survey already submitted for this ticket." }
```

---

### GET `/api/tickets/view/{id}`
> Render a browser-viewable HTML page for a ticket (for email links).

**Auth:** Public (Anonymous)

**URL Params:** `id` — Ticket GUID

**Response:** `text/html`

---

## 3. Users

> **Base route:** `/api/users`  
> **Default Auth:** Admin only

---

### POST `/api/users`
> Create a new user account and send a welcome email with a temporary password.

**Request Body:**
```json
{
  "fullName": "Jane Smith",
  "email": "jane@company.com",
  "role": "Agent",
  "departmentId": "dept-guid"
}
```

---

### GET `/api/users/getAll`
> List all users in the system.

**Response `200 OK`:**
```json
{
  "success": true,
  "data": [
    { "id": "guid", "fullName": "...", "email": "...", "role": "Agent", "isActive": true }
  ]
}
```

---

### GET `/api/users/getById?Id={guid}`
> Get details of a specific user.

---

### PUT `/api/users/UpdateUsersRole`
> Change a user's role.

**Request Body:**
```json
{
  "userId": "user-guid",
  "newRole": "Agent"
}
```

---

### PUT `/api/users/DeleteUser`
> Soft-deactivate a user account.

**Request Body:**
```json
{ "id": "user-guid" }
```

---

### GET `/api/users/agents/active`
> Get a list of all currently active agents.

---

### POST `/api/users/moveDepartment`
> Move a user to a different department.

**Request Body:**
```json
{
  "userId": "user-guid",
  "departmentId": "dept-guid"
}
```

---

### POST `/api/users/bulkImport`
> Import multiple users at once from a JSON array.

**Request Body:**
```json
[
  { "fullName": "Alice", "email": "alice@co.com", "role": "User", "departmentId": "guid" },
  { "fullName": "Bob", "email": "bob@co.com", "role": "Agent", "departmentId": "guid" }
]
```

---

## 4. Categories

> **Base route:** `/api/categories`  
> **Default Auth:** Bearer JWT

---

### GET `/api/categories`
> Get all ticket categories (All roles).

---

### POST `/api/categories`
> Create a new category. **(Admin)**

**Request Body:**
```json
{ "name": "Network", "description": "Network related issues" }
```

---

### POST `/api/categories/update`
> Update an existing category. **(Admin)**

**Request Body:**
```json
{ "id": "guid", "name": "Network Issues", "description": "Updated description" }
```

---

### PUT `/api/categories/ActivateCategory`
> Toggle a category's active/inactive state. **(Admin)**

**Request Body:**
```json
{ "id": "category-guid" }
```

---

## 5. Departments

> **Base route:** `/api/departments`  
> **Default Auth:** Admin only

---

### GET `/api/departments/getAll`
### POST `/api/departments/getDeptById` — Body: `{ "id": "guid" }`
### POST `/api/departments/getDeptSummary` — Body: `{ "id": "guid" }`

### POST `/api/departments/create`
**Request Body:**
```json
{ "name": "IT Support", "description": "Handles all IT tickets" }
```

### POST `/api/departments/update`
**Request Body:**
```json
{ "id": "guid", "name": "IT Support Updated", "description": "..." }
```

### POST `/api/departments/deactivate`
**Request Body:** `{ "id": "guid" }`

### POST `/api/departments/assignHead`
**Request Body:**
```json
{ "departmentId": "guid", "userId": "user-guid" }
```

---

## 6. CSAT (Customer Satisfaction)

> **Base route:** `/api/csat`

---

### POST `/api/csat/submit`
> Submit a CSAT rating (used for API-based submission by authenticated users).

**Auth:** User

**Request Body:**
```json
{
  "ticketId": "ticket-guid",
  "score": 4,
  "comments": "Good service, but slightly delayed."
}
```

---

### POST `/api/csat/getAgentStats`
> Get aggregated CSAT statistics for a specific agent in a date range.

**Auth:** Admin

**Request Body:**
```json
{
  "agentId": "agent-guid",
  "from": "2026-01-01T00:00:00Z",
  "to": "2026-05-01T00:00:00Z"
}
```

**Response `200 OK`:**
```json
{
  "success": true,
  "data": {
    "averageScore": 4.2,
    "totalResponses": 35
  }
}
```

---

## 7. Knowledge Base

> **Base route:** `/api/knowledgebase`

---

### GET `/api/knowledgebase/getAll?status={status}`
> Get all KB articles. Filter by status (`Draft`, `Published`, `Archived`). Auth: All roles.

### POST `/api/knowledgebase/getById` — Body: `{ "id": "guid" }`

### GET `/api/knowledgebase/search?keyword={keyword}`
> Full-text search across KB articles.

### GET `/api/knowledgebase/suggest?title={title}`
> Get KB article suggestions based on ticket title (for auto-suggestion).

### POST `/api/knowledgebase/create`
**Auth:** Admin, Agent

**Request Body:**
```json
{
  "title": "How to reset VPN credentials",
  "content": "Full article markdown content...",
  "categoryId": "guid",
  "tags": ["vpn", "credentials", "reset"]
}
```

### POST `/api/knowledgebase/update`
**Auth:** Admin, Agent

**Request Body:**
```json
{ "id": "guid", "title": "...", "content": "...", "categoryId": "guid", "tags": [] }
```

### POST `/api/knowledgebase/publish` — Body: `{ "id": "guid" }` — **Auth: Admin**
### POST `/api/knowledgebase/unpublish` — Body: `{ "id": "guid" }` — **Auth: Admin**
### POST `/api/knowledgebase/delete` — Body: `{ "id": "guid" }` — **Auth: Admin**

### POST `/api/knowledgebase/submitFeedback`
**Request Body:**
```json
{ "articleId": "guid", "isHelpful": true }
```

---

## 8. Notification Preferences

> **Base route:** `/api/notification-preferences`  
> **Default Auth:** Bearer JWT (all roles)

---

### GET `/api/notification-preferences/getMine`
> Get the current user's notification preferences.

**Response:**
```json
{
  "success": true,
  "data": [
    { "eventType": "TicketCreated", "isEnabled": true },
    { "eventType": "TicketClosed", "isEnabled": true },
    { "eventType": "SurveyRequest", "isEnabled": true }
  ]
}
```

---

### POST `/api/notification-preferences/upsert`
> Enable or disable a specific notification event.

**Request Body:**
```json
{
  "eventType": "SurveyRequest",
  "isEnabled": false
}
```

> ⚠️ **Admin cannot disable** `SlaBreached` or `TicketEscalated` notifications (PRD 5.3).

---

## 9. Reports

> **Base route:** `/api/reports`  
> **Default Auth:** Admin (except `agentSelf`)

---

### POST `/api/reports/ticketVolume`
> Ticket volume breakdown by status, priority, and category.

**Request Body:**
```json
{
  "from": "2026-01-01T00:00:00Z",
  "to": "2026-05-01T00:00:00Z",
  "departmentId": "guid-or-null"
}
```

### POST `/api/reports/agentPerformance`
**Request Body:**
```json
{
  "agentId": "guid-or-null",
  "filter": { "from": "2026-01-01T00:00:00Z", "to": "2026-05-01T00:00:00Z" }
}
```

### POST `/api/reports/slaCompliance`
**Request Body:** Same as `ticketVolume` filter.

### GET `/api/reports/agentSelf`
> Agent's own performance metrics. **(Auth: Agent)**

### POST `/api/reports/exportTicketsCsv`
> Download tickets as a CSV file.

**Request Body:** Report filter — Returns `text/csv` file.

### POST `/api/reports/exportTicketsPdf`
> Download tickets as a PDF file.

**Request Body:** Report filter — Returns `application/pdf` file.

### POST `/api/reports/exportAuditCsv`
> Export the audit log as a CSV file.

**Request Body:**
```json
{
  "from": "2026-01-01T00:00:00Z",
  "to": "2026-05-01T00:00:00Z"
}
```

---

## 10. Audit Log

> **Base route:** `/api/audit`  
> **Default Auth:** Bearer JWT

---

### POST `/api/audit/getAll`
> Paginated, filterable audit log.

**Request Body:**
```json
{
  "from": "2026-01-01T00:00:00Z",
  "to": "2026-05-05T00:00:00Z",
  "actor": "user-id-or-null",
  "action": "Modified",
  "entityType": "Ticket",
  "page": 1,
  "pageSize": 20
}
```

### POST `/api/audit/getByEntity`
> Get all audit entries for a specific entity.

**Request Body:** `{ "id": "entity-guid" }`

---

## 11. Dashboard

> **Base route:** `/api/dashboard`  
> **Default Auth:** Bearer JWT

---

### GET `/api/dashboard`
> Get system-wide dashboard statistics.

**Response `200 OK`:**
```json
{
  "success": true,
  "data": {
    "totalTickets": 120,
    "openTickets": 35,
    "resolvedToday": 12,
    "slaBreached": 3,
    "avgResolutionTimeHours": 4.5,
    "topCategories": [ { "name": "Network", "count": 30 } ]
  }
}
```

---

## 12. Recurring Templates

> **Base route:** `/api/recurring-templates`  
> **Default Auth:** Bearer JWT

---

### GET `/api/recurring-templates/getAll`
> List all recurring ticket templates.

### POST `/api/recurring-templates/getById` — Body: `{ "id": "guid" }`

### POST `/api/recurring-templates/create`
**Request Body:**
```json
{
  "templateName": "Weekly Server Check",
  "title": "Perform weekly server health check",
  "description": "Check CPU, memory, and disk usage on all servers.",
  "priority": "Medium",
  "categoryId": "guid",
  "cronExpression": "0 9 * * MON"
}
```

### POST `/api/recurring-templates/toggleActive` — Body: `{ "id": "guid" }`
### POST `/api/recurring-templates/triggerManual` — Body: `{ "id": "guid" }`
### POST `/api/recurring-templates/delete` — Body: `{ "id": "guid" }`

---

## 13. System Settings

> **Base route:** `/api/systemsettings`  
> **Default Auth:** Admin only

---

### GET `/api/systemsettings/getAll`
> Retrieve all system configuration key-value pairs.

### POST `/api/systemsettings/getByKey`
**Request Body:** `{ "key": "SLA_RESPONSE_HOURS" }`

### POST `/api/systemsettings/update`
**Request Body:**
```json
{ "key": "SLA_RESPONSE_HOURS", "value": "8" }
```

### POST `/api/systemsettings/sendTestEmail`
> Send a test email to verify SMTP configuration.

**Request Body:**
```json
{ "toEmail": "admin@company.com" }
```

---

## Appendix — Common Enums

| Enum | Values |
|---|---|
| `TicketStatus` | `Open`, `InProgress`, `OnHold`, `Resolved`, `Closed` |
| `TicketPriority` | `Low`, `Medium`, `High`, `Critical` |
| `UserRole` | `Admin`, `Agent`, `User` |
| `KbArticleStatus` | `Draft`, `Published`, `Archived` |
| `NotificationEventType` | `TicketCreated`, `TicketAssigned`, `TicketStatusChanged`, `TicketClosed`, `TicketEscalated`, `SlaBreached`, `CommentAdded`, `SurveyRequest`, `WelcomeEmail` |
| `SlaStatus` | `WithinSla`, `AtRisk`, `Breached` |

---

## Appendix — HTTP Status Codes

| Code | Meaning |
|---|---|
| `200 OK` | Success |
| `400 Bad Request` | Validation error or business rule violation |
| `401 Unauthorized` | Missing or invalid JWT token |
| `403 Forbidden` | Valid token but insufficient role |
| `404 Not Found` | Resource not found |
| `500 Internal Server Error` | Unhandled server error |
