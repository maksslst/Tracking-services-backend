using Application.DTOs.Mappings;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories;
using Infrastructure.Repositories.MonitoringSettingRepository;

namespace Application.Services;

public class MonitoringSettingService : IMonitoringSettingService
{
    private readonly IMonitoringSettingRepository _monitoringSettingRepository;
    private readonly IMapper _mapper;

    public MonitoringSettingService(IMonitoringSettingRepository monitoringSettingRepository, IMapper mapper)
    {
        _mapper = mapper;
        _monitoringSettingRepository = monitoringSettingRepository;
    }

    public async Task<MonitoringSetting?> Add(MonitoringSettingDto monitoringSettingDto)
    {
        MonitoringSetting mappedMonitoringSetting = _mapper.Map<MonitoringSetting>(monitoringSettingDto);
        if (mappedMonitoringSetting != null)
        {
            await _monitoringSettingRepository.CreateSetting(mappedMonitoringSetting);
            return mappedMonitoringSetting;
        }

        return null;
    }

    public async Task<bool> Update(MonitoringSettingDto monitoringSettingDto)
    {
        MonitoringSetting mappedMonitoringSetting = _mapper.Map<MonitoringSetting>(monitoringSettingDto);
        if (mappedMonitoringSetting == null)
        {
            return false;
        }

        return await _monitoringSettingRepository.UpdateSetting(mappedMonitoringSetting);
    }

    public async Task<bool> Delete(int monitoringSettingId)
    {
        return await _monitoringSettingRepository.DeleteSetting(monitoringSettingId);
    }

    public async Task<MonitoringSettingDto?> GetMonitoringSetting(int serviceId)
    {
        MonitoringSetting? monitoringSetting = await _monitoringSettingRepository.ReadByResourceId(serviceId);
        MonitoringSettingDto mappedMonitoringSetting = _mapper.Map<MonitoringSettingDto>(monitoringSetting);
        return mappedMonitoringSetting;
    }
}