using AutoMapper;
using Microsoft.AspNetCore.Http;
using Ord.WebApi.Helpers.Paging;
using System.Collections.Generic;

namespace Ord.WebApi.Mappers.Restaurant
{
    public class RestaurantMapper : IRestaurantMapper
    {
        private readonly HttpContext _httpContext;

        public RestaurantMapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public PagedList<Models.Restaurant> RestaurantEntitiesToModels(PagedList<Entities.Restaurant> restaurants)
        {
            return new MapperConfiguration(cfg => cfg.CreateMap<Entities.Restaurant,
                Models.Restaurant>()
                .ForMember(dest => dest.RestaurantImagePath, opt =>
                opt.MapFrom(src => $"{GetBaseUrl()}/{src.RestaurantImagePath}"))
                .ForMember(dest => dest.RestaurantCoverImagePath, opt =>
                opt.MapFrom(src => $"{GetBaseUrl()}/{src.RestaurantCoverImagePath}"))
                .ForMember(dest => dest.CollectionTypes, opt =>
                opt.MapFrom(src => RestaurantCollectionTypeEntitiesToModels(src.CollectionTypes)))
                .ForMember(dest => dest.PaymentMethods, opt =>
                opt.MapFrom(src => RestaurantPaymentMethodEntitiesToModels(src.PaymentMethods))))
                .CreateMapper()
                .Map<PagedList<Models.Restaurant>>(restaurants);
        }

        public IEnumerable<Models.Restaurant> RestaurantEntitiesToModels(IEnumerable<Entities.Restaurant> restaurants)
        {
            return new MapperConfiguration(cfg => cfg.CreateMap<Entities.Restaurant,
                Models.Restaurant>()
                .ForMember(dest => dest.RestaurantImagePath, opt =>
                opt.MapFrom(src => $"{GetBaseUrl()}/{src.RestaurantImagePath}"))
                .ForMember(dest => dest.RestaurantCoverImagePath, opt =>
                opt.MapFrom(src => $"{GetBaseUrl()}/{src.RestaurantCoverImagePath}"))
                .ForMember(dest => dest.CollectionTypes, opt =>
                opt.MapFrom(src => RestaurantCollectionTypeEntitiesToModels(src.CollectionTypes)))
                .ForMember(dest => dest.PaymentMethods, opt =>
                opt.MapFrom(src => RestaurantPaymentMethodEntitiesToModels(src.PaymentMethods))))
                .CreateMapper()
                .Map<IEnumerable<Models.Restaurant>>(restaurants);
        }

        public Models.Restaurant RestaurantEntityToModel(Entities.Restaurant restaurant)
        {
            return new MapperConfiguration(cfg => cfg.CreateMap<Entities.Restaurant, 
                Models.Restaurant>()
                .ForMember(dest => dest.RestaurantImagePath, opt => 
                opt.MapFrom(src => $"{GetBaseUrl()}/{src.RestaurantImagePath}"))
                .ForMember(dest => dest.RestaurantCoverImagePath, opt =>
                opt.MapFrom(src => $"{GetBaseUrl()}/{src.RestaurantCoverImagePath}"))
                .ForMember(dest => dest.CollectionTypes, opt =>
                opt.MapFrom(src => RestaurantCollectionTypeEntitiesToModels(src.CollectionTypes)))
                .ForMember(dest => dest.PaymentMethods, opt =>
                opt.MapFrom(src => RestaurantPaymentMethodEntitiesToModels(src.PaymentMethods))))
                .CreateMapper()
                .Map<Models.Restaurant>(restaurant);
        }

        public Entities.Restaurant RestaurantModelToEntity(Models.Restaurant restaurant)
        {
            return new MapperConfiguration(cfg => cfg.CreateMap<Models.Restaurant, 
                Entities.Restaurant>()
                .ForMember(dest => dest.OrdUser, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceArea, opt => opt.Ignore()))
                .CreateMapper()
                .Map<Entities.Restaurant>(restaurant);
        }

        public void MapRestaurantModelToEntity(Models.Restaurant restaurantMod, 
            Entities.Restaurant restaurantEnt)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.Restaurant, Entities.Restaurant>())
                .CreateMapper();

