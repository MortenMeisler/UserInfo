using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserInfo.Library.Services;

namespace UserInfo.WebSample.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IUserInfoService _userService;

        public IndexModel(ILogger<IndexModel> logger, IUserInfoService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task OnGetAsync()
        {
            // Arya Stark
            ViewData["username"] = await _userService.GetUserNameByObjectId("03c0532c-3a9b-4875-8c15-5930e0394eb6");
            
            ViewData["user"] = await _userService.GetUserByObjectId("03c0532c-3a9b-4875-8c15-5930e0394eb6");
        }
    }
}