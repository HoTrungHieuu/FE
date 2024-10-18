using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json; // Ð?ng quên thêm using này ð? s? d?ng PostAsJsonAsync
using System.Text.Json;
using Service.Service;
using BussinessObject.ViewModel;

namespace FE.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginModel(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _httpClient.BaseAddress = new Uri("https://localhost:7257/");
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }
        public void OnGetAsync()
        {
            _httpContextAccessor.HttpContext.Session.Remove("AccountId");
            _httpContextAccessor.HttpContext.Session.Remove("Token");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var url = $"api/Account/Login?email={Email}&password={Password}";

            var loginData = new
            {
                email = Email,
                password = Password
            };

            var response = await _httpClient.PostAsJsonAsync(url, loginData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ServiceResult>();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                AccountView data = JsonSerializer.Deserialize<AccountView>(result.Data.ToString(), options);

                if (data!=null)
                {
                    _httpContextAccessor.HttpContext.Session.SetString("AccountId", data.AccountId.ToString());
                    _httpContextAccessor.HttpContext.Session.SetString("Token", data.Token);
                }

                return RedirectToPage("/NewArtical");
            }
            else
            {
                ErrorMessage = "Login Fail!";
                return Page();
            }
        }
    }
}
