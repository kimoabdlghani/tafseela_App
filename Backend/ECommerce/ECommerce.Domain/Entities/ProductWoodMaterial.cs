namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// Join table — defines which WoodMaterials are allowed for a Product.
    /// Product N:N WoodMaterial via this entity.
    /// </summary>
    public class ProductWoodMaterial
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int WoodMaterialId { get; set; }
        public WoodMaterial WoodMaterial { get; set; } = null!;
    }
}
