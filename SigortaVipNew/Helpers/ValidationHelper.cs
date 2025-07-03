using System;
using System.Text.RegularExpressions;

namespace SigortaVipNew.Helpers
{
    public static class ValidationHelper
    {
        // TC Kimlik No kontrolü
        public static bool IsValidTcNo(string tcNo)
        {
            if (string.IsNullOrWhiteSpace(tcNo) || tcNo.Length != 11)
                return false;

            // Sadece rakam kontrolü
            if (!Regex.IsMatch(tcNo, @"^\d{11}$"))
                return false;

            return true;
        }

        // Vergi No kontrolü
        public static bool IsValidVergiNo(string vergiNo)
        {
            if (string.IsNullOrWhiteSpace(vergiNo) || vergiNo.Length != 10)
                return false;

            // Sadece rakam kontrolü
            return Regex.IsMatch(vergiNo, @"^\d{10}$");
        }

        // Plaka kontrolü
        public static bool IsValidPlate(string plate)
        {
            if (string.IsNullOrWhiteSpace(plate))
                return false;

            // Türk plaka formatı: 34ABC123 veya 34AB1234
            string pattern = @"^\d{2}[A-Z]{1,3}\d{2,4}$";
            return Regex.IsMatch(plate.ToUpper().Replace(" ", ""), pattern);
        }

        // Doğum tarihi kontrolü
        public static bool IsValidBirthDate(string birthDate)
        {
            if (string.IsNullOrWhiteSpace(birthDate))
                return false;

            // dd.MM.yyyy formatında olmalı
            if (DateTime.TryParseExact(birthDate, "dd.MM.yyyy", null,
                System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                // 18-100 yaş arası olmalı
                var age = DateTime.Now.Year - date.Year;
                return age >= 18 && age <= 100;
            }

            return false;
        }

        // Seri No kontrolü
        public static bool IsValidSerialNo(string serialNo)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
                return false;

            // En az 6 karakter olmalı
            return serialNo.Replace(" ", "").Length >= 6;
        }

        // Telefon kontrolü
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Türk telefon numarası: 5xxxxxxxxx
            string cleanPhone = phone.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
            return Regex.IsMatch(cleanPhone, @"^5\d{9}$");
        }

        // Email kontrolü
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }
    }
}