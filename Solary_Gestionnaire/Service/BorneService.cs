using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Solary_Gestionnaire.Model;

namespace Solary_Gestionnaire.Service
{
    public class BorneService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://solary.vabre.ch";

        public BorneService()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(15)
            };

            // En-têtes de base
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        // Méthode pour récupérer toutes les bornes
        public async Task<List<Borne>> GetAllBornesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/GetAllBornes");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Borne>>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erreur API: {response.StatusCode} - {errorContent}");
                    throw new Exception($"Erreur API: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception dans GetAllBornesAsync: {ex.Message}");
                throw;
            }
        }

        // Méthode pour supprimer une borne
        public async Task<bool> DeleteBorneAsync(int borneId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/DeleteBorne/{borneId}");

                // Lire et afficher la réponse
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Réponse de suppression: {response.StatusCode} - {responseContent}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception dans DeleteBorneAsync: {ex.Message}");
                throw;
            }
        }

        // Méthode pour ajouter une borne
        public async Task<bool> AddBorneAsync(Borne borne)
        {
            try
            {
                // Créer un objet avec TOUS les champs requis selon la documentation
                var borneData = new
                {
                    name = borne.name,
                    address = borne.address,
                    city = borne.city,
                    postal_code = borne.postal_code,
                    latitude = borne.latitude ?? "0.0", // Valeur par défaut si null
                    longitude = borne.longitude ?? "0.0", // Valeur par défaut si null
                    power_output = borne.power_output,
                    charge_percentage = borne.charge_percentage,
                    status = borne.status,
                    is_in_maintenance = borne.is_in_maintenance
                };

                // Sérialiser en JSON
                var json = JsonSerializer.Serialize(borneData);
                Console.WriteLine($"Données envoyées: {json}");

                // Créer le contenu de la requête
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Envoyer la requête
                var response = await _httpClient.PostAsync($"{BaseUrl}/AddBorne", content);

                // Lire et afficher la réponse
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Réponse: {response.StatusCode} - {responseContent}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception dans AddBorneAsync: {ex.Message}");
                throw;
            }
        }
    }
}