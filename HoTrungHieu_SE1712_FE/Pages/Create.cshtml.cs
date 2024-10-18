using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObject.ViewModel;
using System.Text.Json;
using Service.Service;
using BussinessObject.AddModel;
using Microsoft.Extensions.Options;
using DataAccessObject.Models;
using System.Net.Http.Headers;

namespace FE.Pages
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public NewsArticleAdd NewsArticle { get; set; } = new NewsArticleAdd();

        public List<TagView> Tags { get; set; } = new List<TagView>();
        public List<CategoryView> Categories { get; set; } = new List<CategoryView>();

        public async Task OnGetAsync()
        {
            await LoadTagsAndCategoriesAsync();
        }

        private async Task LoadTagsAndCategoriesAsync()
        {
            var tagResponse = await _httpClient.GetAsync("https://localhost:7257/api/Tag/ViewAll");
            if (tagResponse.IsSuccessStatusCode)
            {
                var tagResult = await tagResponse.Content.ReadFromJsonAsync<ServiceResult>();
                if (tagResult != null && tagResult.Status == 200)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    Tags = JsonSerializer.Deserialize<List<TagView>>(tagResult.Data.ToString(), options);
                }
            }

            var categoryResponse = await _httpClient.GetAsync("https://localhost:7257/api/Category/ViewAll");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var categoryResult = await categoryResponse.Content.ReadFromJsonAsync<ServiceResult>();
                if (categoryResult != null && categoryResult.Status == 200)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    Categories = JsonSerializer.Deserialize<List<CategoryView>>(categoryResult.Data.ToString(), options);
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadTagsAndCategoriesAsync();
                return Page();
            }
            var token = HttpContext.Session.GetString("Token");
            NewsArticle.AccountId = short.Parse(HttpContext.Session.GetString("AccountId"));
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            // In dữ liệu để kiểm tra
            var jsonContent = JsonSerializer.Serialize(NewsArticle);
            Console.WriteLine(jsonContent);
            if (Tags == null) Tags = new();
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7257/api/NewsArticle/Add", NewsArticle);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/NewArtical");
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Có lỗi xảy ra: {errorMessage}");

            await LoadTagsAndCategoriesAsync();
            return Page();
        }

    }
}
