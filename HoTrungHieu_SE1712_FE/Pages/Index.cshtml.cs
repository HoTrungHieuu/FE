﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DataAccessObject.Models;
using BussinessObject.ViewModel;
using Service.Service;
using System.Text.Json;

namespace FE.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public List<NewsArticleView> ArticleViews { get; set; } = new List<NewsArticleView>();

        public async Task OnGetAsync()
        {
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

                    ArticleViews = JsonSerializer.Deserialize<List<NewsArticleView>>(result.Data.ToString(), options);
                }
            }
        }
    }
}
