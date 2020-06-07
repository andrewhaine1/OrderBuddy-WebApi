using System.ComponentModel.DataAnnotations;

namespace Ord.WebApi.Entities.Shared
{
    public class CollectionType : BaseEntity
    {
        [Required]
        public string Name { get; set; }
    }
}
