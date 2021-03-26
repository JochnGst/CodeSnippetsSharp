using System;

namespace _01_AppSettingsJson
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var userSettings = new UserSettings<UserSettingsModel>();
            userSettings.Settings.BaseColor = "Green";
            userSettings.Save();
            userSettings.ConfigFilePath = @".\newFolder\test\config.json";
            userSettings.Read();
        }
    }
}
