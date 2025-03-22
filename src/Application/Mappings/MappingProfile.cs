using Application.DTOs.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<Company, CompanyDto>().ReverseMap();
        CreateMap<ServiceTask, ServiceTaskDto>().ReverseMap();
        CreateMap<MonitoringSetting, MonitoringSettingDto>().ReverseMap();
        CreateMap<Metric, MetricDto>().ReverseMap();
        CreateMap<MetricValue, MetricValueDto>().ReverseMap();
        CreateMap<Resource, ResourceDto>().ReverseMap();
    }
}