using AutoMapper;
using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Domain.Entities;

namespace ConstructionManager.Api.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Customer
            CreateMap<Customer, CustomerDto>().ReverseMap();
            #endregion

            #region Employee
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            #endregion

            #region Equipment
            CreateMap<Equipment, EquipmentDto>().ReverseMap();
            #endregion

            #region Supplier
            CreateMap<Supplier, SupplierDto>().ReverseMap();
            #endregion

            #region Project
            CreateMap<Project, ProjectDto>().ReverseMap();
            #endregion

            #region Invoice
            CreateMap<Invoice, InvoiceDto>().ReverseMap();
            #endregion

            #region PurchaseInvoice
            CreateMap<PurchaseInvoice, PurchaseInvoiceDto>().ReverseMap();
            #endregion

            #region PurchaseInvoiceDetail
            CreateMap<PurchaseInvoiceDetail, PurchaseInvoiceDetailDto>().ReverseMap();
            #endregion

            #region Payment
            CreateMap<Payment, PaymentDto>().ReverseMap();
            #endregion

            #region ProjectAssignment
            CreateMap<ProjectAssignment, ProjectAssignmentDto>().ReverseMap();
            #endregion

            #region Material
            CreateMap<Material, MaterialDto>().ReverseMap();
            #endregion

            #region User
            CreateMap<User, UserDto>()
            .ForMember(dest => dest.Password, opt => opt.Ignore());

            #endregion
        }
    }
}