using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccessObject.Models;
using BussinessObject.ViewModel;
using System.Text.Json;
using Service.Service;

namespace FE.Pages
{
    public class NewArticalModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public NewArticalModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public List<NewsArticleView> ArticleViews { get; set; } = new List<NewsArticleView>();

        [BindProperty(SupportsGet = true)]
        public string SearchTitle { get; set; } 

        public int AccountId { get; set; }

        public async Task OnGetAsync()
        {
            AccountId = Convert.ToInt32(HttpContext.Session.GetString("AccountId") ?? "0");

            var response = await _httpClient.GetAsync("https://localhost:7257/api/NewsArticle/ViewAll");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ServiceResult>();
                if (result != null && result.Status == 200)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var allArticles = JsonSerializer.Deserialize<List<NewsArticleView>>(result.Data.ToString(), options);

                    if (!string.IsNullOrEmpty(SearchTitle))
                    {
                        ArticleViews = allArticles
                            .Where(article => article.NewsTitle.Contains(SearchTitle, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                    }
                    else
                    {
                        ArticleViews = allArticles;
                    }
                }
            }
        }
    }
}
