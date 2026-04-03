// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';

export const routes: Routes = [
    {
        path: 'login',
        loadComponent: () =>
            import('./features/auth/login/login.component').then(m => m.LoginComponent)
    },
    {
        path: '',
        loadComponent: () =>
            import('./shared/layout/main-layout/main-layout.component').then(m => m.MainLayoutComponent),
        canActivate: [authGuard],
        children: [
            { path: '', redirectTo: 'tickets', pathMatch: 'full' },
            {
                path: 'tickets',
                loadComponent: () =>
                    import('./features/tickets/ticket-list/ticket-list.component').then(m => m.TicketListComponent)
            },
            {
                path: 'tickets/new',
                canActivate: [roleGuard],
                data: { roles: ['Admin', 'User'] },
                loadComponent: () =>
                    import('./features/tickets/ticket-create/ticket-create.component').then(m => m.TicketCreateComponent)
            },
            {
                path: 'tickets/:id',
                loadComponent: () =>
                    import('./features/tickets/ticket-detail/ticket-detail.component').then(m => m.TicketDetailComponent)
            },
            {
                path: 'dashboard',
                canActivate: [roleGuard],
                data: { roles: ['Admin'] },
                loadComponent: () =>
                    import('./features/admin/dashboard/dashboard.component').then(m => m.DashboardComponent)
            },
            {
                path: 'admin/users',
                canActivate: [roleGuard],
                data: { roles: ['Admin'] },
                loadComponent: () =>
                    import('./features/admin/user-management/user-management.component').then(m => m.UserManagementComponent)
            },
            {
                path: 'admin/categories',
                canActivate: [roleGuard],
                data: { roles: ['Admin'] },
                loadComponent: () =>
                    import('./features/admin/category-management/category-management.component').then(m => m.CategoryManagementComponent)
            },
            {
                path: 'agent/tickets',
                canActivate: [roleGuard],
                data: { roles: ['Agent'] },
                loadComponent: () =>
                    import('./features/agent/agent-tickets/agent-tickets.component').then(m => m.AgentTicketsComponent)
            },
        ]
    },
    {
        path: '**',
        loadComponent: () =>
            import('./shared/components/page-not-found/page-not-found.component').then(m => m.PageNotFoundComponent)
    }
];