using HelpDesk.Application.Commands.CategoryCommand;
using HelpDesk.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
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
            _logger.LogInformation("Admin creating category: {Name}", command.Name);
            var response = await _categoryService.CreateAsync(command);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("ActivateCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var response = await _categoryService.ToggleActiveAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}
