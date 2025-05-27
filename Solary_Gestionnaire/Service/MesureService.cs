using Solary_Gestionnaire.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace Solary_Gestionnaire.Service
{
    public class MesureService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://solary.vabre.ch";

        public MesureService()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(15)
            };

            // En-têtes de base
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "SolaryGestionnaire/1.0");
        }

        // Méthode pour récupérer toutes les mesures d'énergie
        public async Task<List<MesureEnergie>> GetAllMesuresEnergieAsync()
        {
            try
            {
                Console.WriteLine($"=== Appel API: {BaseUrl}/GetAllMesuresEnergie ===");

                var response = await _httpClient.GetAsync($"{BaseUrl}/GetAllMesuresEnergie");

                Console.WriteLine($"Status Code: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"JSON reçu (premiers 200 caractères): {jsonContent.Substring(0, Math.Min(200, jsonContent.Length))}...");

                    var result = await response.Content.ReadFromJsonAsync<List<MesureEnergie>>();
                    Console.WriteLine($"Désérialisation réussie: {result?.Count ?? 0} mesures d'énergie");

                    // Afficher quelques exemples de données
                    if (result != null && result.Count > 0)
                    {
                        var firstMesure = result[0];
                        Console.WriteLine($"Première mesure - ID: {firstMesure.mesure_id}, Borne: {firstMesure.borne_id}, Date string: {firstMesure.measure_date_string}, Date convertie: {firstMesure.measure_date}");
                    }

                    return result ?? new List<MesureEnergie>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erreur API: {response.StatusCode} - {errorContent}");
                    throw new Exception($"Erreur API: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception dans GetAllMesuresEnergieAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        // Méthode pour récupérer les mesures d'énergie d'une borne spécifique
        public async Task<List<MesureEnergie>> GetMesuresEnergieByBorneIdAsync(int borneId)
        {
            try
            {
                Console.WriteLine($"=== Récupération mesures pour borne {borneId} ===");
                var allMesures = await GetAllMesuresEnergieAsync();
                var filteredMesures = allMesures.Where(m => m.borne_id == borneId).ToList();
                Console.WriteLine($"Mesures filtrées pour borne {borneId}: {filteredMesures.Count}");
                return filteredMesures;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception dans GetMesuresEnergieByBorneIdAsync: {ex.Message}");
                throw;
            }
        }
    }
}
