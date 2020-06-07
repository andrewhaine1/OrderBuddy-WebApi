using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ord.WebApi.Entities.Shared
{
    public class RestaurantCollectionType : BaseEntity
    {
        [ForeignKey("RestaurantId")]
        public Restaurant Restaurant { get; set; }

        [Required]
        public int RestaurantId { get; set; }

        [ForeignKey("CollectionTypeId")]
        public CollectionType CollectionType { get; set; }

        [Required]
        public int CollectionTypeId { get; set; }
    }
}
