using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccessObject.Models;
using BussinessObject.AddModel;
using BussinessObject.ViewModel;
using Service.Service;
using System.Text.Json;
using System.Net.Http.Headers;

namespace FE.Pages.Category
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetailsModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public CategoryView Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var token = HttpContext.Session.GetString("Token"); 

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"https://localhost:7257/api/Category/ViewDetail?categoryId={id}");

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
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }

            return Page();
        }
    }
}
