using HelpDesk.API.Records;
using HelpDesk.Application.Commands.CategoryCommand;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger, ICurrentUserProvider currentUserProvider) : base(currentUserProvider)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var getAllCategories = await _categoryService.GetAllAsync();
            return Ok(getAllCategories);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
        {
            _logger.LogInformation("Admin {AdminId} creating category: {Name}",_currentUserProvider.GetCurrentUserId(), command.Name);
            var response = await _categoryService.CreateAsync(command);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryCommand command)
        {
            var result = await _categoryService.UpdateAsync(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("ActivateCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleActive([FromBody] GetByIdRequest command)
        {
            var response = await _categoryService.ToggleActiveAsync(command.Id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}
