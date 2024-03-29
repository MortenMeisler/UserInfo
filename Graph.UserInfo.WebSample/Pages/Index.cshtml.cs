﻿using Microsoft.AspNetCore.Mvc.RazorPages;
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

            var listofObjectIds = new string[]
            {   "03c0532c-3a9b-4875-8c15-5930e0394eb6",
            "03c0532c-3a9b-4875-8c15-5930e0394eb6",
            "03c0532c-3a9b-4875-8c15-5930e0394eb6",
            "03c0532c-3a9b-4875-8c15-5930e0394eb6",
            "03c0532c-3a9b-4875-8c15-5930e0394eb6",
            "03c0532c-3a9b-4875-8c15-5930e0394eb6",
            "03c0532c-3a9b-4875-8c15-5930e0394eb6",
            "03c0532c-3a9b-4875-8c15-5930e0394eb6",
            "03c0532c-3a9b-4875-8c15-5930e0394eb0",
            "03c0532c-3a9b-4875-8c15-5930e0394eb1",
            "03c0532c-3a9b-4875-8c15-5930e0394eb2",
            "03c0532c-3a9b-4875-8c15-5930e0394eb3",
            "03c0532c-3a9b-4875-8c15-5930e0394eb4",
            "03c0532c-3a9b-4875-8c15-5930e0394eb5",
            "03c0532c-3a9b-4875-8c15-5930e0394eb6",
            "03c0532c-3a9b-4875-8c15-5930e0394eb7",
            "03c0532c-3a9b-4875-8c15-5930e0394eb8",
            "03c0532c-3a9b-4875-8c15-5930e0394eb9",
                "6348e806-7813-4c31-a9e4-638bfc0e5a7a"
            };
            
            ViewData["users"] = await _userService.GetUserNamesByObjectIds(listofObjectIds);
        }
    }
}