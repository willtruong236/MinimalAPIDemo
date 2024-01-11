﻿using AutoMapper;
using CouponAPI.Models;
using CouponAPI.Models.DTO;

namespace CouponAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Coupon, CouponDTO>().ReverseMap();
        }
    }
}
