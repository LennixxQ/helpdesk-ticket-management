using AutoMapper;
using HelpDesk.Application.DTOs.Category;
using HelpDesk.Application.DTOs.Comment;
using HelpDesk.Application.DTOs.Csat;
using HelpDesk.Application.DTOs.Department;
using HelpDesk.Application.DTOs.Escalation;
using HelpDesk.Application.DTOs.KbArticle;
using HelpDesk.Application.DTOs.RecurringTemplate;
using HelpDesk.Application.DTOs.SystemSetting;
using HelpDesk.Application.DTOs.Ticket;
using HelpDesk.Application.DTOs.User;
using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.DepartmentName, o => o.MapFrom(s =>
                s.Department != null ? s.Department.Name : null));

        CreateMap<Category, CategoryDto>();

        CreateMap<Comment, CommentDto>()
            .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.User.FullName));

        CreateMap<Ticket, TicketDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department != null ? s.Department.Name : null))
            .ForMember(d => d.Escalation, o => o.MapFrom(s => s.EscalationRecord));

        CreateMap<Ticket, CreateTicketResponseDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Message, o => o.MapFrom(s => "Ticket created successfully."));

        CreateMap<Department, DepartmentDto>()
            .ForMember(d => d.DepartmentHeadName, o => o.MapFrom(s =>
                s.DepartmentHead != null ? s.DepartmentHead.FullName : null));

        CreateMap<KbArticle, KbArticleDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Author.FullName));

        CreateMap<KbArticle, KbArticleSummaryDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));

        CreateMap<EscalationRecord, EscalationDto>()
            .ForMember(d => d.EscalatedByName, o => o.MapFrom(s => s.EscalatedBy))
            .ForMember(d => d.IsAcknowledged, o => o.MapFrom(s => s.AcknowledgedAt.HasValue));

        CreateMap<CsatResponse, CsatResponseDto>();

        CreateMap<RecurringTemplate, RecurringTemplateDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));

        CreateMap<SystemSetting, SystemSettingDto>();
    }
}