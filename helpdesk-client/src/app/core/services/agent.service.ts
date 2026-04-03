import { Injectable } from '@angular/core';
import { TicketService } from './ticket.service';

@Injectable({ providedIn: 'root' })
export class AgentService {
    constructor(public ticketService: TicketService) { }
}