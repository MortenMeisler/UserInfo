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

        public void OnGet()
        {

        }
    }
}