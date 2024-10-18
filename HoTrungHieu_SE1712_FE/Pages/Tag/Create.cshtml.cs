using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataAccessObject.Models;
using BussinessObject.AddModel;
using BussinessObject.ViewModel;
using Service.Service;
using System.Text.Json;
using System.Net.Http.Headers;

namespace FE.Pages.Tag
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public TagAdd Tag { get; set; } = new TagAdd();
        public List<TagView> TagVIew { get; set; } = new List<TagView>();
        public async Task OnGetAsync()
        {
            await LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
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

                    TagVIew = JsonSerializer.Deserialize<List<TagView>>(tagResult.Data.ToString(), options);
                }
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var jsonContent = JsonSerializer.Serialize(Tag);
            Console.WriteLine(jsonContent);
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7257/api/Tag/Add", Tag);

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
