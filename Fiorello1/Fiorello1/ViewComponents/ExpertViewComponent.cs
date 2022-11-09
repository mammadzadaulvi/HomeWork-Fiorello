using Fiorello1.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello1.ViewComponents
{
    public class ExpertViewComponent : ViewComponent
    {
        private readonly AppDbContext _appDbContext;

        public ExpertViewComponent(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var experts = await _appDbContext.Experts.ToListAsync();
            return View(experts);
        }
    }
}
