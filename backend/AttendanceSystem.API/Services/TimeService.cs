using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace AttendanceSystem.API.Services;

public interface ITimeService
{
    Task<DateTime> GetCurrentTimeAsync();
}

public class TimeApiOptions
{
    public string? BaseUrl { get; set; } 
    public string? TimeZone { get; set; }
}

public class TimeService : ITimeService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TimeService> _logger;
    private readonly TimeApiOptions _options;

    public TimeService(
        HttpClient httpClient,
        ILogger<TimeService> logger,
        IOptions<TimeApiOptions> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<DateTime> GetCurrentTimeAsync()
    {
        var url = $"{_options.BaseUrl}?timeZone={Uri.EscapeDataString(_options.TimeZone)}";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<WorldTimeResponse>(url);

            if (response?.Datetime == null)
                throw new TimeServiceException("תגובה ריקה משירות הזמן");

            return DateTime.Parse(response.Datetime);
        }
        catch (TimeServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "שגיאת חיבור לשירות הזמן החיצוני עבור אזור הזמן {TimeZone}", _options.TimeZone);
            throw new TimeServiceException("לא ניתן לגשת לשירות הזמן. נסה שוב.", ex);
        }
    }
}

public class WorldTimeResponse
{
    [JsonPropertyName("dateTime")]
    public string? Datetime { get; set; }
}

public class TimeServiceException : Exception
{
    public TimeServiceException(string message, Exception? inner = null)
        : base(message, inner)
    {
    }
}