            mapper.Map(restaurantMod, restaurantEnt);
        }

        public string GetBaseUrl()
        {
            var request = _httpContext.Request;

            var host = request.Host.ToUriComponent();

            var pathBase = request.PathBase.ToUriComponent();

            return $"{request.Scheme}s://{host}{pathBase}";
        }


        /* ------------------------------------- Restaurant Collection Types -------------------------------------- */
        public IEnumerable<Models.Shared.RestaurantCollectionType> RestaurantCollectionTypeEntitiesToModels(
            IEnumerable<Entities.Shared.RestaurantCollectionType> restaurantCollectionTypes)
            => new MapperConfiguration(cfg => cfg.CreateMap<Entities.Shared.RestaurantCollectionType,
                Models.Shared.RestaurantCollectionType>()
                .ForMember(c => c.Name, opt => opt.MapFrom(src => 
                src.CollectionType.Name)))
                .CreateMapper()
                .Map<IEnumerable<Models.Shared.RestaurantCollectionType>>(restaurantCollectionTypes);

        public IEnumerable<Entities.Shared.RestaurantCollectionType> RestaurantCollectionTypeModelsToEntities(
            IEnumerable<Models.Shared.RestaurantCollectionType> restaurantCollectionTypes)
            => new MapperConfiguration(cfg => cfg.CreateMap<Models.Shared.RestaurantCollectionType,
                Entities.Shared.RestaurantCollectionType>())
                .CreateMapper()
                .Map<IEnumerable<Entities.Shared.RestaurantCollectionType>>(restaurantCollectionTypes);

        public Models.Shared.RestaurantCollectionType RestaurantCollectionTypeEntityToModel(
            Entities.Shared.RestaurantCollectionType restaurantCollectionType)
            => new MapperConfiguration(cfg => cfg.CreateMap<Entities.Shared.RestaurantCollectionType,
                Models.Shared.RestaurantCollectionType>()
                .ForMember(c => c.Name, opt => opt.MapFrom(src =>
                src.CollectionType.Name)))
                .CreateMapper()
                .Map<Models.Shared.RestaurantCollectionType>(restaurantCollectionType);

        public void MapOrderModelToEntity(Models.Shared.RestaurantCollectionType restaurantCollectionTypeMod,
            Entities.Shared.RestaurantCollectionType restaurantCollectionTypeEnt)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.Shared.RestaurantCollectionType,
                Entities.Shared.RestaurantCollectionType>())
                .CreateMapper();

            mapper.Map(restaurantCollectionTypeMod, restaurantCollectionTypeEnt);
        }


        /* ------------------------------------- Restaurant Payment Methods -------------------------------------- */
        public IEnumerable<Models.Shared.RestaurantPaymentMethod> RestaurantPaymentMethodEntitiesToModels(
            IEnumerable<Entities.Shared.RestaurantPaymentMethod> restaurantPaymentMethods)
            => new MapperConfiguration(cfg => cfg.CreateMap<Entities.Shared.RestaurantPaymentMethod,
                Models.Shared.RestaurantPaymentMethod>()
                .ForMember(c => c.Name, opt => opt.MapFrom(src =>
                src.PaymentMethod.Name)))
                .CreateMapper()
                .Map<IEnumerable<Models.Shared.RestaurantPaymentMethod>>(restaurantPaymentMethods);

        public IEnumerable<Entities.Shared.RestaurantPaymentMethod> RestaurantPaymentMethodModelsToEntities(
            IEnumerable<Models.Shared.RestaurantPaymentMethod> restaurantPaymentMethods)
            => new MapperConfiguration(cfg => cfg.CreateMap<Models.Shared.RestaurantPaymentMethod,
                Entities.Shared.RestaurantPaymentMethod>())
                .CreateMapper()
                .Map<IEnumerable<Entities.Shared.RestaurantPaymentMethod>>(restaurantPaymentMethods);

        public Models.Shared.RestaurantPaymentMethod RestaurantPaymentMethodEntityToModel(
            Entities.Shared.RestaurantPaymentMethod restaurantPaymentMethod)
            => new MapperConfiguration(cfg => cfg.CreateMap<Entities.Shared.RestaurantPaymentMethod,
                Models.Shared.RestaurantPaymentMethod>()
                .ForMember(c => c.Name, opt => opt.MapFrom(src =>
                src.PaymentMethod.Name)))
                .CreateMapper()
                .Map<Models.Shared.RestaurantPaymentMethod>(restaurantPaymentMethod);

        public void MapRestaurantPaymentMethodModelToEntity(Models.Shared.RestaurantPaymentMethod restaurantPaymentMethodMod, 
            Entities.Shared.RestaurantPaymentMethod restaurantPaymentMethodEnt)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.Shared.RestaurantPaymentMethod,
                Entities.Shared.RestaurantPaymentMethod>())
                .CreateMapper();

            mapper.Map(restaurantPaymentMethodMod, restaurantPaymentMethodEnt);
        }
    }
}
