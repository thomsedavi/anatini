using System.ComponentModel.DataAnnotations;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Events
{
    public class CreateEvent
    {
        [MaxLength(256)]
        public required string Name { get; set; }

        [Handle(nullable: true), MaxLength(256)]
        public string? Handle { get; set; }

        [EnumDataType(typeof(Visibility))]
        public required Visibility Visibility { get; set; }

        [Display(Name = "Starts At (NZ)")]
        public required DateTime StartsAtNz { get; set; }

        [Display(Name = "Ends At (NZ)")]
        public DateTime? EndsAtNz { get; set; }

        public TimeSpan? Duration { get; set; }

        [MaxLength(256)]
        public string? Url { get; set; }

        public string? Article { get; set; }

        [Display(Name = "Is Draft")]
        public bool? IsDraft { get; set; }

        [RecurrenceRule]
        public string? RecurrenceRule { get; set; }
    }
}
