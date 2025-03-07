using AutoMapper;
using Models.Emergencies;
using Models.User;

namespace Models.Mapper
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            // User Registration Mappings
            CreateMap<RegisterationRequestDTO, LocalUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id (set by Identity)
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.SSN, opt => opt.MapFrom(src => src.SSN))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore()) // Set programmatically
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Set by UserManager

            CreateMap<RegisterationUserRequestDTO, LocalUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.SSN, opt => opt.MapFrom(src => src.SSN))
                .ForMember(dest => dest.ImageProfile, opt => opt.MapFrom(src => src.ImageProfile))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            // LocalUser to DTO (optional for responses if needed)
            CreateMap<LocalUser, RegisterationUserRequestDTO>()
                .ForMember(dest => dest.Password, opt => opt.Ignore()) // Don’t expose password
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore());

            // Emergency Mappings
            CreateMap<EmergPerson, EmergPersonDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.LocalUser != null ? src.LocalUser.UserName : null))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<EmergPersonDTO, EmergPerson>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id for creation
                .ForMember(dest => dest.SendAt, opt => opt.Ignore()) // Set programmatically
                .ForMember(dest => dest.Status, opt => opt.Ignore()) // Set programmatically
                .ForMember(dest => dest.LocalUser, opt => opt.Ignore()) // Handled by UserId
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<EmergAnother, EmergAnotherDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.LocalUser != null ? src.LocalUser.UserName : null))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<EmergAnotherDTO, EmergAnother>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SendAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.LocalUser, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
        }
    }
}