using System;
using System.Threading.Tasks;
using Janet.DubCore.Models;

namespace Janet.DubCore.Services;

public interface IAppConfigService
{
    AppConfigSettings Settings { get; }
    Task LoadAsync();
    Task SaveAsync();
    Task UpdateAsync(Action<AppConfigSettings> updateAction);
}