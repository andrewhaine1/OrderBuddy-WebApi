using System.ComponentModel.DataAnnotations;

namespace Ord.WebApi.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
