using AutoMapper;
using HelpDesk.Application.Commands.CsatCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Csat;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Application.Validators;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Services
{
    public class CsatService : ICsatService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly SubmitCsatValidator _validator;

        public CsatService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
            _validator = new SubmitCsatValidator();
        }

        public async Task<BaseResponse<object>> SubmitAsync(SubmitCsatCommand command, Guid currentUserId)
        {
            var validation = await _validator.ValidateAsync(command);
            if (!validation.IsValid)
                return BaseResponse<object>.Fail("Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(command.TicketId);
            if (ticket is null) return BaseResponse<object>.Fail("Ticket not found.");
            if (ticket.Status != TicketStatus.Closed)
                return BaseResponse<object>.Fail("Survey only for closed tickets.");
            if (ticket.RaisedByUserId != currentUserId)
                return BaseResponse<object>.Fail("You can only submit surveys for your own tickets.");
            if (ticket.LastModifiedAt.HasValue && ticket.LastModifiedAt.Value.AddDays(7) < DateTime.UtcNow)
                return BaseResponse<object>.Fail("Survey link has expired (7 days).");
            if (await _uow.Csat.ExistsForTicketAsync(command.TicketId))
                return BaseResponse<object>.Fail("Survey already submitted for this ticket.");

            var response = new CsatResponse
            {
                Id = Guid.NewGuid(),
                TicketId = command.TicketId,
                RespondentId = currentUserId,
                ClosingAgentId = ticket.AssignedAgentId ?? Guid.Empty,
                Score = command.Score,
                Comments = command.Comments,
                SubmittedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId.ToString()
            };
            await _uow.Csat.AddAsync(response);
            await _uow.SaveChangesAsync();

            return BaseResponse<object>.Ok(new object(), "Survey submitted. Thank you!");
        }

        public async Task<BaseResponse<AgentCsatStatsDto>> GetAgentStatsAsync(
            Guid agentId, DateTime from, DateTime to)
        {
            var agent = await _uow.Users.GetByIdAsync(agentId);
            if (agent is null) return BaseResponse<AgentCsatStatsDto>.Fail("Agent not found.");

            var avgScore = await _uow.Csat.GetAverageScoreForAgentAsync(agentId, from, to);
            var responses = await _uow.Csat.GetByAgentIdAsync(agentId);

            return BaseResponse<AgentCsatStatsDto>.Ok(new AgentCsatStatsDto
            {
                AgentId = agentId,
                AgentName = agent.FullName,
                AverageScore = avgScore,
                ResponseCount = responses.Count()
            });
        }
    }
}
