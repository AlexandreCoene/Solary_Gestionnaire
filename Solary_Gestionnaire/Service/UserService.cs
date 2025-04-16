using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Solary_Gestionnaire.Service
{
    public class UserService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<List<Model.User>> GetUsersAsync()
        {
            try
            {
                string url = "https://solary.vabre.ch/GetAllUsers";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Model.User>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    return new List<Model.User>();
                }
            }
            catch (Exception)
            {
                return new List<Model.User>();
            }
        }

        public async Task<bool> DeleteUserAsync(string email)
        {
            try
            {
                string url = $"https://solary.vabre.ch/DeleteUser?email={Uri.EscapeDataString(email)}";
                HttpResponseMessage response = await _httpClient.DeleteAsync(url);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
