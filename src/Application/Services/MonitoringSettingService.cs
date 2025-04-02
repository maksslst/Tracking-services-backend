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
        try
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

            return await _monitoringSettingRepository.CreateSetting(monitoringSetting);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't add setting");
        }
    }

    public async Task<bool> Update(UpdateMonitoringSettingRequest request)
    {
        try
        {
            var monitoringSetting = await _monitoringSettingRepository.ReadByResourceId(request.ResourceId);
            if (monitoringSetting == null)
            {
                throw new NotFoundApplicationException("MonitoringSetting not found");
            }

            monitoringSetting.CheckInterval = request.CheckInterval;
            monitoringSetting.Mode = request.Mode;
            monitoringSetting.ResourceId = request.ResourceId;
            return await _monitoringSettingRepository.UpdateSetting(monitoringSetting);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't update setting");
        }
    }

    public async Task<bool> Delete(int monitoringSettingId)
    {
        try
        {
            bool isDeleted = await _monitoringSettingRepository.DeleteSetting(monitoringSettingId);
            if (!isDeleted)
            {
                throw new NotFoundApplicationException("MonitoringSetting not found");
            }

            return true;
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't delete setting");
        }
    }

    public async Task<MonitoringSettingResponse> GetMonitoringSetting(int serviceId)
    {
        try
        {
            var monitoringSetting = await _monitoringSettingRepository.ReadByResourceId(serviceId);
            if (monitoringSetting == null)
            {
                throw new NotFoundApplicationException("MonitoringSetting not found");
            }

            return _mapper.Map<MonitoringSettingResponse>(monitoringSetting);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't find the setting");
        }
    }
}