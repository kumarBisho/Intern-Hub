// using AutoMapper;
// using AutoMapperProfile = AutoMapper.Profile;
// using InternMS.Domain.Entities;

// namespace InternMS.Api.DTOs
// {
//     public class MappingProfile : AutoMapperProfile
//     {
//         private object src;

//         public MappingProfile()
//         {
//             CreateMap<User, UserDto>()
//                 .ForMember(dest => dest.Role, opt => opt.MapFrom(src.UserRoles != null && src.UserRoles.count > 0 ? 
//                 src.UserRoles.First().Role.Name: null));

//             CreateMap<CreateUserDto, User>();
//             CreateMap<Profile, ProfileDto>();
//             CreateMap<UpdateProfileDto, Profile>();
//             CreateMap<Project, ProjectDto>()
//                 .FroMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

//             CreateMap<CreateProjectDto, Project>();
//             CreateMap<ProjectUpdate, ProjectUpdateDto>()
//                 .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

//             CreateMap<CreateProjectUpdateDto, ProjectUpdate>();
//             CreateMap<Notification, NotificationDto>();
//         }
//     }
// }

using AutoMapper;
using AutoMapperProfile = AutoMapper.Profile;
using InternMS.Domain.Entities;
using System.Linq;

namespace InternMS.Api.DTOs
{
    public class MappingProfile : AutoMapperProfile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role,
                opt => opt.MapFrom(
                    src => src.UserRoles != null && src.UserRoles.Any() ? src.UserRoles.First().Role.Name : null
                ));

            CreateMap<CreateUserDto, User>();

            // User profile mappings
            CreateMap<UserProfile, ProfileDto>();

            CreateMap<UpdateProfileDto, UserProfile>()
                .ForAllMembers(opt => opt.Condition((src, dest, value) => value != null));

            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateProjectDto, Project>();

            CreateMap<ProjectUpdate, ProjectUpdateDto>()
                .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateProjectUpdateDto, ProjectUpdate>();

            CreateMap<Notification, NotificationDto>();
        }
    }
}