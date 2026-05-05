# Help Desk System - Comprehensive Test Suite (PRD v2.0)

This document contains the full suite of test cases required to validate the Help Desk Ticket Management System against the Version 2.0 requirements.

---

## 1. Role-Based Access Control (RBAC)
*Reference: PRD Section 2*

| TC ID | Feature | Role | Test Action | Expected Result |
| :--- | :--- | :--- | :--- | :--- |
| **RBAC-01** | Create Ticket | User | Raise a new ticket | Success. |
| **RBAC-02** | Create Ticket | Agent | Raise a new ticket | Denied (Agents cannot raise tickets). |
| **RBAC-03** | Create Ticket | Admin | Raise on behalf of user | Success. |
| **RBAC-04** | View Tickets | User | View other's ticket | Denied (Own only). |
| **RBAC-05** | View Tickets | Agent | View all tickets | Denied (Assigned only). |
| **RBAC-06** | View Tickets | Admin | View any ticket | Success. |
| **RBAC-07** | Assign Ticket | Agent | Assign ticket to self | Denied (Only Admin can assign). |
| **RBAC-08** | Close Ticket | Agent | Close assigned ticket | Denied (Only Admin can close). |

---

## 2. Ticket Lifecycle & Fields
*Reference: PRD Sections 3 & 4*

| TC ID | Requirement | Test Action | Expected Result |
| :--- | :--- | :--- | :--- |
| **LIFE-01** | Mandatory Fields | Submit without Title/Desc | Validation error. |
| **LIFE-02** | Department Field | Create ticket | Department is auto-filled or selectable. |
| **LIFE-03** | Auto-Status | Create ticket | Initial status is 'Open'. |
| **LIFE-04** | Escalation Sub-state | Escalate ticket | Ticket remains 'In Progress' but shows 'Escalated' flag. |
| **LIFE-05** | Reopen Ticket | Reopen closed ticket | Status moves to 'Reopened'; SLA timer resets to zero. |
| **LIFE-06** | Description Limit | Enter >2000 chars | Validation error. |

---

## 3. Service Level Agreements (SLA)
*Reference: PRD Section 6*

| TC ID | Requirement | Test Action | Expected Result |
| :--- | :--- | :--- | :--- |
| **SLA-01** | Priority Recalc | Change High -> Critical | SLA deadline is recalculated immediately. |
| **SLA-02** | Business Hours | Raise ticket at 8 PM | SLA timer starts next business day opening. |
| **SLA-03** | Pause on Hold | Move to 'On Hold' | SLA timer pauses; resumes on 'In Progress'. |
| **SLA-04** | Warning Trigger | Reach 75% of time | Status changes to 'Warning'; Notification sent. |
| **SLA-05** | Breach Action | Reach 100% of time | Status changes to 'Breached'; Auto-escalation triggered. |
| **SLA-06** | Holiday Exclusion | Timer spans a holiday | Holiday hours are not counted in SLA elapsed time. |
| **SLA-07** | Manual Override | Admin overrides deadline | Deadline updates; recorded in Audit Log. |

---

## 4. Email Notifications
*Reference: PRD Section 5*

| TC ID | Trigger | Recipient | Expected Content |
| :--- | :--- | :--- | :--- |
| **NOTI-01** | Comment Added | All Parties | First 200 characters of the comment. |
| **NOTI-02** | SLA Breach | Admin, User, Agent | Urgent subject line; direct ticket link. |
| **NOTI-03** | Account Welcome | New User | Temporary password + login URL. |
| **NOTI-04** | Retry Logic | Failed SMTP | Exponential backoff retry (3 attempts). |
| **NOTI-05** | Opt-out | User disables alerts | No notification sent for that event type. |

---

## 5. Escalation Workflow
*Reference: PRD Section 10*

