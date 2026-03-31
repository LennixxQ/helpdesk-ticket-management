using AutoMapper;
using HelpDesk.Application.Commands.CategoryCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Application.Validators;
using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Services
{
    public class CategoryService :ICategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<BaseResponse<CategoryDto>> CreateAsync(CreateCategoryCommand command)
        {
            var validator = new CreateCategoryValidator();
            var result = await validator.ValidateAsync(command);
            if (!result.IsValid)
                return BaseResponse<CategoryDto>.Fail(result.Errors.Select(e => e.ErrorMessage).ToList());

            var existing = await _uow.Categories.GetByNameAsync(command.Name);
            if (existing is not null)
                return BaseResponse<CategoryDto>.Fail("Category already exists.");

            var category = _mapper.Map<Category>(command);
            category.CreatedAt = DateTime.UtcNow;

            await _uow.Categories.AddAsync(category);
            await _uow.SaveChangesAsync();

            return BaseResponse<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category), "Category created.");

        }

        public async Task<BaseResponse<List<CategoryDto>>> GetAllAsync()
        {
            var categories = await _uow.Categories.GetAllAsync();
            return BaseResponse<List<CategoryDto>>.Ok(_mapper.Map<List<CategoryDto>>(categories));
        }

        public async Task<BaseResponse<CategoryDto>> ToggleActiveAsync(Guid id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category is null)
                return BaseResponse<CategoryDto>.Fail("Category not found.");

            category.IsActive = !category.IsActive;
            category.LastModifiedAt = DateTime.UtcNow;

            _uow.Categories.Update(category);
            await _uow.SaveChangesAsync();

            return BaseResponse<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category), "Category updated.");

        }
    }
}
