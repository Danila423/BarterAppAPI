using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarterAppAPI.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }

        // Связь с таблицей Users
        public int UserId { get; set; }

        // Связь с таблицей Listings
        public int ListingId { get; set; }

        public Listing Listing { get; set; }
    }
}
