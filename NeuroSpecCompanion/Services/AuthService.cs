﻿using NeuroSpec.Shared.Models.DTO;
using NeuroSpecCompanion.Shared.Services.DTO_Services;
using System.Text.Json;


namespace NeuroSpecCompanion.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApi;
        public AuthService()
        {
            _httpClient = new HttpClient();
            _baseApi = "http://neurospec.runasp.net/api/Patient";
        }
        public async Task<Patient> GetPatientByIdAsync(int patientID)
        {
            var response = await _httpClient.GetAsync($"{_baseApi}/{patientID}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var patient = JsonSerializer.Deserialize<Patient>(content, options);
            return patient;
        }
        public async Task<bool> VerifyPatientCallerAsync(int patientID, string password, bool autoLogin)
        {
            PatientService patientService = new PatientService();
            var response = await _httpClient.GetAsync(_baseApi + "/" + patientID + "/" + password);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var isValid = JsonSerializer.Deserialize<bool>(content);
            if (isValid)
            {
                var patient = await GetPatientByIdAsync(patientID);

                if (patient != null)
                {
                    LoggedInPatientService.LoggedInPatient = patient;
                    Console.WriteLine($"Logged in as {patient.Username}" + patient.FirstName);
                    Console.WriteLine($"Logged in as {LoggedInPatientService.LoggedInPatient.Username}" + LoggedInPatientService.LoggedInPatient.FirstName);
                    if (autoLogin)
                    {
                        // Save credentials securely for auto login
                        await SecureStorage.SetAsync("PatientID", patientID.ToString());
                        await SecureStorage.SetAsync("Password", password);
                    }

                }
            }
            return isValid;
        }
        public async Task<bool> AutoLoginAsync()
        {
            var patientIdString = await SecureStorage.GetAsync("PatientID");
            var password = await SecureStorage.GetAsync("Password");

            if (!string.IsNullOrEmpty(patientIdString) && !string.IsNullOrEmpty(password))
            {
                if (int.TryParse(patientIdString, out var patientID))
                {
                    return await VerifyPatientCallerAsync(patientID, password, true);
                }
            }

            return false;
        }
    }
}
