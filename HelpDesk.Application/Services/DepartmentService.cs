using AutoMapper;
using HelpDesk.Application.Commands.DepartmentCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Department;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Application.Validators;
using HelpDesk.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace HelpDesk.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly CreateDepartmentValidator _validator;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "Departments_List";

        public DepartmentService(IUnitOfWork uow, IMapper mapper, IMemoryCache cache)
        {
            _uow = uow;
            _mapper = mapper;
            _validator = new CreateDepartmentValidator();
            _cache = cache;
        }

        public async Task<BaseResponse<List<DepartmentDto>>> GetAllAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out List<DepartmentDto>? cachedDepts) || cachedDepts == null)
            {
                var depts = await _uow.Departments.GetAllWithIncludesAsync();
                cachedDepts = _mapper.Map<List<DepartmentDto>>(depts);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(CacheKey, cachedDepts, cacheOptions);
            }

            return BaseResponse<List<DepartmentDto>>.Ok(cachedDepts);
        }

        public async Task<BaseResponse<DepartmentDto>> GetByIdAsync(Guid id)
        {
            var dept = await _uow.Departments.GetByIdAsync(id);
            if (dept is null) return BaseResponse<DepartmentDto>.Fail("Department not found.");
            return BaseResponse<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(dept));
        }

        public async Task<BaseResponse<DepartmentSummaryDto>> GetSummaryAsync(Guid id)
        {
            var dept = await _uow.Departments.GetByIdAsync(id);
            if (dept is null) return BaseResponse<DepartmentSummaryDto>.Fail("Department not found.");

            var (activeUsers, openTickets, last30) = await _uow.Departments.GetSummaryAsync(id);
            return BaseResponse<DepartmentSummaryDto>.Ok(new DepartmentSummaryDto
            {
                Id = id,
                Name = dept.Name,
                ActiveUserCount = activeUsers,
                OpenTicketCount = openTickets,
                TicketsLast30Days = last30
            });
        }

        public async Task<BaseResponse<DepartmentDto>> CreateAsync(CreateDepartmentCommand command)
        {
            var validation = await _validator.ValidateAsync(command);
            if (!validation.IsValid)
                return BaseResponse<DepartmentDto>.Fail("Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var existing = await _uow.Departments.GetByNameAsync(command.Name);
            if (existing is not null)
                return BaseResponse<DepartmentDto>.Fail("Department already exists.");

            var dept = new Department
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                DepartmentHeadId = command.DepartmentHeadId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system"
            };
            await _uow.Departments.AddAsync(dept);
            await _uow.SaveChangesAsync();
            _cache.Remove(CacheKey);

            return BaseResponse<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(dept), "Department created.");
        }

        public async Task<BaseResponse<DepartmentDto>> UpdateAsync(UpdateDepartmentCommand command)
        {
            var dept = await _uow.Departments.GetByIdAsync(command.Id);
            if (dept is null) return BaseResponse<DepartmentDto>.Fail("Department not found.");

            dept.Name = command.Name;
            dept.DepartmentHeadId = command.DepartmentHeadId;
            dept.LastModifiedAt = DateTime.UtcNow;
            _uow.Departments.Update(dept);
            await _uow.SaveChangesAsync();
            _cache.Remove(CacheKey);

            return BaseResponse<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(dept), "Department updated.");
        }

        public async Task<BaseResponse<object>> DeactivateAsync(Guid id)
        {
            var dept = await _uow.Departments.GetByIdAsync(id);
            if (dept is null) return BaseResponse<object>.Fail("Department not found.");
            if (dept.Name == "General") return BaseResponse<object>.Fail("Cannot deactivate General department.");

            dept.IsActive = false;
            dept.LastModifiedAt = DateTime.UtcNow;
            _uow.Departments.Update(dept);
            await _uow.SaveChangesAsync();
            _cache.Remove(CacheKey);

            return BaseResponse<object>.Ok(new object(), "Department deactivated.");
        }

        public async Task<BaseResponse<object>> ActivateAsync(Guid id)
        {
            var dept = await _uow.Departments.GetByIdAsync(id);
            if (dept is null) return BaseResponse<object>.Fail("Department not found.");

            dept.IsActive = true;
            dept.LastModifiedAt = DateTime.UtcNow;
            _uow.Departments.Update(dept);
            await _uow.SaveChangesAsync();
            _cache.Remove(CacheKey);

            return BaseResponse<object>.Ok(new object(), "Department activated.");
        }

        public async Task<BaseResponse<object>> AssignHeadAsync(Guid departmentId, Guid userId)
        {
            var dept = await _uow.Departments.GetByIdAsync(departmentId);
            if (dept is null) return BaseResponse<object>.Fail("Department not found.");

            var user = await _uow.Users.GetByIdAsync(userId);
            if (user is null) return BaseResponse<object>.Fail("User not found.");

            dept.DepartmentHeadId = userId;
            dept.LastModifiedAt = DateTime.UtcNow;
            _uow.Departments.Update(dept);
            await _uow.SaveChangesAsync();
            _cache.Remove(CacheKey);

            return BaseResponse<object>.Ok(new object(), "Department head assigned.");
        }
    }
}
