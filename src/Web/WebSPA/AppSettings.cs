namespace WebSPA
{
    public class AppSettings
    {
        public string IdentityUrl { get; set; } = "http://localhost:55105/";

        public string ConferenceUrl { get; set; } = "http://localhost:55104/";

        public string SignalrHubUrl { get; set; } = "http://localhost:55104/signalr";

        public string EquipmentSignalrHubUrl { get; set; } = "http://localhost:55104/equipment-signalr";

        public string FrontendUrl { get; set; } = "http://localhost:55103/";
    }
}
