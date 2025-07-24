using Microsoft.AspNetCore.Mvc;
using ThaiNationalIDCard.NET;
using System.Globalization;

namespace CardScanApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NationalIdController : ControllerBase
    {
        private readonly ThaiNationalIDCardReader _reader;

        public NationalIdController()
        {
            _reader = new ThaiNationalIDCardReader();
        }

        [HttpGet("read")]
        public IActionResult ReadNationalIdCard()
        {
            try
            {
                // Read card data with photo
                var personalPhoto = _reader.GetPersonalPhoto();
                
                if (personalPhoto == null)
                {
                    return NotFound("ไม่พบบัตรประชาชนในเครื่องอ่าน หรือไม่สามารถอ่านข้อมูลได้");
                }
                
                // Log what data we actually received for debugging
                Console.WriteLine($"Card data received:");
                Console.WriteLine($"CitizenId: {personalPhoto.CitizenID ?? "NULL"}");
                Console.WriteLine($"Thai Name: {personalPhoto.ThaiPersonalInfo?.ToString() ?? "NULL"}");
                Console.WriteLine($"English Name: {personalPhoto.EnglishPersonalInfo?.ToString() ?? "NULL"}");
                Console.WriteLine($"Address: {personalPhoto.AddressInfo?.ToString() ?? "NULL"}");
                Console.WriteLine($"Photo: {(string.IsNullOrEmpty(personalPhoto.Photo) ? "NULL" : "Available")}");

                // Convert gender to Thai text
                string genderText = string.IsNullOrEmpty(personalPhoto.Sex) ? "ไม่ระบุ" : 
                    (personalPhoto.Sex == "1" ? "ชาย" : (personalPhoto.Sex == "2" ? "หญิง" : "ไม่ระบุ"));

                // Use Thai culture for date formatting
                CultureInfo thaiCulture = new CultureInfo("th-TH");

                // Helper function to safely format dates
                string SafeFormatDate(DateTime date)
                {
                    try
                    {
                        return date == DateTime.MinValue ? "ไม่ระบุ" : date.ToString("dd MMMM yyyy", thaiCulture);
                    }
                    catch
                    {
                        return "ไม่ระบุ";
                    }
                }

                // Extract photo base64 (remove data:image/jpeg;base64, prefix if exists)
                string? photoBase64 = null;
                if (!string.IsNullOrEmpty(personalPhoto.Photo))
                {
                    if (personalPhoto.Photo.StartsWith("data:image/jpeg;base64,"))
                    {
                        photoBase64 = personalPhoto.Photo.Substring("data:image/jpeg;base64,".Length);
                    }
                    else
                    {
                        photoBase64 = personalPhoto.Photo;
                    }
                }

                var response = new
                {
                    citizenId = string.IsNullOrEmpty(personalPhoto.CitizenID) ? "ไม่ระบุ" : personalPhoto.CitizenID,
                    titleNameTh = string.IsNullOrEmpty(personalPhoto.ThaiPersonalInfo?.Prefix) ? "ไม่ระบุ" : personalPhoto.ThaiPersonalInfo.Prefix,
                    firstNameTh = string.IsNullOrEmpty(personalPhoto.ThaiPersonalInfo?.FirstName) ? "ไม่ระบุ" : personalPhoto.ThaiPersonalInfo.FirstName,
                    lastNameTh = string.IsNullOrEmpty(personalPhoto.ThaiPersonalInfo?.LastName) ? "ไม่ระบุ" : personalPhoto.ThaiPersonalInfo.LastName,
                    titleNameEn = string.IsNullOrEmpty(personalPhoto.EnglishPersonalInfo?.Prefix) ? "ไม่ระบุ" : personalPhoto.EnglishPersonalInfo.Prefix,
                    firstNameEn = string.IsNullOrEmpty(personalPhoto.EnglishPersonalInfo?.FirstName) ? "ไม่ระบุ" : personalPhoto.EnglishPersonalInfo.FirstName,
                    lastNameEn = string.IsNullOrEmpty(personalPhoto.EnglishPersonalInfo?.LastName) ? "ไม่ระบุ" : personalPhoto.EnglishPersonalInfo.LastName,
                    gender = genderText,
                    birthDate = SafeFormatDate(personalPhoto.DateOfBirth),
                    issueDate = SafeFormatDate(personalPhoto.IssueDate),
                    expireDate = SafeFormatDate(personalPhoto.ExpireDate),
                    address = personalPhoto.AddressInfo?.ToString() ?? "ไม่ระบุ",
                    photoBase64 = photoBase64,
                    // Add debug info to see what's available
                    DebugInfo = new
                    {
                        HasCitizenId = !string.IsNullOrEmpty(personalPhoto.CitizenID),
                        HasThaiName = personalPhoto.ThaiPersonalInfo != null,
                        HasEnglishName = personalPhoto.EnglishPersonalInfo != null,
                        HasAddress = personalPhoto.AddressInfo != null,
                        HasPhoto = !string.IsNullOrEmpty(photoBase64),
                        HasGender = !string.IsNullOrEmpty(personalPhoto.Sex),
                        RawThaiInfo = personalPhoto.ThaiPersonalInfo?.ToString(),
                        RawEnglishInfo = personalPhoto.EnglishPersonalInfo?.ToString(),
                        RawAddress = personalPhoto.AddressInfo?.ToString(),
                        Issuer = personalPhoto.Issuer
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"เกิดข้อผิดพลาดในการอ่านบัตร: {ex.Message}");
            }
        }
    }
}