| TC ID | Requirement | Test Action | Expected Result |
| :--- | :--- | :--- | :--- |
| **ESC-01** | Manual Escalation | Agent provides reason | Ticket flagged; Priority raised; Alerts sent. |
| **ESC-02** | Auto-Escalation | Critical ticket unassigned 30m | System auto-escalates at 30m mark. |
| **ESC-03** | Reopen Auto-ESC | Reopen same ticket 3 times | 3rd reopen triggers auto-escalation. |
| **ESC-04** | Acknowledgement | Admin acknowledges ESC | Highlighting removed; timestamp recorded. |

---

## 6. Knowledge Base (KB)
*Reference: PRD Section 9*

| TC ID | Requirement | Test Action | Expected Result |
| :--- | :--- | :--- | :--- |
| **KB-01** | Draft Visibility | User searches KB | Draft articles do not appear in results. |
| **KB-02** | Version History | Edit article 3 times | 3 versions exist; Admin can restore v1. |
| **KB-03** | Ticket Suggestion | Type "Wifi" in ticket title | KB articles about Wifi suggested in sidebar. |
| **KB-04** | Resolved via KB | Agent resolves using KB | Ticket linked to article; tracked in reports. |
| **KB-05** | Feedback | Click 'Thumbs Up' | Helpful count increases; user cannot click twice. |

---

## 7. Reporting & Analytics
*Reference: PRD Section 7*

| TC ID | Requirement | Test Action | Expected Result |
| :--- | :--- | :--- | :--- |
| **REP-01** | Date Range | Filter by 'Last Month' | Only tickets from last month included. |
| **REP-02** | PDF Export | Export volume report | Branded PDF generated with charts included. |
| **REP-03** | Async Large Export | Export 15,000 rows | "Export in progress" msg; Email sent when ready. |
| **REP-04** | Agent Dashboard | Agent views performance | Shows average resolution time & personal CSAT only. |

---

## 8. Audit Trail
*Reference: PRD Section 8*

| TC ID | Requirement | Test Action | Expected Result |
| :--- | :--- | :--- | :--- |
| **AUD-01** | Log Integrity | Admin tries to delete log | Action rejected (Logs are read-only). |
| **AUD-02** | Field Capture | Change ticket priority | Log records Actor, IP, Old Value, New Value. |
| **AUD-03** | Retention | Query 11-month old log | Data is available. |
| **AUD-04** | IP Tracking | Login from IP X | Audit log captures IP X for the login event. |

---

## 9. CSAT & User Feedback
*Reference: PRD Section 11*

| TC ID | Requirement | Test Action | Expected Result |
| :--- | :--- | :--- | :--- |
| **CSAT-01** | Survey Trigger | Close ticket | User receives survey link after 30 mins (default). |
| **CSAT-02** | Survey Expiry | Open link after 8 days | "Link Expired" message displayed. |
| **CSAT-03** | Anonymity | Agent with 2 responses | CSAT score hidden (Min 5 req). |
| **CSAT-04** | Duplicate Block | Submit survey twice | Second submission rejected. |

---

## 10. Recurring Ticket Templates
*Reference: PRD Section 12*

| TC ID | Requirement | Test Action | Expected Result |
| :--- | :--- | :--- | :--- |
| **REC-01** | CRON Pattern | Set weekly schedule | Ticket generated automatically every week. |
| **REC-02** | Holiday Deferral | Template fires on holiday | Generation deferred to next business day. |
| **REC-03** | Manual Run | Click 'Run Now' on template | Ticket generated immediately regardless of schedule. |

---

## 11. System Administration
*Reference: PRD Sections 13 & 14*

| TC ID | Requirement | Test Action | Expected Result |
| :--- | :--- | :--- | :--- |
| **ADM-01** | Bulk User Import | Upload CSV with error | Errors listed; no users created until fixed. |
| **ADM-02** | Dept Deactivation | Deactivate dept with users | Action rejected (Must move users first). |
| **ADM-03** | Session Timeout | Idle for 30 minutes | User is automatically logged out. |
| **ADM-04** | Data Archival | Configure 12-month policy | Tickets closed >12 months ago moved to archive. |
