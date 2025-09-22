using AAUParkingService.Helpers;
using Hangfire;

namespace AAUParkingService.Jobs;

[DisableConcurrentExecution(60)]
public class ParkingJob(ILogger<ParkingJob> logger, HttpClient httpClient)
{
    public async Task Execute()
    {
        logger.LogInformation("Parking job executed at {Time}", DateTime.Now);

        var phoneNumber = EnvironmentHelper.GetEnvironmentVariable("PARKING_JOB_PHONE_NUMBER");
        var licensePlate = EnvironmentHelper.GetEnvironmentVariable("PARKING_JOB_LICENSE_PLATE");

        logger.LogInformation("Registering parking for '{LicensePlate}' using phone number '{PhoneNumber}'", 
            licensePlate, phoneNumber);
        
        var content = ContentHelper.GetRequestContent(phoneNumber, licensePlate); 
        
        var result = await httpClient.PostAsync("/v10/permit/Tablet/confirm", content);

        var responseContent = await result.Content.ReadAsStringAsync();
        if (result.IsSuccessStatusCode)
        {
            logger.LogInformation("Parking registered successfully: {ResponseContent}", responseContent);
        }
        else
        {
            logger.LogError("Failed to register parking: {StatusCode} - {ResponseContent}", 
                result.StatusCode, responseContent);
        }
    }
}