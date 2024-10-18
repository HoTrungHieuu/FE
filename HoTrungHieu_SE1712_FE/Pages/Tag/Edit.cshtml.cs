using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

using DataAccessObject.Models;
using BussinessObject.ViewModel;
using Service.Service;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Headers;
using BussinessObject.UpdateModel;

namespace FE.Pages.Tag
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public TagView Tag { get; set; } = new TagView(); 

        [BindProperty]
        public TagUpdate tagUpdate { get; set; } = new TagUpdate();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"https://localhost:7257/api/Tag/ViewDetail?TagId={id}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ServiceResult>();
                if (result != null && result.Status == 200)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    Tag = JsonSerializer.Deserialize<TagView>(result.Data.ToString(), options);
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
            tagUpdate.Id = Tag.TagId;
            tagUpdate.Name = Tag.TagName;
            tagUpdate.Note = Tag.Note;
            var updateResponse = await _httpClient.PutAsJsonAsync("https://localhost:7257/api/Tag/Update", tagUpdate);

            if (updateResponse.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, "An error occurred while updating the news article.");
            return Page();
        }

    }
}
