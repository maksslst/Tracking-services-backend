using Application.DTOs.Mappings;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.MonitoringSettingRepository;
using Application.Requests;
using Application.Responses;
using Infrastructure.Repositories.ResourceRepository;

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

    public async Task<MonitoringSetting?> Add(CreateMonitoringSettingRequest request)
    {
        if (await _resourceRepository.ReadByResourceId(request.ResourceId) == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        var monitoringSetting = new MonitoringSetting()
        {
            ResourceId = request.ResourceId,
            CheckInterval = request.CheckInterval,
            Mode = request.Mode
        };

        await _monitoringSettingRepository.CreateSetting(monitoringSetting);
        return monitoringSetting;
    }

    public async Task<bool> Update(UpdateMonitoringSettingRequest request)
    {
        if (await _monitoringSettingRepository.ReadByResourceId(request.ResourceId) == null)
        {
            throw new NotFoundApplicationException("MonitoringSetting not found");
        }

        var monitoringSetting = new MonitoringSetting()
        {
            Id = request.Id,
            CheckInterval = request.CheckInterval,
            Mode = request.Mode,
            ResourceId = request.ResourceId
        };

        return await _monitoringSettingRepository.UpdateSetting(monitoringSetting);
    }

    public async Task<bool> Delete(int monitoringSettingId)
    {
        if (await _monitoringSettingRepository.ReadById(monitoringSettingId) == null)
        {
            throw new NotFoundApplicationException("MonitoringSetting not found");
        }

        return await _monitoringSettingRepository.DeleteSetting(monitoringSettingId);
    }

    public async Task<MonitoringSettingResponse?> GetMonitoringSetting(int serviceId)
    {
        var monitoringSetting = await _monitoringSettingRepository.ReadByResourceId(serviceId);
        if (monitoringSetting == null)
        {
            throw new NotFoundApplicationException("MonitoringSetting not found");
        }

        return _mapper.Map<MonitoringSettingResponse>(monitoringSetting);
    }
}