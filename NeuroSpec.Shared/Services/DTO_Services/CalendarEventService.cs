﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NeuroSpec.Shared.Models.DTO;

namespace NeuroSpecCompanion.Shared.Services.DTO_Services
{
    public class CalendarEventService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApi;
        private readonly JsonSerializerOptions options;
        public CalendarEventService()
        {
            _httpClient = new HttpClient();
            _baseApi = "http://neurospec.runasp.net/api/CalendarEvent";
            options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<CalendarEvent>> GetAllCalendarEventsAsync()
        {
            var response = await _httpClient.GetAsync(_baseApi);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CalendarEvent>>(content, options);
        }

        public async Task<CalendarEvent> GetCalendarEventByIDAsync(int eventID)
        {
            var response = await _httpClient.GetAsync($"{_baseApi}/{eventID}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CalendarEvent>(content, options);
        }

        public async Task<CalendarEvent> InsertCalendarEventAsync(CalendarEvent calendarEvent)
        {
            var json = JsonSerializer.Serialize(calendarEvent);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseApi, content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CalendarEvent>(responseContent, options);
        }

        public async Task UpdateCalendarEventAsync(int eventID, CalendarEvent calendarEvent)
        {
            var json = JsonSerializer.Serialize(calendarEvent);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseApi}/{eventID}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCalendarEventAsync(int eventID)
        {
            var response = await _httpClient.DeleteAsync($"{_baseApi}/{eventID}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<CalendarEvent>> GetCalendarEventsByUserIDAndDateAsync(int userID, DateTime dateTime)
        {
            var response = await _httpClient.GetAsync($"{_baseApi}/ByUserIDAndDate/{userID}/{dateTime.ToString("yyyy-MM-dd")}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CalendarEvent>>(content, options);
        }

    }
}
