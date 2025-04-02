using Application.DTOs.Mappings;
using Domain.Entities;
using Application.Requests;
using Application.Responses;

namespace Application.Services;

public interface IMonitoringSettingService
{
    public Task<int> Add(CreateMonitoringSettingRequest request);
    public Task<bool> Update(UpdateMonitoringSettingRequest request);
    public Task<bool> Delete(int monitoringSettingId);
    public Task<MonitoringSettingResponse> GetMonitoringSetting(int serviceId);
}