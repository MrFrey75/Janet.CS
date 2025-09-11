using System;
using System.Threading.Tasks;
using Janet.DubCore.Models;

namespace Janet.DubCore.Services;

/// <summary>
/// Defines the contract for a strongly-typed application configuration service.
/// </summary>
public interface IAppConfigService
{
    /// <summary>
    /// Gets the current, in-memory application settings.
    /// </summary>
    AppConfigSettings Settings { get; }

    /// <summary>
    /// Loads the settings from the YAML file asynchronously.
    /// This is typically called once at startup.
    /// </summary>
    Task LoadAsync();

    /// <summary>
    /// Saves the current in-memory settings to the YAML file asynchronously.
    /// </summary>
    Task SaveAsync();

    /// <summary>
    /// Updates the settings in a transactional manner and persists them to the file.
    /// </summary>
    /// <param name="updateAction">An action to perform on the settings object.</param>
    Task UpdateAsync(Action<AppConfigSettings> updateAction);
}