using System.Linq;
using AutoMapper;
using HomeCinema.Entities;
using HomeCinema.Web.Models;

namespace HomeCinema.Web.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DomainToViewModelMappings"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<Movie, MovieViewModel>()
                .ForMember(vm => vm.Genre, map => map.MapFrom(m => m.Genre.Name))
                .ForMember(vm => vm.GenreId, map => map.MapFrom(m => m.Genre.ID))
                .ForMember(vm => vm.IsAvailable, map => map.MapFrom(m => m.Stocks.Any(s => s.IsAvailable)))
                .ForMember(vm => vm.NumberOfStocks, map => map.MapFrom(m => m.Stocks.Count))
                .ForMember(vm => vm.Image, map => map.MapFrom(m => string.IsNullOrEmpty(m.Image) == true ? "unknown.jpg" : m.Image));

            Mapper.CreateMap<Genre, GenreViewModel>()
                .ForMember(vm => vm.NumberOfMovies, map => map.MapFrom(g => g.Movies.Count));
            // code omitted
            Mapper.CreateMap<Customer, CustomerViewModel>();

            Mapper.CreateMap<Stock, StockViewModel>();

            Mapper.CreateMap<Rental, RentalViewModel>();

            Mapper.CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.Id, map => map.MapFrom(src => src.ID))
                .ForMember(dest => dest.Username, map => map.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, map => map.MapFrom(src => src.Email))
                .ForMember(dest => dest.IsLocked, map => map.MapFrom(src => src.IsLocked))
                .ReverseMap()
                .ForMember(dest => dest.ID, map => map.MapFrom(src => src.Id))
                ;

            Mapper.CreateMap<Claim, ClaimViewModel>()
                  .ForMember(dest => dest.Id, map => map.MapFrom(src => src.ID))
                  .ForMember(dest => dest.ClaimType, map => map.MapFrom(src => src.ClaimType))
                  .ForMember(dest => dest.ClaimValue, map => map.MapFrom(src => src.ClaimValue))
                  .ForMember(dest => dest.Description, map => map.MapFrom(src => src.Description))
                  ;

            Mapper.CreateMap<User, UserDetailViewModel>()
                .IncludeBase<User, UserViewModel>()
                .ForMember(dest => dest.Roles, map => map.MapFrom(src => src.UserRoles.Select(ur => ur.Role)))
                .ForMember(dest => dest.Claims, map => map.MapFrom(src => src.UserRoles.Select(ur => ur.Role).SelectMany(r => r.RoleClaims.Select(rc => rc.Claim)).Distinct())) // Distinct, in case we have overlaps in Role-Claim assignment
                ;

            Mapper.CreateMap<UserDetailViewModel, User>()
               .IncludeBase<UserViewModel, User>()
               .ForMember(dest => dest.UserRoles, map => map.MapFrom(src => src.Roles.Select(r => new UserRole { RoleId = r.Id })))
               ;

            Mapper.CreateMap<Role, RoleViewModel>()
                .ForMember(dest => dest.Id, map => map.MapFrom(src => src.ID))
                .ForMember(dest => dest.Name, map => map.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, map => map.MapFrom(src => src.Description))
                ;
        }
    }
}