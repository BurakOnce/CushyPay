using AutoMapper;
using CushyPay.Application.Common.DTOs;
using CushyPay.Domain.Entities;

namespace CushyPay.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ReverseMap();

        CreateMap<Wallet, WalletDto>()
            .ReverseMap();

        CreateMap<Transaction, TransactionDto>()
            .ReverseMap();
    }
}
