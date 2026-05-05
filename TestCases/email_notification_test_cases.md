# Email Notification Test Cases (PRD v2.0)

This document outlines the comprehensive test suite for the Help Desk Email Notification system, covering triggers, templates, delivery, and user preferences.

## 1. Trigger Matrix Verification (Functional)

| TC ID | Event | Expected Recipient(s) | Verification Points |
| :--- | :--- | :--- | :--- |
| **EN-01** | Ticket Created | User, Admin | Ticket ID, Summary, confirmation message for User. |
| **EN-02** | Ticket Assigned | Agent, User | Agent name included, direct deep-link to ticket. |
| **EN-03** | Status Change | User (Owner) | Correct status reflected (e.g., In Progress, On Hold). |
| **EN-04** | New Comment | All Ticket Parties | First 200 chars of comment included in email. |
| **EN-05** | Ticket Closed | User (Owner) | Includes resolution summary + CSAT survey link. |
| **EN-06** | Ticket Reopened | Agent, Admin | Includes reason for reopening. |
| **EN-07** | Escalated | Admin, Dept Head | Marked 'URGENT', includes escalation reason. |
| **EN-08** | SLA Warning (75%) | Admin, Agent | Warning about impending breach. |
| **EN-09** | SLA Breached | Admin, Agent, User | Urgent notification; flag as SLA Breach. |
| **EN-10** | Agent Reassigned | Old Agent, New Agent, User | Both agents notified of the transition. |
| **EN-11** | Welcome Email | New User | Temporary password + login link included. |
| **EN-12** | CSAT Survey | User | Sent 30 mins after closure (configurable delay). |

## 2. Template & Rendering (Visual & Compatibility)

| TC ID | Requirement | Test Action | Expected Result |
| :--- | :--- | :--- | :--- |
| **ER-01** | Branded HTML | View email in Outlook/Gmail | Company logo, ticket ID, title, and status badge present. |
| **ER-02** | Plain-Text Fallback | Disable HTML in mail client | Readable plain-text version is displayed. |
| **ER-03** | Deep-Link | Click "View Ticket" button | User is navigated directly to the ticket detail page. |
| **ER-04** | Size Limit | Inspect sent email size | Total email size must be < 100 KB. |
| **ER-05** | Branded Footer | Check email footer | Includes copyright, system name, and preferences link. |

## 3. User Preferences & Permissions

| TC ID | Scenario | Test Action | Expected Result |
| :--- | :--- | :--- | :--- |
| **UP-01** | Global Disable | User disables all notifications | No emails (except mandatory) are sent to the user. |
| **UP-02** | Selective Enable | User enables only 'Comments' | User receives comment alerts but no status updates. |
| **UP-03** | Admin Mandatory | Admin tries to disable SLA alerts | System prevents disabling mandatory alerts. |
| **UP-04** | Dept Head Alert | Ticket in Dept A escalated | Only Dept Head A (and Admin) receives notification. |
| **UP-05** | Immediate Effect | Change preference, then trigger | Preference change is applied immediately (no lag). |

## 4. Delivery & Reliability (Non-Functional)

| TC ID | Scenario | Test Action | Expected Result |
| :--- | :--- | :--- | :--- |
| **DR-01** | Asynchronous Send | Perform action (e.g. Comment) | Action completes instantly; email follows in background. |
| **DR-02** | Retry Strategy | Simulate SMTP failure | System retries at 30s, 2m, 10m intervals. |
| **DR-03** | Max Retries | Failure after 3 retries | System logs failure and stops retrying. |
| **DR-04** | Audit Logging | Check 'Audit Log' table | Every send (success/fail) is logged with timestamp/outcome. |
| **DR-05** | Provider Switch | Toggle SMTP to SendGrid | System seamlessly switches delivery engine via config. |

## 5. Edge Cases

| TC ID | Scenario | Expected Result |
| :--- | :--- | :--- |
| **EC-01** | User deactivated during retry | System should handle recipient not found/inactive gracefully. |
| **EC-02** | Large comment (>2000 chars) | Email snippet must correctly truncate at 200 characters. |
| **EC-03** | Batch reassignment | System should not flood user if 10 tickets reassigned at once (debouncing). |
| **EC-04** | Survey link expiry | Clicking survey link after 7 days should show "Expired" message. |
| **EC-05** | System name change | Email headers/footer update immediately after Admin config change. |
