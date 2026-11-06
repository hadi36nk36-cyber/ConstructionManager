using ConstructionManager.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManager.Api.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    #region DbSets
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
    public DbSet<PurchaseInvoiceDetail> PurchaseInvoiceDetails { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<ProjectAssignment> ProjectAssignments { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<User> Users { get; set; }
    #endregion

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}