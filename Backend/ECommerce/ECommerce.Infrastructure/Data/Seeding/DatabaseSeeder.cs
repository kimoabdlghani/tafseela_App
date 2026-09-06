using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAllAsync(
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        // 1. Seed Roles
        string[] roles = ["Admin", "Carpenter", "Customer"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int> { Name = role });
            }
        }

        // 2. Seed Default Admin User
        const string adminEmail = "Moamen@ecommerce.com";
        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin == null)
        {
            var adminUser = new User
            {
                FirstName = "Moamen",
                LastName = "Mohamed",
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Moamen@123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // 3. Seed Default Company Settings
        if (!await context.CompanySettings.AnyAsync())
        {
            context.CompanySettings.Add(new CompanySettings
            {
                CarpenterPercentage = 0.30m,     // 30%
                CompanyProfitPercentage = 0.20m, // 20%
                DepositPercentage = 0.30m,       // 30%
                EffectiveFrom = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        // 4. Seed Categories
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new Category { Name = "دواليب وخزائن", Description = "دواليب ملابس وخزائن حائط مخصصة حسب المقاس", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Category { Name = "غرف نوم", Description = "أسرة وكومودينو وتسريحات غرف النوم الفاخرة", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Category { Name = "مكاتب وطاولات", Description = "مكاتب عمل دراسية وطاولات اجتماعات وتلفزيون", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Category { Name = "مطابخ ووحدات تخزين", Description = "وحدات مطبخ وخزائن أواني مصممة خصيصاً", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // 5. Seed Wood Materials and Colors
        if (!await context.WoodMaterials.AnyAsync())
        {
            var mdf18 = new WoodMaterial
            {
                Name = "MDF 18mm إسباني",
                Thickness = 1.8m,
                Unit = "لوح",
                UnitPrice = 1200m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Colors = new List<WoodColor>
                {
                    new WoodColor { Name = "أبيض صريح مط", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new WoodColor { Name = "رمادي مودرن داكن", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new WoodColor { Name = "بيج دافئ", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new WoodColor { Name = "أسود فاحم", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                }
            };

            var mdf25 = new WoodMaterial
            {
                Name = "MDF 25mm تركي مدعم",
                Thickness = 2.5m,
                Unit = "لوح",
                UnitPrice = 1600m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Colors = new List<WoodColor>
                {
                    new WoodColor { Name = "خشب بلوط طبيعي فاتح", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new WoodColor { Name = "خشب جوزي ملكي", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new WoodColor { Name = "أبيض هاي جلوس لامع", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                }
            };

            var beech = new WoodMaterial
            {
                Name = "خشب زان روماني طبيعي",
                Thickness = 5.0m,
                Unit = "متر مكعب",
                UnitPrice = 3500m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Colors = new List<WoodColor>
                {
                    new WoodColor { Name = "زان طبيعي مدهون ورنيش", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new WoodColor { Name = "بني استر محروق", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new WoodColor { Name = "عسلي كلاسيكي", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                }
            };

            context.WoodMaterials.AddRange(mdf18, mdf25, beech);
            await context.SaveChangesAsync();
        }

        // 6. Seed Components (Hardware)
        if (!await context.Components.AnyAsync())
        {
            var components = new List<Component>
            {
                new Component { Name = "مفصلة هيدروليك بلوم إيطالي", Type = ComponentType.Hinge, Unit = "قطعة", UnitPrice = 65m, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Component { Name = "مجري درج تلسكوبي سوفت كلوز 45 سم", Type = ComponentType.DrawerSlide, Unit = "طقم", UnitPrice = 180m, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Component { Name = "مقبض ألومنيوم مودرن غاطس 20 سم", Type = ComponentType.Handle, Unit = "قطعة", UnitPrice = 50m, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Component { Name = "أرجل معدنية مودرن ستانلس 12 سم", Type = ComponentType.Other, Unit = "قطعة", UnitPrice = 85m, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            context.Components.AddRange(components);
            await context.SaveChangesAsync();
        }

        // 7. Seed Sample Parts
        if (!await context.Parts.AnyAsync())
        {
            var parts = new List<Part>
            {
                new Part { Name = "لوح جانبي أيمن وأيسر", Description = "الألواح الرأسية الحاملة للهيكل", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Part { Name = "أرفف داخلية متحركة وثابتة", Description = "أرفف التخزين الداخلية", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Part { Name = "دلفة باب رئيسية", Description = "أبواب الخزانة الخارجية", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Part { Name = "واجهة درج وجوانب درج", Description = "صناديق الأدراج السفلية", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Part { Name = "لوح ظهر 3 مم", Description = "اللوح الخلفي للإغلاق والحماية", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            context.Parts.AddRange(parts);
            await context.SaveChangesAsync();
        }

        // 8. Seed Sample Products
        if (!await context.Products.AnyAsync())
        {
            var wardrobeCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name.Contains("دواليب"));
            var deskCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name.Contains("مكاتب"));
            var mdf18 = await context.WoodMaterials.FirstOrDefaultAsync(w => w.Name.Contains("18mm"));
            var mdf25 = await context.WoodMaterials.FirstOrDefaultAsync(w => w.Name.Contains("25mm"));
            var hinge = await context.Components.FirstOrDefaultAsync(c => c.Type == ComponentType.Hinge);
            var slide = await context.Components.FirstOrDefaultAsync(c => c.Type == ComponentType.DrawerSlide);
            var handle = await context.Components.FirstOrDefaultAsync(c => c.Type == ComponentType.Handle);

            if (wardrobeCategory != null && mdf18 != null)
            {
                var wardrobe = new Product
                {
                    CategoryId = wardrobeCategory.Id,
                    Name = "دولاب ملابس مودرن 3 ضلفة قابل للتفصيل",
                    Description = "دولاب ملابس فندقي فاخر مصنوع من خامات عالية الجودة، متاح بتفصيل المقاسات واللون حسب مساحة غرفتك.",
                    Status = ProductStatus.Published,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Dimensions = new List<ProductDimension>
                    {
                        new ProductDimension { Name = "العرض (Width)", Unit = "سم", MinValue = 160m, MaxValue = 300m, DefaultValue = 240m, DisplayOrder = 1 },
                        new ProductDimension { Name = "الارتفاع (Height)", Unit = "سم", MinValue = 180m, MaxValue = 270m, DefaultValue = 220m, DisplayOrder = 2 },
                        new ProductDimension { Name = "العمق (Depth)", Unit = "سم", MinValue = 50m, MaxValue = 75m, DefaultValue = 60m, DisplayOrder = 3 }
                    },
                    ProductWoodMaterials = new List<ProductWoodMaterial>
                    {
                        new ProductWoodMaterial { WoodMaterialId = mdf18.Id },
                        new ProductWoodMaterial { WoodMaterialId = mdf25!.Id }
                    },
                    ProductComponents = new List<ProductComponent>()
                };

                if (hinge != null) wardrobe.ProductComponents.Add(new ProductComponent { ComponentId = hinge.Id, Quantity = 6 });
                if (slide != null) wardrobe.ProductComponents.Add(new ProductComponent { ComponentId = slide.Id, Quantity = 2 });
                if (handle != null) wardrobe.ProductComponents.Add(new ProductComponent { ComponentId = handle.Id, Quantity = 3 });

                context.Products.Add(wardrobe);
            }

            if (deskCategory != null && mdf18 != null)
            {
                var desk = new Product
                {
                    CategoryId = deskCategory.Id,
                    Name = "مكتب دراسي وعملي مودرن مع وحدات أدراج",
                    Description = "مكتب عملي مصمم لغرف المذاكرة والمكاتب المنزلية بمقاسات مرنة وتصميم مريح.",
                    Status = ProductStatus.Published,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Dimensions = new List<ProductDimension>
                    {
                        new ProductDimension { Name = "العرض (Width)", Unit = "سم", MinValue = 100m, MaxValue = 180m, DefaultValue = 140m, DisplayOrder = 1 },
                        new ProductDimension { Name = "الارتفاع (Height)", Unit = "سم", MinValue = 70m, MaxValue = 85m, DefaultValue = 75m, DisplayOrder = 2 },
                        new ProductDimension { Name = "العمق (Depth)", Unit = "سم", MinValue = 50m, MaxValue = 80m, DefaultValue = 60m, DisplayOrder = 3 }
                    },
                    ProductWoodMaterials = new List<ProductWoodMaterial>
                    {
                        new ProductWoodMaterial { WoodMaterialId = mdf18.Id }
                    },
                    ProductComponents = new List<ProductComponent>()
                };

                if (slide != null) desk.ProductComponents.Add(new ProductComponent { ComponentId = slide.Id, Quantity = 3 });
                if (handle != null) desk.ProductComponents.Add(new ProductComponent { ComponentId = handle.Id, Quantity = 3 });

                context.Products.Add(desk);
            }

            await context.SaveChangesAsync();
        }
    }
}
