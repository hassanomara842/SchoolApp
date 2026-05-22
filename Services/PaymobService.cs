using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SchoolApp.Models;

namespace SchoolApp.Services
{
    public class PaymobService : IPaymobService
    {
        private readonly HttpClient _httpClient;
        private readonly PaymobSettings _settings;

        public PaymobService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _settings = config.GetSection("PaymobSettings").Get<PaymobSettings>() ?? new PaymobSettings();
        }

        public async Task<(string? paymentKey, string? orderId)> GetPaymentKeyAsync(Course course, Trainee trainee)
        {
            try
            {
                // Step 1: Authentication
                var authBody = new { api_key = _settings.ApiKey };
                var authResponse = await PostAsync("https://accept.paymob.com/api/auth/tokens", authBody);
                if (authResponse == null) return (null, null);
                var authToken = authResponse.Value.GetProperty("token").GetString();

                // Step 2: Order Registration
                int amountCents = (int)((course.DiscountPrice ?? course.Price) * 100);
                
                var orderBody = new
                {
                    auth_token = authToken,
                    delivery_needed = "false",
                    amount_cents = amountCents.ToString(),
                    currency = "EGP",
                    items = new[]
                    {
                        new
                        {
                            name = course.Name,
                            amount_cents = amountCents.ToString(),
                            description = "Course Enrollment",
                            quantity = "1"
                        }
                    }
                };
                
                var orderResponse = await PostAsync("https://accept.paymob.com/api/ecommerce/orders", orderBody);
                if (orderResponse == null) return (null, null);
                var orderId = orderResponse.Value.GetProperty("id").GetInt32().ToString();

                // Step 3: Payment Key Request
                var paymentKeyBody = new
                {
                    auth_token = authToken,
                    amount_cents = amountCents.ToString(),
                    expiration = 3600,
                    order_id = orderId,
                    billing_data = new
                    {
                        apartment = "NA",
                        email = "trainee@example.com", // Mocked as we don't have email in Trainee model easily accessible here without joining AspNetUsers, Paymob requires these fields but they can be dummy for testing
                        floor = "NA",
                        first_name = trainee.Name.Split(' ').FirstOrDefault() ?? "Student",
                        street = trainee.Address ?? "NA",
                        building = "NA",
                        phone_number = "+201000000000",
                        shipping_method = "PKG",
                        postal_code = "NA",
                        city = "Cairo",
                        country = "EG",
                        last_name = trainee.Name.Split(' ').LastOrDefault() ?? "Student",
                        state = "Cairo"
                    },
                    currency = "EGP",
                    integration_id = int.Parse(_settings.IntegrationId)
                };

                var paymentResponse = await PostAsync("https://accept.paymob.com/api/acceptance/payment_keys", paymentKeyBody);
                if (paymentResponse == null) return (null, orderId);
                var paymentKey = paymentResponse.Value.GetProperty("token").GetString();

                return (paymentKey, orderId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Paymob Error: {ex.Message}");
                return (null, null);
            }
        }

        public bool ValidateHMAC(IQueryCollection query)
        {
            // Paymob standard HMAC calculation
            string amount_cents = query["amount_cents"].ToString();
            string created_at = query["created_at"].ToString();
            string currency = query["currency"].ToString();
            string error_occured = query["error_occured"].ToString();
            string has_parent_transaction = query["has_parent_transaction"].ToString();
            string id = query["id"].ToString();
            string integration_id = query["integration_id"].ToString();
            string is_3d_secure = query["is_3d_secure"].ToString();
            string is_auth = query["is_auth"].ToString();
            string is_capture = query["is_capture"].ToString();
            string is_refunded = query["is_refunded"].ToString();
            string is_standalone_payment = query["is_standalone_payment"].ToString();
            string is_voided = query["is_voided"].ToString();
            string order_id = query["order"].ToString();
            string owner = query["owner"].ToString();
            string pending = query["pending"].ToString();
            string source_data_pan = query["source_data.pan"].ToString();
            string source_data_sub_type = query["source_data.sub_type"].ToString();
            string source_data_type = query["source_data.type"].ToString();
            string success = query["success"].ToString();

            string concatenatedString = 
                amount_cents + created_at + currency + error_occured + has_parent_transaction + 
                id + integration_id + is_3d_secure + is_auth + is_capture + is_refunded + 
                is_standalone_payment + is_voided + order_id + owner + pending + 
                source_data_pan + source_data_sub_type + source_data_type + success;

            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_settings.HMACSecret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString));
                var hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();
                return hashString == query["hmac"].ToString().ToLower();
            }
        }

        private async Task<JsonElement?> PostAsync(string url, object data)
        {
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<JsonElement>(json);
            }
            return null;
        }
    }
}
