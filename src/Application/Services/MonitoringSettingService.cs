using Application.DTOs.Mappings;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories;

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

    public async Task Add(MonitoringSettingDto monitoringSettingDto)
    {
        MonitoringSetting mappedMonitoringSetting = _mapper.Map<MonitoringSetting>(monitoringSettingDto);
        if (mappedMonitoringSetting != null)
        {
            await _monitoringSettingRepository.CreateSetting(mappedMonitoringSetting);
        }
    }

    public async Task<bool> Update(MonitoringSettingDto monitoringSettingDto)
    {
        MonitoringSetting mappedMonitoringSetting = _mapper.Map<MonitoringSetting>(monitoringSettingDto);
        return await _monitoringSettingRepository.UpdateSetting(mappedMonitoringSetting);
    }

    public async Task<bool> Delete(int monitoringSettingId)
    {
        return await _monitoringSettingRepository.DeleteSetting(monitoringSettingId);
    }

    public async Task<MonitoringSettingDto?> GetByServiceId(int serviceId)
    {
        MonitoringSetting? monitoringSetting = await _monitoringSettingRepository.ReadByServiceId(serviceId);
        MonitoringSettingDto mappedMonitoringSetting = _mapper.Map<MonitoringSettingDto>(monitoringSetting);
        return mappedMonitoringSetting;
    }
}