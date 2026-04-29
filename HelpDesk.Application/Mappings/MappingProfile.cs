using AutoMapper;
using HelpDesk.Application.Commands.CategoryCommand;
using HelpDesk.Application.Commands.TicketCommand;
using HelpDesk.Application.Commands.UserCommand;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.DTOs.Department;
using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateTicketCommand, Ticket>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedAgentId, opt => opt.Ignore())
            .ForMember(dest => dest.RaisedByUserId, opt => opt.Ignore());

        CreateMap<CreateUserCommand, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        CreateMap<CreateCategoryCommand, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

        CreateMap<Ticket, TicketDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department != null ? s.Department.Name : null))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.RaisedByUserName, opt => opt.MapFrom(src => src.RaisedByUser != null ? src.RaisedByUser.FullName : string.Empty))
            .ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments))
            .ForMember(dest => dest.AssignedAgentName, opt => opt.MapFrom(src => src.AssignedAgent != null ? src.AssignedAgent.FullName : null));

        CreateMap<Ticket, CreateTicketResponseDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Message, o => o.MapFrom(s => "Ticket created successfully."));

        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.PostedByUserName,opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty));

        CreateMap<Category, CategoryDto>();

        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.DepartmentHeadName, opt => opt.MapFrom(src => src.DepartmentHead != null ? src.DepartmentHead.FullName : string.Empty));

        CreateMap<User, UserDto>()
            .ForMember(d => d.DepartmentName, o => o.MapFrom(s =>s.Department != null ? s.Department.Name : null))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        CreateMap<AuditLog, AuditLogDto>();
        CreateMap<AuditLogDetail, AuditLogDetailDto>();
    }
}