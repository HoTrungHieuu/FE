using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

using DataAccessObject.Models;
using BussinessObject.AddModel;
using BussinessObject.UpdateModel;
using BussinessObject.ViewModel;
using Service.Service;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FE.Pages.Category
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public CategoryView Category { get; set; } = new CategoryView();
        [BindProperty]
        public List<CategoryView> CategoryViews { get; set; } = new List<CategoryView>();

        [BindProperty]
        public CategoryUpdate CategoryUpdate { get; set; } = new CategoryUpdate();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var tagResponse = await _httpClient.GetAsync("https://localhost:7257/api/Category/ViewAll");
            if (tagResponse.IsSuccessStatusCode)
            {
                var tagResult = await tagResponse.Content.ReadFromJsonAsync<ServiceResult>();
                if (tagResult != null && tagResult.Status == 200)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    CategoryViews = JsonSerializer.Deserialize<List<CategoryView>>(tagResult.Data.ToString(), options);
                }
            }
            var response = await _httpClient.GetAsync($"https://localhost:7257/api/Category/ViewDetail?CategoryId={id}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ServiceResult>();
                if (result != null && result.Status == 200)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    Category = JsonSerializer.Deserialize<CategoryView>(result.Data.ToString(), options);
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            CategoryUpdate.Id = Category.CategoryId;
            CategoryUpdate.CategoryName = Category.CategoryName;
            CategoryUpdate.CategoryDesciption = Category.CategoryDesciption;
            CategoryUpdate.ParentCategoryId = Category.ParentCategoryId;
            CategoryUpdate.IsActive = Category.IsActive;

            var updateResponse = await _httpClient.PutAsJsonAsync("https://localhost:7257/api/Category/Update", CategoryUpdate);

            if (updateResponse.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, "An error occurred while updating the news article.");
            return Page();
        }
    }
}
