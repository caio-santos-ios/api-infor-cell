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
            CreateMap<CreatePlanDTO, Plan>().ReverseMap();
            CreateMap<UpdatePlanDTO, Plan>().ReverseMap();

            CreateMap<CreateCompanyDTO, Company>().ReverseMap();
            CreateMap<UpdateCompanyDTO, Company>().ReverseMap();
            
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

            CreateMap<CreateStoreDTO, Store>().ReverseMap();
            CreateMap<UpdateStoreDTO, Store>().ReverseMap();

            CreateMap<CreateBrandDTO, Brand>().ReverseMap();
            CreateMap<UpdateBrandDTO, Brand>().ReverseMap();

            CreateMap<CreateProductDTO, Product>().ReverseMap();
            CreateMap<UpdateProductDTO, Product>().ReverseMap();

            #endregion
        }
    }
}