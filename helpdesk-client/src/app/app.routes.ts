import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';

export const routes: Routes = [
    // ── Public Routes ───────────────────────────────────────────
    {
        path: 'login',
        loadComponent: () =>
            import('./shared/layout/auth-layout/auth-layout.component')
                .then(m => m.AuthLayoutComponent),
        children: [{
            path: '',
            loadComponent: () =>
                import('./features/auth/login/login.component')
                    .then(m => m.LoginComponent)
        }]
    },
    {
        path: 'mfa-verify',
        loadComponent: () =>
            import('./shared/layout/auth-layout/auth-layout.component')
                .then(m => m.AuthLayoutComponent),
        children: [{
            path: '',
            loadComponent: () =>
                import('./features/auth/mfa-verify/mfa-verify.component')
                    .then(m => m.MfaVerifyComponent)
        }]
    },

    // ── Protected Routes ────────────────────────────────────────
    {
        path: '',
        canActivate: [authGuard],
        loadComponent: () =>
            import('./shared/layout/main-layout/main-layout.component')
                .then(m => m.MainLayoutComponent),
        children: [

            // Default redirect
            { path: '', redirectTo: 'dashboard', pathMatch: 'full' },

            // ── Dashboard (Admin) ─────────────────────────────────
            {
                path: 'dashboard',
                canActivate: [roleGuard],
                data: { roles: ['Admin'] },
                loadComponent: () =>
                    import('./features/admin/dashboard/dashboard.component')
                        .then(m => m.DashboardComponent)
            },

            // ── Tickets (All Roles) ──────────────────────────────
            {
                path: 'tickets',
                loadComponent: () =>
                    import('./features/tickets/ticket-list/ticket-list.component')
                        .then(m => m.TicketListComponent)
            },
            {
                path: 'tickets/new',
                canActivate: [roleGuard],
                data: { roles: ['Admin', 'User'] },
                loadComponent: () =>
                    import('./features/tickets/ticket-create/ticket-create.component')
                        .then(m => m.TicketCreateComponent)
            },
            {
                path: 'tickets/:id',
                loadComponent: () =>
                    import('./features/tickets/ticket-detail/ticket-detail.component')
                        .then(m => m.TicketDetailComponent)
            },

            // ── Admin - User Management ───────────────────────────
            {
                path: 'admin/users',
                canActivate: [roleGuard],
                data: { roles: ['Admin'] },
                loadComponent: () =>
                    import('./features/admin/user-management/user-management.component')
                        .then(m => m.UserManagementComponent)
            },

            // ── Admin - Categories ───────────────────────────────
            {
                path: 'admin/categories',
                canActivate: [roleGuard],
                data: { roles: ['Admin'] },
                loadComponent: () =>
                    import('./features/admin/category-management/category-management.component')
                        .then(m => m.CategoryManagementComponent)
            },

            // ── Admin - Departments ──────────────────────────────
            {
                path: 'admin/departments',
                canActivate: [roleGuard],
                data: { roles: ['Admin'] },
                loadComponent: () =>
                    import('./features/admin/department-management/department-management.component')
                        .then(m => m.DepartmentManagementComponent)
            },

            // ── Admin - Reports ──────────────────────────────────
            {
                path: 'admin/reports',
                canActivate: [roleGuard],
                data: { roles: ['Admin'] },
                loadComponent: () =>
                    import('./features/admin/reports/reports.component')
                        .then(m => m.ReportsComponent)
            },

            // ── Admin - Audit Log ────────────────────────────────
            {
                path: 'admin/audit',
                canActivate: [roleGuard],
                data: { roles: ['Admin'] },
                loadComponent: () =>
                    import('./features/admin/audit-log/audit-log.component')
                        .then(m => m.AuditLogComponent)
            },

            // ── Admin - Settings ─────────────────────────────────
            {
                path: 'admin/settings',
                canActivate: [roleGuard],
                data: { roles: ['Admin'] },
                loadComponent: () =>
                    import('./features/admin/settings/settings.component')
                        .then(m => m.SettingsComponent)
            },

            // ── Admin - Recurring Templates ──────────────────────
            {
                path: 'admin/recurring-templates',
                canActivate: [roleGuard],
                data: { roles: ['Admin'] },
                loadComponent: () =>
                    import('./features/admin/recurring-templates/recurring-templates.component')
                        .then(m => m.RecurringTemplatesComponent)
            },

            // ── Agent - Tickets ──────────────────────────────────
            {
                path: 'agent/tickets',
                canActivate: [roleGuard],
                data: { roles: ['Agent'] },
                loadComponent: () =>
                    import('./features/agent/agent-tickets/agent-tickets.component')
                        .then(m => m.AgentTicketsComponent)
            },

            // ── Knowledge Base ───────────────────────────────────
            {
                path: 'kb',
                loadComponent: () =>
                    import('./features/knowledge-base/kb-list/kb-list.component')
                        .then(m => m.KbListComponent)
            },
            {
                path: 'kb/:id',
                loadComponent: () =>
                    import('./features/knowledge-base/kb-detail/kb-detail.component')
                        .then(m => m.KbDetailComponent)
            },
            {
                path: 'kb/create',
                canActivate: [roleGuard],
                data: { roles: ['Admin', 'Agent'] },
                loadComponent: () =>
                    import('./features/knowledge-base/kb-editor/kb-editor.component')
                        .then(m => m.KbEditorComponent)
            },
            {
                path: 'kb/edit/:id',
                canActivate: [roleGuard],
                data: { roles: ['Admin', 'Agent'] },
                loadComponent: () =>
                    import('./features/knowledge-base/kb-editor/kb-editor.component')
                        .then(m => m.KbEditorComponent)
            },

            // ── Profile ──────────────────────────────────────────
            {
                path: 'profile',
                loadComponent: () =>
                    import('./features/profile/profile.component')
                        .then(m => m.ProfileComponent)
            },
            {
                path: 'profile/security',
                loadComponent: () =>
                    import('./features/profile/security/security.component')
                        .then(m => m.SecurityComponent)
            },

            // ── Notifications ────────────────────────────────────
            {
                path: 'notifications',
                loadComponent: () =>
                    import('./features/notifications/notifications.component')
                        .then(m => m.NotificationsComponent)
            },

        ]
    },

    // ── 404 ──────────────────────────────────────────────────────
    {
        path: '**',
        loadComponent: () =>
            import('./shared/components/page-not-found/page-not-found.component')
                .then(m => m.PageNotFoundComponent)
    }
];