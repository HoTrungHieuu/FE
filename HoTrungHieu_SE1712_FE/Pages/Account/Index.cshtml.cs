using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccessObject.Models;
using BussinessObject.ViewModel;
using DataAccessObject.Models;
using Service.Service;
using System.Text.Json;

namespace FE.Pages.Account
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public List<SystemAccount> SystemAccount { get; set; } = new List<SystemAccount>();

        public async Task OnGetAsync()
        {
            var response = await _httpClient.GetAsync("https://localhost:7257/api/Account/ViewAll");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ServiceResult>();
                if (result != null && result.Status == 200)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    SystemAccount = JsonSerializer.Deserialize<List<SystemAccount>>(result.Data.ToString(), options);
                }
            }
        }
    }
}
