using System;
using System.Collections.Generic;
using AutoMapper;
using USARoadTrip.Api.EntityModels;
using USARoadTrip.Api.Models;

namespace USARoadTrip.Api.Extensions
{
    /* 
     * EXAMPLES TO SHOW SOME COMMON MAPPING CONFIGURATIONS
     * 
     * To convert from integer to enumerated
       Mapper.CreateMap<EnumClass, Int16>().ConvertUsing(os => (Int16)os);
       Mapper.CreateMap<Int16, EnumClass>().ConvertUsing(s => (EnumClass)s);
     * 
     * To ignore properties that you don't want to map
       Mapper.CreateMap<Model, Entity>().ForMember(model => model.Property, mo => mo.Ignore())
     * 
    */

    public static class MappingExtensions
    {
        private static bool _initialized = false;

        private static void InitializeMappings()
        {
            if (_initialized)
                return;

            InitializeUserMapping();
            InitializeTripMapping();
            InitializeLocationMapping();

            _initialized = true;
        }


        private static void InitializeUserMapping()
        {
            Mapper.CreateMap<User, UserEntity>();
            Mapper.CreateMap<UserEntity, User>();
        }

        private static void InitializeTripMapping()
        {
            Mapper.CreateMap<Trip, TripEntity>()
                .ForMember(model => model.Destinations, mo => mo.Ignore());
            Mapper.CreateMap<TripEntity, Trip>()
                .ForMember(model => model.Destinations, mo => mo.Ignore());
        }

        private static void InitializeLocationMapping()
        {
            Mapper.CreateMap<Location, LocationEntity>();
            Mapper.CreateMap<LocationEntity, Location>();
        }

        public static E ToEntity<M, E>(this M model, E entity = null) where E : class, new()
        {
            InitializeMappings();
            entity = entity ?? new E();
            entity = Mapper.Map(model, entity);
            return entity;
        }

        public static M ToModel<E, M>(this E entity) where M : class, new()
        {
            InitializeMappings();
            M model = new M();
            model = Mapper.Map(entity, model);
            return model;
        }

        public static IList<M> ToModels<E, M>(this IEnumerable<E> entities) where M : class, new()
        {
            InitializeMappings();
            List<M> models = new List<M>();
            return Mapper.Map(entities, models);
        }
    }
}
