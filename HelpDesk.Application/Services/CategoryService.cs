using AutoMapper;
using HelpDesk.Application.Commands.CategoryCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Category;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Application.Validators;
using HelpDesk.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace HelpDesk.Application.Services
{
    public class CategoryService :ICategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly CreateCategoryValidator _validator;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "Categories_List";

        public CategoryService(IUnitOfWork uow, IMapper mapper, IMemoryCache cache)
        {
            _uow = uow;
            _mapper = mapper;
            _validator = new CreateCategoryValidator();
            _cache = cache;
        }

        public async Task<BaseResponse<CategoryDto>> CreateAsync(CreateCategoryCommand command)
        {
            var validation = await _validator.ValidateAsync(command);
            if (!validation.IsValid)
                return BaseResponse<CategoryDto>.Fail("Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var existing = await _uow.Categories.GetByNameAsync(command.Name);
            if (existing is not null)
                return BaseResponse<CategoryDto>.Fail("Category already exists.");

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Description = command.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system"
            };
            await _uow.Categories.AddAsync(category);
            await _uow.SaveChangesAsync();
            _cache.Remove(CacheKey);

            return BaseResponse<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category), "Category created.");
        }

        public async Task<BaseResponse<CategoryDto>> UpdateAsync(UpdateCategoryCommand command)
        {
            var category = await _uow.Categories.GetByIdAsync(command.Id);
            if (category is null) return BaseResponse<CategoryDto>.Fail("Category not found.");

            category.Name = command.Name;
            category.Description = command.Description;
            category.LastModifiedAt = DateTime.UtcNow;
            _uow.Categories.Update(category);
            await _uow.SaveChangesAsync();
            _cache.Remove(CacheKey);

            return BaseResponse<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category), "Category updated.");
        }

        public async Task<BaseResponse<List<CategoryDto>>> GetAllAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out List<CategoryDto>? cachedCats) || cachedCats == null)
            {
                var cats = await _uow.Categories.GetAllAsync();
                cachedCats = _mapper.Map<List<CategoryDto>>(cats);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(CacheKey, cachedCats, cacheOptions);
            }

            return BaseResponse<List<CategoryDto>>.Ok(cachedCats);
        }

        public async Task<BaseResponse<CategoryDto>> ToggleActiveAsync(Guid id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category is null) return BaseResponse<CategoryDto>.Fail("Category not found.");

            category.IsActive = !category.IsActive;
            category.LastModifiedAt = DateTime.UtcNow;
            _uow.Categories.Update(category);
            await _uow.SaveChangesAsync();
            _cache.Remove(CacheKey);

            return BaseResponse<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category),
                category.IsActive ? "Activated." : "Deactivated.");
        }
    }
}
