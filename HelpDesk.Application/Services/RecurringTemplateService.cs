using AutoMapper;
using HelpDesk.Application.Commands.RecurringTemplateCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.RecurringTemplate;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Application.Validators;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Services
{
    public class RecurringTemplateService : IRecurringTemplateService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly CreateRecurringTemplateValidator _validator;

        public RecurringTemplateService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
            _validator = new CreateRecurringTemplateValidator();
        }

        public async Task<BaseResponse<List<RecurringTemplateDto>>> GetAllAsync()
        {
            var templates = await _uow.RecurringTemplates.GetAllAsync();
            return BaseResponse<List<RecurringTemplateDto>>.Ok(_mapper.Map<List<RecurringTemplateDto>>(templates));
        }

        public async Task<BaseResponse<RecurringTemplateDto>> GetByIdAsync(Guid id)
        {
            var template = await _uow.RecurringTemplates.GetByIdAsync(id);
            if (template is null) return BaseResponse<RecurringTemplateDto>.Fail("Template not found.");
            return BaseResponse<RecurringTemplateDto>.Ok(_mapper.Map<RecurringTemplateDto>(template));
        }

        public async Task<BaseResponse<RecurringTemplateDto>> CreateAsync(
            CreateRecurringTemplateCommand command, Guid adminId)
        {
            var validation = await _validator.ValidateAsync(command);
            if (!validation.IsValid)
                return BaseResponse<RecurringTemplateDto>.Fail("Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var template = new RecurringTemplate
            {
                Id = Guid.NewGuid(),
                TemplateName = command.TemplateName,
                TicketTitle = command.TicketTitle,
                Description = command.Description,
                Priority = command.Priority,
                CategoryId = command.CategoryId,
                RecurrencePattern = command.RecurrencePattern,
                CronExpression = command.CronExpression,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                MaxOccurrences = command.MaxOccurrences,
                AssignToAgentId = command.AssignToAgentId,
                RaiseOnBehalfOfId = command.RaiseOnBehalfOfId,
                CreatedByAdminId = adminId,
                IsActive = true,
                NextRunAt = command.StartDate,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = adminId.ToString()
            };
            await _uow.RecurringTemplates.AddAsync(template);
            await _uow.SaveChangesAsync();

            var created = await _uow.RecurringTemplates.GetByIdAsync(template.Id);
            return BaseResponse<RecurringTemplateDto>.Ok(_mapper.Map<RecurringTemplateDto>(created), "Template created.");
        }

        public async Task<BaseResponse<object>> ToggleActiveAsync(Guid id)
        {
            var template = await _uow.RecurringTemplates.GetByIdAsync(id);
            if (template is null) return BaseResponse<object>.Fail("Template not found.");
            template.IsActive = !template.IsActive;
            template.LastModifiedAt = DateTime.UtcNow;
            _uow.RecurringTemplates.Update(template);
            await _uow.SaveChangesAsync();
            return BaseResponse<object>.Ok(new object(), template.IsActive ? "Activated." : "Paused.");
        }

        public async Task<BaseResponse<object>> TriggerManualAsync(Guid id, Guid adminId)
        {
            var template = await _uow.RecurringTemplates.GetByIdAsync(id);
            if (template is null) return BaseResponse<object>.Fail("Template not found.");

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = template.TicketTitle,
                Description = template.Description,
                CategoryId = template.CategoryId,
                Priority = template.Priority,
                Status = TicketStatus.Open,
                RaisedByUserId = template.RaiseOnBehalfOfId,
                AssignedAgentId = template.AssignToAgentId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = adminId.ToString()
            };
            await _uow.Tickets.AddAsync(ticket);
            template.RunCount++;
            template.LastRunAt = DateTime.UtcNow;
            template.LastModifiedAt = DateTime.UtcNow;
            _uow.RecurringTemplates.Update(template);
            await _uow.SaveChangesAsync();

            return BaseResponse<object>.Ok(new object(), $"Template triggered. Ticket {ticket.Id} created.");
        }

        public async Task<BaseResponse<object>> DeleteAsync(Guid id)
        {
            var template = await _uow.RecurringTemplates.GetByIdAsync(id);
            if (template is null) return BaseResponse<object>.Fail("Template not found.");
            _uow.RecurringTemplates.Delete(template);
            await _uow.SaveChangesAsync();
            return BaseResponse<object>.Ok(new object(), "Template deleted.");
        }
    }
}
