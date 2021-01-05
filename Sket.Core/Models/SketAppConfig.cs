using MongoDB.Entities;

namespace Sket.Core.Models
{
    [Name("App")]
    public class SketAppConfig: Entity
    {
        public string Jwt { get; set; }
    }
}