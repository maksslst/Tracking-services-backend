using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.MonitoringSettingRepository;
using Application.Requests;
using Application.Responses;
using Infrastructure.Repositories.ResourceRepository;
using Npgsql;

namespace Application.Services;

public class MonitoringSettingService : IMonitoringSettingService
{
    private readonly IMonitoringSettingRepository _monitoringSettingRepository;
    private readonly IMapper _mapper;
    private readonly IResourceRepository _resourceRepository;

    public MonitoringSettingService(IMonitoringSettingRepository monitoringSettingRepository, IMapper mapper,
        IResourceRepository resourceRepository)
    {
        _mapper = mapper;
        _monitoringSettingRepository = monitoringSettingRepository;
        _resourceRepository = resourceRepository;
    }

    public async Task<int> Add(CreateMonitoringSettingRequest request)
    {
        var monitoringSetting = _mapper.Map<MonitoringSetting>(request);
        return await _monitoringSettingRepository.CreateSetting(monitoringSetting);
    }

    public async Task<bool> Update(UpdateMonitoringSettingRequest request)
    {
        var monitoringSetting = await _monitoringSettingRepository.ReadByResourceId(request.ResourceId);
        if (monitoringSetting == null)
        {
            throw new NotFoundApplicationException("MonitoringSetting not found");
        }

        monitoringSetting = _mapper.Map<MonitoringSetting>(request);
        return await _monitoringSettingRepository.UpdateSetting(monitoringSetting);
    }

    public async Task<bool> Delete(int monitoringSettingId)
    {
        bool isDeleted = await _monitoringSettingRepository.DeleteSetting(monitoringSettingId);
        if (!isDeleted)
        {
            throw new NotFoundApplicationException("MonitoringSetting not found");
        }

        return true;
    }

    public async Task<MonitoringSettingResponse> GetMonitoringSetting(int serviceId)
    {
        var monitoringSetting = await _monitoringSettingRepository.ReadByResourceId(serviceId);
        if (monitoringSetting == null)
        {
            throw new NotFoundApplicationException("MonitoringSetting not found");
        }

        return _mapper.Map<MonitoringSettingResponse>(monitoringSetting);
    }
}