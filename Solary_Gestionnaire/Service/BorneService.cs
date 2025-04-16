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
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<List<Borne>> GetAllBornesAsync()
        {
            try
            {
                var url = "https://solary.vabre.ch/GetAllBornes";
                return await _httpClient.GetFromJsonAsync<List<Borne>>(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur API : " + ex.Message);
                throw; // On relance l’erreur pour la capturer dans le ViewModel
            }
        }

    }
}