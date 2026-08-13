using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Entities
{
    public class Measurement : BaseEntity, ISoftDeletable
    {
        public string Unit { get; set; }
        public decimal Length { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public Product Product { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
