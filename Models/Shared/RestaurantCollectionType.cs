using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ord.WebApi.Models.Shared
{
    public class RestaurantCollectionType
    {
	public int Id { get; set; }	

        [Required]
        public int RestaurantId { get; set; }

        [Required]
        public int CollectionTypeId { get; set; }

        public string Name { get; set; }
    }
}
