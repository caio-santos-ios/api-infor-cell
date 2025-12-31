using AutoMapper;
using api_infor_cell.src.Models;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Configuration
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            #region MASTER DATA
            CreateMap<CreateGenericTableDTO, GenericTable>().ReverseMap();
            CreateMap<UpdateGenericTableDTO, GenericTable>().ReverseMap();
            
            CreateMap<CreateAddressDTO, Address>().ReverseMap();
            CreateMap<UpdateAddressDTO, Address>().ReverseMap();
            
            CreateMap<CreateContactDTO, Contact>().ReverseMap();
            CreateMap<UpdateContactDTO, Contact>().ReverseMap();

            CreateMap<CreateAttachmentDTO, Attachment>().ReverseMap();
            CreateMap<UpdateAttachmentDTO, Attachment>().ReverseMap();      
           
            CreateMap<CreateSupplierDTO, Supplier>().ReverseMap();
            CreateMap<UpdateSupplierDTO, Supplier>().ReverseMap();      
            #endregion
        }
    }
}