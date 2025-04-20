using Application.Requests;
using Application.Responses;

namespace Application.Services;

public interface IMonitoringSettingService
{
    public Task<int> Add(CreateMonitoringSettingRequest request);
    public Task Update(UpdateMonitoringSettingRequest request);
    public Task Delete(int monitoringSettingId);
    public Task<MonitoringSettingResponse> GetMonitoringSetting(int resourceId);
}