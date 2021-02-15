using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfTemplate.Utilities
{
    /// <summary>
    /// You can define your own settings objects that will be stored in (and retrieved from) the user folder with this class.
    /// You can also encrypt the settings with this class.
    /// </summary>
    public class AppSettingsService
    {
        private readonly string _folder;

        private readonly byte[] key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

        public static AppSettingsService Create(string appName)
        {
            if (string.IsNullOrWhiteSpace(appName))
                throw new ArgumentNullException(nameof(appName));

            return new AppSettingsService(appName);
        }

        private AppSettingsService() : this("") { }

        public AppSettingsService(string appName)
        {
            if (string.IsNullOrWhiteSpace(appName))
            {
                var assemblyName = Assembly.GetExecutingAssembly().FullName;
                if (string.IsNullOrEmpty(assemblyName))
                {
                    appName = "MyApp";
                }
                else
                {
                    int i = assemblyName.IndexOf(".");
                    if (i < 0)
                        appName = assemblyName;
                    else
                    {
                        appName = assemblyName.Substring(0, i);
                    }
                }
            }
            _folder = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), appName);
        }

        /// <summary>
        /// Access the user's app settings folder
        /// </summary>
        internal string AppDataFolder
        {
            get
            {
                if (!Directory.Exists(_folder))
                    Directory.CreateDirectory(_folder);
                return _folder;
            }
        }

        internal async Task WriteSettings<T>(string fileName, T settings)
        {
            string fn = Path.Combine(AppDataFolder, fileName);
            string json = JsonSerializer.Serialize<T>(settings);
            await using StreamWriter writer = new StreamWriter(fn);
            await writer.WriteAsync(json).ConfigureAwait(false);
        }

        internal async Task<T> LoadSettings<T>(string fileName) where T : class
        {
            string fn = Path.Combine(AppDataFolder, fileName);

            if (!File.Exists(fn))
                return null;

            using StreamReader reader = new StreamReader(fn);
            string json = await reader.ReadToEndAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(json);
        }

        internal void WriteSettingsEncrypted<T>(string filePath, T item)
        {
            string json = JsonSerializer.Serialize<T>(item);
            using FileStream myStream = new FileStream(filePath, FileMode.OpenOrCreate);
            using Aes aes = Aes.Create();
            aes.Key = key;
            byte[] iv = aes.IV;
            myStream.Write(iv, 0, iv.Length);
            using CryptoStream cryptStream = new CryptoStream(
                myStream,
                aes.CreateEncryptor(),
                CryptoStreamMode.Write);
            using StreamWriter sWriter = new StreamWriter(cryptStream);
            sWriter.Write(json);
        }

        internal T LoadSettingsEncrypted<T>(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                throw new ArgumentException(nameof(filePath));

            using FileStream myStream = new FileStream(filePath, FileMode.Open);
            using Aes aes = Aes.Create();
            byte[] iv = new byte[aes.IV.Length];
            myStream.Read(iv, 0, iv.Length);
            using CryptoStream cryptStream = new CryptoStream(
                myStream,
                aes.CreateDecryptor(key, iv),
                CryptoStreamMode.Read);
            using StreamReader sReader = new StreamReader(cryptStream);
            string json = sReader.ReadToEnd();
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

    }
}
