using System.Text.Json;

namespace AAUParkingService.Helpers;

public static class ContentHelper
{
    public static StringContent GetRequestContent(string phoneNumber, string licensePlate)
    {
        var requestBody = new
        {
            email = "",
            PhoneNumber = $"45{phoneNumber}",
            VehicleRegistrationCountry = "DK",
            Duration = 600,
            VehicleRegistration = licensePlate,
            parkingAreas = new[]
            {
                new
                {
                    ParkingAreaId = 1956,
                    ParkingAreaKey = "ADK-4688"
                }
            },
            UId = "12cdf204-d969-469a-9bd5-c1f1fc59ee34",
            Lang = "da"
        };
        
        var serializedBody = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(serializedBody, System.Text.Encoding.UTF8, "application/json");
        
        return content;
    }
}