import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';
export const routes: Routes = [
    // ── Public ─────────────────────────────────────────────────
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

    // ── Protected ───────────────────────────────────────────────
    {
        path: '',
        canActivate: [authGuard],
        loadComponent: () =>
            import('./shared/layout/main-layout/main-layout.component')
                .then(m => m.MainLayoutComponent),
        children: [

            // Default redirect
            { path: '', redirectTo: 'tickets', pathMatch: 'full' },

            // ── Tickets (All roles) ─────────────────────────────────
            {
                path: 'tickets',
                loadComponent: () =>
                    import('./features/tickets/ticket-list/ticket-list.component')
                        .then(m => m.TicketListComponent)
            },

            // ⚠️ /tickets/new MUST be before /tickets/:id
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

            // ── Admin ────────────────────────────────────────────────
            {
                path: 'dashboard',
                canActivate: [roleGuard],
                data: { roles: ['Admin'] },
                loadComponent: () =>
                    import('./features/admin/dashboard/dashboard.component')
                        .then(m => m.DashboardComponent)
            },
            {
                path: 'admin/users',
                canActivate: [roleGuard],
                data: { roles: ['Admin'] },
                loadComponent: () =>
                    import('./features/admin/user-management/user-management.component')
                        .then(m => m.UserManagementComponent)
            },
            {
                path: 'admin/categories',
                canActivate: [roleGuard],
                data: { roles: ['Admin'] },
                loadComponent: () =>
                    import('./features/admin/category-management/category-management.component')
                        .then(m => m.CategoryManagementComponent)
            },

            // ── Agent ────────────────────────────────────────────────
            {
                path: 'agent/tickets',
                canActivate: [roleGuard],
                data: { roles: ['Agent'] },
                loadComponent: () =>
                    import('./features/agent/agent-tickets/agent-tickets.component')
                        .then(m => m.AgentTicketsComponent)
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