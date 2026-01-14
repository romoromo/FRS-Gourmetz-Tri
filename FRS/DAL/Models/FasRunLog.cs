using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class FasRunLog
    {
        [Key]
        public long Id { get; set; }
        public int ClassLevelId { get; set; }
        public string RunKey { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
