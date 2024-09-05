using InMemoryCRUDEmployeeOperationDhiki.Models;
using Microsoft.EntityFrameworkCore;

namespace InMemoryCRUDEmployeeOperationDhiki.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        
        //Membuat DbSet Employee
        public DbSet<Employee> Employees { get; set; }
        
        //Mengkonfigurasi model Employee
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .Property(e => e.EmployeeId)
                .HasMaxLength(10);

            modelBuilder.Entity<Employee>()
                .Property(e => e.FullName)
                .HasMaxLength(50);
        }
    }
}