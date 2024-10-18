using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataAccessObject.Models;
using BussinessObject.UpdateModel;
using BussinessObject.ViewModel;
using Service.Service;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FE.Pages.Account
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public SystemAccount SystemAccount { get; set; } = new SystemAccount();

        [BindProperty]
        public AccountUpdate accountUpdate { get; set; } = new AccountUpdate();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"https://localhost:7257/api/Account/AccountDetail?accountId={id}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ServiceResult>();
                if (result != null && result.Status == 200)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    SystemAccount = JsonSerializer.Deserialize<SystemAccount>(result.Data.ToString(), options);
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
            accountUpdate.Id = SystemAccount.AccountId;
            accountUpdate.AccountName = SystemAccount.AccountName;
            accountUpdate.Password = SystemAccount.AccountPassword;

            var updateResponse = await _httpClient.PutAsJsonAsync("https://localhost:7257/api/Account/Update", accountUpdate);

            if (updateResponse.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, "An error occurred while updating the news article.");
            return Page();
        }
    }
}
