using System.ComponentModel.DataAnnotations;

namespace Ord.WebApi.Models.Shared
{
    public class CollectionType
    {
	public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
