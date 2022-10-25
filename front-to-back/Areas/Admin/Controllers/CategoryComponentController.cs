using front_to_back.Areas.Admin.ViewModels.CategoryComponent;
using front_to_back.DAL;
using front_to_back.Helpers;
using front_to_back.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace front_to_back.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryComponentController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryComponentController(AppDbContext appDbContext,
            IFileService fileService,
            IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new CategoryComponentIndexViewModel
            {
                CategoryComponents = await _appDbContext.CategoryComponents
                .Include(cc => cc.Category)
                .ToListAsync()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CategoryComponentCreateViewModel
            {
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                })
                .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryComponentCreateViewModel model)
        {
            model.Categories = await _appDbContext.Categories.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            })
            .ToListAsync();

            if (!ModelState.IsValid) return View(model);

            var category = await _appDbContext.Categories.FindAsync(model.CategoryId);
            if (category == null)
            {
                ModelState.AddModelError("CategoryId", "Bu kateqoriya mövcud deyil");
                return View(model);
            }

            bool isExist = await _appDbContext.CategoryComponents
                                .AnyAsync(cc => cc.Title.ToLower().Trim() == model.Title.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Title", "Bu adda kateqoriya komponent mövcuddur");
                return View(model);
            }

            if (!_fileService.IsImage(model.Photo))
            {
                ModelState.AddModelError("Photo", "Fayl şəkil formatında olmalıdır");
                return View(model);
            }

            var categoryComponent = new CategoryComponent
            {
                Title = model.Title,
                Description = model.Description,
                CategoryId = model.CategoryId,
                FilePath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.CategoryComponents.AddAsync(categoryComponent);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }
    }
}
