namespace TestApi
{
    public class ApiCallService
    {
        private readonly HttpClient _httpClient;

        public ApiCallService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CallApiEndpointAsync()
        {
            var response = await _httpClient.GetAsync("https://localhost:7282/api/TestApi");
            response.EnsureSuccessStatusCode();
            // Log success or handle the response as needed
        }
    }
}
