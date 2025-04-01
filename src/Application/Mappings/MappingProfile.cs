using Application.DTOs.Mappings;
using Application.Requests;
using Application.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserResponse>().ReverseMap();
        CreateMap<Company, CompanyResponse>().ReverseMap();
        CreateMap<ServiceTask, TaskResponse>().ReverseMap();
        CreateMap<MonitoringSetting, MonitoringSettingResponse>().ReverseMap();
        CreateMap<Metric, MetricResponse>().ReverseMap();
        CreateMap<MetricValue, MetricValueResponse>().ReverseMap();
        CreateMap<Resource, ResourceResponse>().ReverseMap();
    }
}