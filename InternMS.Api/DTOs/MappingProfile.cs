using AutoMapper;
using InternMS.Domain.Entities;

namespace InternMS.Api.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .FroMember(dest => dest.Role, opt => opt.MapFrom(src.UserRoles != null && src.UserRoles.count > 0 ? 
                src.UserRoles.First().Role.Name: null));

            CreateMap<CreateUserDto, User>();
            CreateMap<Profile, ProfileDto>();
            CreateMap<UpdateProfileDto, Profile>();
            CreateMap<Project, ProjectDto>()
                .FroMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateProjectDto, Project>();
            CreateMap<ProjectUpdate, ProjectUpdateDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateProjectUpdateDto, ProjectUpdate>();
            CreateMap<Notification, NotificationDto>();
        }
    }
}