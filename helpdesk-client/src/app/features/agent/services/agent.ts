import { Injectable } from '@angular/core';
import { TicketService } from '../../../core/services/ticket.service';

// Agent uses same TicketService — this is a thin wrapper for agent-specific needs
@Injectable({ providedIn: 'root' })
export class AgentService {
  constructor(public ticketService: TicketService) { }
}