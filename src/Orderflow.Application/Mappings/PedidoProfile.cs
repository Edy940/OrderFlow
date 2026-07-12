using AutoMapper;
using OrderFlow.Application.DTO;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Mappings
{
    public class PedidoProfile : Profile
    {
        public PedidoProfile()
        {
            CreateMap<Cliente, ClienteResponseDto>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
                
            CreateMap<Pedido, PedidoResponseDto>()
                .ForMember(dest => dest.Cliente, opt => opt.MapFrom(src => src.Cliente.Nome))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.ValorTotal));

            CreateMap<ItemPedido, ItemPedidoResponseDto>()
                .ForMember(dest => dest.Produto, opt => opt.MapFrom(src => src.Produto.Nome))
                .ForMember(dest => dest.PrecoUnitario, opt => opt.MapFrom(src => src.PrecoUnitario))
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.CalcularValorTotal()));
        }
    }
}