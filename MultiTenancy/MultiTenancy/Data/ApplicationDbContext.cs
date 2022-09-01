using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Entities;
using MultiTenancy.Entities.Interfaces;
using MultiTenancy.Services;
using MultiTenancy.Services.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace MultiTenancy.Data {
    public class ApplicationDbContext : IdentityDbContext {
        private string _tenantId;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ITenantService tenantService
        ) : base(options) {
            _tenantId = tenantService.GetTenant();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            foreach (var item in ChangeTracker.Entries().Where(e => e.State == EntityState.Added && e.Entity is ITenantEntitie)) {
                if (string.IsNullOrEmpty(_tenantId)) {
                    throw new Exception("The TenantId was not found to the moment of create the register");
                }

                var entity = item.Entity as ITenantEntitie;
                entity!.TenantId = _tenantId;
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<Country>().HasData(new Country[] {
                new Country { Id = 1, Name = "México" },
                new Country { Id = 2, Name = "Colombia" },
                new Country { Id = 3, Name = "Estados Unidos" }
            });

            foreach (var entitie in builder.Model.GetEntityTypes()) {
                var type = entitie.ClrType;

                if (typeof(ITenantEntitie).IsAssignableFrom(type)) {
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(MakeGlobalFilterTenant), 
                        BindingFlags.NonPublic | BindingFlags.Static
                    )?.MakeGenericMethod(type);

                    var filter = method?.Invoke(null, new object[] { this })!;

                    entitie.SetQueryFilter((LambdaExpression)filter);
                    entitie.AddIndex(entitie.FindProperty(nameof(ITenantEntitie.TenantId)));
                } else if (type.ShouldSkipTenantValidation()) {
                    continue;
                } else {
                    throw new Exception($"The entitie { entitie } has not marked as tenant or common");
                }
            }
        }

        private static LambdaExpression MakeGlobalFilterTenant<TEntitie>(
            ApplicationDbContext context
        ) where TEntitie : class, ITenantEntitie {
            Expression<Func<TEntitie, bool>> filter =
                t => t.TenantId == context._tenantId;

            return filter;
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Country> Countries => Set<Country>();
    }
}