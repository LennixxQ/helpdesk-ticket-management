import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable, tap } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';

export interface NotificationItem {
    id: string;
    title: string;
    message: string;
    type: string;
    createdAt: string;
    routeUrl: string;
    isRead: boolean;
}

@Injectable({ providedIn: 'root' })
export class NotificationAlertService {
    private http = inject(HttpClient);
    private apiUrl = `${environment.apiUrl}/notifications`;
    
    notifications = signal<NotificationItem[]>([]);
    unreadCount = signal<number>(0);
    
    private readonly READ_KEY = 'read_notifications';
    
    getReadIds(): string[] {
        const stored = localStorage.getItem(this.READ_KEY);
        return stored ? JSON.parse(stored) : [];
    }
    
    saveReadIds(ids: string[]) {
        localStorage.setItem(this.READ_KEY, JSON.stringify(ids));
    }

    fetchNotifications(): Observable<ApiResponse<NotificationItem[]>> {
        return this.http.get<ApiResponse<NotificationItem[]>>(this.apiUrl).pipe(
            tap(res => {
                if (res.success && res.data) {
                    const readIds = this.getReadIds();
                    const notifs = res.data.map(n => ({
                        ...n,
                        isRead: readIds.includes(n.id)
                    }));
                    this.notifications.set(notifs);
                    this.unreadCount.set(notifs.filter(n => !n.isRead).length);
                }
            })
        );
    }
    
    markAsRead(id: string) {
        const readIds = this.getReadIds();
        if (!readIds.includes(id)) {
            readIds.push(id);
            this.saveReadIds(readIds);
            
            this.notifications.update(list => list.map(n => n.id === id ? { ...n, isRead: true } : n));
            this.unreadCount.update(c => Math.max(0, c - 1));
        }
    }
    
    markAllAsRead() {
        const allIds = this.notifications().map(n => n.id);
        const readIds = [...new Set([...this.getReadIds(), ...allIds])];
        this.saveReadIds(readIds);
        
        this.notifications.update(list => list.map(n => ({ ...n, isRead: true })));
        this.unreadCount.set(0);
    }
}
