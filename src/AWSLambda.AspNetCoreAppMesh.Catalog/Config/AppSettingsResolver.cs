﻿using AWSLambda.AspNetCoreAppMesh.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreAppMesh.Catalog.Config
{
    public static class AppSettingsResolver
    {
        static readonly string settingsFilePath = "settings.json";

        public static AppSettings Load(string[] args)
        {
            var settings = Load();

            // no current settings
            if(settings == null)
            {
                settings = new AppSettings()
                {
                    Args = args
                };

                return settings;
            }

            // if args were passed in, they supercede any saved args
            if (args != null && args.Any())
            {
                settings.Args = args;

                return settings;
            }

            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] Using previously saved application settings");

            return settings;
        }

        public static void Save(AppSettings settings)
        {
            try
            {
                using var isf = IsolatedStorageFile.GetUserStoreForApplication();

                using var fs = new IsolatedStorageFileStream(settingsFilePath, FileMode.Create, FileAccess.Write, isf);

                JsonUtil.Serialize(fs, settings);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error saving app settings to isolated storage: {ex.ToString()}");
            }

            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] Application settings saved");
        }

        static AppSettings Load()
        {
            try
            {
                using var isf = IsolatedStorageFile.GetUserStoreForApplication();

                if (!isf.FileExists(settingsFilePath))
                    return null;

                using var fs = new IsolatedStorageFileStream(settingsFilePath, FileMode.Open, FileAccess.Read, isf);

                var settings = JsonUtil.Deserialize<AppSettings>(fs);

                return settings;
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"Error reading app settings from isolated storage: {ex.ToString()}");
            }

            return null;
        }
    }
}
