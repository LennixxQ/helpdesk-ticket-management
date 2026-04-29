using AutoMapper;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Department;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Exceptions;

namespace HelpDesk.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<BaseResponse<object>> AssignHeadAsync(Guid departmentId, Guid userId)
        {
            var department = await _uow.Departments.GetByIdAsync(departmentId) ?? throw new NotFoundException("Department", departmentId);

            var user = await _uow.Users.GetByIdAsync(userId) ?? throw new NotFoundException("User", userId);

            department.DepartmentHeadId = userId;
            _uow.Departments.Update(department);
            await _uow.SaveChangesAsync();

            return BaseResponse<object>.Ok(new object(), "Department head assigned.");
        }

        public async Task<BaseResponse<DepartmentDto>> CreateAsync(CreateDepartmentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BaseResponse<DepartmentDto>.Fail("Department name is required.");

            if (await _uow.Departments.NameExistsAsync(dto.Name))
                return BaseResponse<DepartmentDto>.Fail("A department with this name already exists.");

            var department = new Department
            {
                Name = dto.Name,
                DepartmentHeadId = dto.DepartmentHeadId,
                IsActive = true
            };

            if (dto.DepartmentHeadId.HasValue)
            {
                department.DepartmentHead = await _uow.Users.GetByIdAsync(dto.DepartmentHeadId.Value);
            }

            await _uow.Departments.AddAsync(department);
            await _uow.SaveChangesAsync();

            return BaseResponse<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(department), "Department created.");
        }

        public async Task<BaseResponse<object>> DeactivateAsync(Guid id)
        {
            var department = await _uow.Departments.GetByIdAsync(id)
            ?? throw new NotFoundException("Department", id);

            if (department.Name == "General")
                return BaseResponse<object>.Fail("The 'General' department cannot be deactivated.");

            var hasActiveUsers = department.Members.Any(u => u.IsActive);
            if (hasActiveUsers)
                return BaseResponse<object>.Fail("Cannot deactivate a department with active users.");

            department.IsActive = false;
            department.LastModifiedAt = DateTime.UtcNow;
            _uow.Departments.Update(department);
            await _uow.SaveChangesAsync();

            return BaseResponse<object>.Ok(new object(), "Department deactivated.");
        }

        public async Task<BaseResponse<List<DepartmentDto>>> GetAllAsync()
        {
            var departments = await _uow.Departments.GetAllAsync();
            return BaseResponse<List<DepartmentDto>>.Ok(_mapper.Map<List<DepartmentDto>>(departments));
        }

        public async Task<BaseResponse<DepartmentDto>> GetByIdAsync(Guid id)
        {
            var department = await _uow.Departments.GetByIdAsync(id) ?? throw new NotFoundException("Department", id);

            return BaseResponse<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(department));
        }

        public async Task<BaseResponse<DepartmentSummaryDto>> GetSummaryAsync(Guid id)
        {
            var department = await _uow.Departments.GetByIdAsync(id) ?? throw new NotFoundException("Department", id);

            var summary = await _uow.Departments.GetSummaryAsync(id);

            return BaseResponse<DepartmentSummaryDto>.Ok(new DepartmentSummaryDto
            {
                Id = id,
                Name = department.Name,
                ActiveUserCount = summary.ActiveUserCount,
                OpenTicketCount = summary.OpenTicketCount,
                TicketsLast30Days = summary.TicketsLast30Days
            });
        }

        public async Task<BaseResponse<DepartmentDto>> UpdateAsync(Guid id, UpdateDepartmentDto dto)
        {
            var department = await _uow.Departments.GetByIdAsync(id)
            ?? throw new NotFoundException("Department", id);

            department.Name = dto.Name;
            
            if (department.DepartmentHeadId != dto.DepartmentHeadId)
            {
                department.DepartmentHeadId = dto.DepartmentHeadId;
                if (dto.DepartmentHeadId.HasValue)
                {
                    department.DepartmentHead = await _uow.Users.GetByIdAsync(dto.DepartmentHeadId.Value);
                }
                else
                {
                    department.DepartmentHead = null;
                }
            }
            _uow.Departments.Update(department);
            await _uow.SaveChangesAsync();

            return BaseResponse<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(department), "Department updated.");
        }
    }
}
