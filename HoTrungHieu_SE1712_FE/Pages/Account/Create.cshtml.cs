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
using System.Net.Http.Headers;
using System.Text.Json;

namespace FE.Pages.Account
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public AccountAdd SystemAccount { get; set; } = new AccountAdd();
        public List<AccountView> Account { get; set; } = new List<AccountView>();
        public async Task OnGetAsync()
        {
            await LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            var tagResponse = await _httpClient.GetAsync("https://localhost:7257/api/Account/ViewAll");
            if (tagResponse.IsSuccessStatusCode)
            {
                var tagResult = await tagResponse.Content.ReadFromJsonAsync<ServiceResult>();
                if (tagResult != null && tagResult.Status == 200)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    Account = JsonSerializer.Deserialize<List<AccountView>>(tagResult.Data.ToString(), options);
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
            var jsonContent = JsonSerializer.Serialize(SystemAccount);
            Console.WriteLine(jsonContent);

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7257/api/Account/Create", SystemAccount);

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
