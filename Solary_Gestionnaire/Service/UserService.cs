using Solary_Gestionnaire.Model;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace Solary_Gestionnaire.Service
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://solary.vabre.ch";

        public UserService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<List<User>> GetUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/GetAllUsers");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    options.Converters.Add(new UserJsonConverter());
                    return JsonSerializer.Deserialize<List<User>>(json, options);
                }

                return new List<User>();
            }
            catch
            {
                return new List<User>();
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                // Utiliser l'ID utilisateur dans l'URL
                string url = $"{BaseUrl}/DeleteUser/{userId}";
                var response = await _httpClient.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(bool Success, string ErrorMessage)> AddUserAsync(User user, string password)
        {
            try
            {
                var userData = new
                {
                    email = user.Email,
                    password = password,
                    role = user.Role,
                    status_compte = user.StatusCompte,
                    compte_verifie = user.CompteVerifie,
                    otp_code = user.OtpCode,
                    otp_created_at = user.OtpCreatedAt
                };

                var json = JsonSerializer.Serialize(userData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{BaseUrl}/AddUser", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, string.Empty);
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return (false, $"Erreur HTTP {(int)response.StatusCode}: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateUserAsync(User user, string newPassword = null)
        {
            try
            {
                var userData = new
                {
                    user_id = user.UserId,
                    email = user.Email,
                    role = user.Role,
                    status_compte = user.StatusCompte,
                    compte_verifie = user.CompteVerifie,
                    otp_code = user.OtpCode,
                    password = newPassword // Inclure le nouveau mot de passe seulement s'il est fourni
                };

                var json = JsonSerializer.Serialize(userData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{BaseUrl}/UpdateUser/{user.UserId}", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, string.Empty);
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return (false, $"Erreur HTTP {(int)response.StatusCode}: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }

    public class UserJsonConverter : System.Text.Json.Serialization.JsonConverter<User>
    {
        public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var user = new User();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return user;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException();

                string propertyName = reader.GetString();
                reader.Read();

                switch (propertyName.ToLower())
                {
                    case "user_id":
                        user.UserId = reader.TokenType == JsonTokenType.Null ? 0 : reader.GetInt32();
                        break;
                    case "email":
                        user.Email = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                        break;
                    case "role":
                        user.Role = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                        break;
                    case "status_compte":
                        user.StatusCompte = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                        break;
                    case "date_creation":
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            string dateStr = reader.GetString();
                            if (DateTime.TryParse(dateStr, out DateTime date))
                                user.DateCreation = date;
                        }
                        break;
                    case "password_hash":
                        user.PasswordHash = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                        break;
                    case "dernier_login":
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            string dateStr = reader.GetString();
                            if (DateTime.TryParse(dateStr, out DateTime date))
                                user.DernierLogin = date;
                        }
                        break;
                    case "otp_code":
                        user.OtpCode = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                        break;
                    case "compte_verifie":
                        user.CompteVerifie = reader.TokenType == JsonTokenType.Null ? 0 : reader.GetInt32();
                        break;
                    case "otp_created_at":
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            string dateStr = reader.GetString();
                            if (DateTime.TryParse(dateStr, out DateTime date))
                                user.OtpCreatedAt = date;
                        }
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
