using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataAccessObject.Models;
using System.Text.Json;
using BussinessObject.AddModel;
using BussinessObject.ViewModel;
using Service.Service;
using System.Net.Http.Headers;

namespace FE.Pages.Category
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public CategoryAdd Category { get; set; } = new CategoryAdd();
        public List<CategoryView> CategoryViews { get; set; } = new List<CategoryView>();
        public async Task OnGetAsync()
        {
            await LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
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
        }
        public async Task<IActionResult> OnPostAsync()
        {

            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var jsonContent = JsonSerializer.Serialize(Category);
            Console.WriteLine(jsonContent);

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7257/api/Category/Add", Category);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Có lỗi xảy ra: {errorMessage}");

            return Page();
        }
    }
}
