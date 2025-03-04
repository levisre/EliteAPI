﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using EliteAPI.Abstractions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EliteAPI.Configuration;

/// <inheritdoc />
public class EliteDangerousApiConfiguration : IEliteDangerousApiConfiguration
{
    private readonly IConfiguration _config;
    private readonly ILogger<EliteDangerousApiConfiguration> _log;

    private IConfiguration _lastConfig;

    /// <summary>Creates a new instance of <see cref="EliteDangerousApiConfiguration" /></summary>
    public EliteDangerousApiConfiguration(ILogger<EliteDangerousApiConfiguration> log, IConfiguration config)
    {
        _log = log;
        _config = config;
    }

    /// <inheritdoc />
    public string JournalsPath { get; set; }

    /// <inheritdoc />
    public string[]? StatusFiles { get; set; }

    /// <inheritdoc />
    public string JournalPattern { get; set; }

    /// <inheritdoc />
    public string OptionsPath { get; set; }

    /// <inheritdoc />
    public int UpdateDelay { get; set; }

    /// <inheritdoc />
    public void Apply()
    {
        _log.LogTrace("Applying configuration");
        
        UpdateDelay = _config.GetValue("EliteAPI:UpdateDelay", 500);
        UpdateDelay = Math.Max(0, UpdateDelay);
        _log.LogDebug("Update delay set to {UpdateDelay}ms", UpdateDelay);

        if (string.IsNullOrWhiteSpace(JournalPattern))
            JournalPattern = _config.GetValue("EliteAPI:JournalPattern", "Journal.*.log");
        _log.LogDebug("Journal pattern set to {JournalPattern}", JournalPattern);
        
        if (string.IsNullOrWhiteSpace(JournalsPath))
            JournalsPath = _config.GetValue("EliteAPI:JournalsPath", Path.Combine(GetSavedGamesPath(), "Frontier Developments", "Elite Dangerous"));
        _log.LogDebug("Journals path set to {JournalsPath}", JournalsPath);
        
        if (string.IsNullOrWhiteSpace(OptionsPath))
            OptionsPath = _config.GetValue("EliteAPI:OptionsPath", Path.Combine(GetLocalAppDataPath(), "Frontier Developments", "Elite Dangerous", "Options"));
        _log.LogDebug("Options path set to {OptionsPath}", OptionsPath);
        
        StatusFiles ??= _config.GetValue("EliteAPI:StatusFiles",
            new[]
            {
                "Status.json", "Backpack.json", "Cargo.json", "ModulesInfo.json", "NavRoute.json", "Outfitting.json",
                "ShipLocker.json", "Shipyard.json", "FCMaterials.json"
            });
        _log.LogDebug("Status files set to {StatusFiles}", string.Join(", ", StatusFiles));

        if (UpdateDelay <= 50)
            _log.LogWarning("The update delay is set to {UpdateDelay}ms, this is an arbitrary low value and may cause performance issues", UpdateDelay);
        
        if (UpdateDelay >= 2500)
            _log.LogWarning("The update delay is set to {UpdateDelay}ms, this is an arbitrary high value and may make the API feel unresponsive at times", UpdateDelay);

        if (!Directory.Exists(JournalsPath))
            _log.LogWarning(new DirectoryNotFoundException($"{JournalsPath} does not exist."),
                "The specified journals directory could not be found");

        if (!Directory.Exists(OptionsPath))
            _log.LogWarning(new DirectoryNotFoundException($"{OptionsPath} does not exist."),
                "The specified options directory could not be found");

    }

    private string GetSavedGamesPath()
    {
        try
        {
            // If we're on Windows, we can use the registry to get the path
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var result = SHGetKnownFolderPath(
                    new Guid("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4"),
                    0,
                    new IntPtr(0),
                    out var path);

                // If we got a path, return it
                if (result == 0)
                    return Marshal.PtrToStringUni(path);
            }

            // Otherwise, we'll get the path using userprofile
            var userProfile = Environment.GetEnvironmentVariable("USERPROFILE");

            // If we got a path, return it
            if (!string.IsNullOrWhiteSpace(userProfile))
                return Path.Combine(userProfile, "Saved Games");

            // Last resort, return the default
            return Path.Combine($@"C:\Users\{Environment.UserName}\Saved Games");
        }
        catch (Exception ex)
        {
            _log.LogDebug(ex, "Could not get the Saved Games directory");
            throw;
        }
    }

    private string GetLocalAppDataPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    }

    [DllImport("Shell32.dll")]
    private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags,
        IntPtr hToken, out IntPtr path);
}