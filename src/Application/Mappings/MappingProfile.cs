using AutoMapper;
using Application.DTOs.Mappings;
using Application.Services;
using Domain.Entities;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<Company, CompanyDto>().ForMember(i => i.UsersDto, opt => opt.MapFrom(u => u.Users)).ReverseMap();
        CreateMap<ServiceTask, ServiceTaskDto>()
            .ForMember(i => i.AssignedUserDto, opt => opt.MapFrom(u => u.AssignedUser)).ForMember(i => i.CreatedByDto, opt => opt.MapFrom(u => u.CreatedBy)).ReverseMap();
        CreateMap<MonitoringSetting, MonitoringSettingDto>().ReverseMap();
        CreateMap<Metric, MetricDto>().ReverseMap();
        CreateMap<MetricValue, MetricValueDto>().ReverseMap();
        CreateMap<Resource, ResourceDto>().ReverseMap();
    }
}