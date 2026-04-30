using HelpDesk.Application.Commands.CategoryCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Category;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<BaseResponse<CategoryDto>> CreateAsync(CreateCategoryCommand command);
        Task<BaseResponse<CategoryDto>> UpdateAsync(UpdateCategoryCommand command);
        Task<BaseResponse<List<CategoryDto>>> GetAllAsync();
        Task<BaseResponse<CategoryDto>> ToggleActiveAsync(Guid id);
    }

}
