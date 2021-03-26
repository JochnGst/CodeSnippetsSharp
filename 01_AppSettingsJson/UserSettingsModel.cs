namespace _01_AppSettingsJson
{
    internal class UserSettingsModel : IUserSettingsModel
    {
        public string Version { get; set; } = "1.0";
        public string ColorScheme { get; set; } = "Blue";
        public string BaseColor { get; set; } = "Light";
    }
}