using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SigortaVip.Models;

namespace SigortaYazilim.Api
{
    public class InsuranceCompanyItemResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("insurance_company")]
        public InsuranceCompany InsuranceCompany { get; set; }

        [JsonProperty("company")]
        public Company Company { get; set; }

        [JsonProperty("cookies")]
        public List<InsuranceCookie> cookies { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("proxy_url")]
        public string ProxyUrl { get; set; }

        [JsonProperty("proxy_username")]
        public string ProxyUsername { get; set; }

        [JsonProperty("proxy_password")]
        public string ProxyPassword { get; set; }

        [JsonProperty("is_proxy_active")]
        public bool IsProxyActive { get; set; }

        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

        [JsonProperty("cookie_use")]
        public bool CookieUse { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
} 