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
        CreateMap<CreateUserRequest, User>();
        CreateMap<UpdateUserRequest, User>();
        CreateMap<Company, CompanyResponse>().ReverseMap();
        CreateMap<CreateCompanyRequest, Company>();
        CreateMap<ServiceTask, TaskResponse>().ReverseMap();
        CreateMap<CreateTaskRequest, ServiceTask>();
        CreateMap<UpdateTaskRequest, ServiceTask>();
        CreateMap<MonitoringSetting, MonitoringSettingResponse>().ReverseMap();
        CreateMap<CreateMonitoringSettingRequest, MonitoringSetting>();
        CreateMap<UpdateMonitoringSettingRequest, MonitoringSetting>();
        CreateMap<Metric, MetricResponse>().ReverseMap();
        CreateMap<CreateMetricRequest, Metric>();
        CreateMap<UpdateMetricRequest, Metric>();
        CreateMap<MetricValue, MetricValueResponse>().ReverseMap();
        CreateMap<CreateMetricValueRequest, MetricValue>();
        CreateMap<Resource, ResourceResponse>().ReverseMap();
        CreateMap<CreateResourceRequest, Resource>();
        CreateMap<UpdateResourceRequest, Resource>();
        CreateMap<RegistrationRequest, User>();
    }
}