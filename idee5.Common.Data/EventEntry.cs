using System;
using System.ComponentModel.DataAnnotations;

namespace idee5.Common.Data {
    /// <summary>
    /// Event store row definition
    /// </summary>
    public class EventEntry {
        /// <summary>
        /// Clustering index.
        /// </summary>
        [Key]
        public int Index { get; set; }

        /// <summary>
        /// Id of the aggregate root.
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Version of the aggregate root
        /// </summary>
        [Required]
        public int Version { get; set; }

        /// <summary>
        /// Serialized event data.
        /// </summary>
        [MaxLength]
        public string Data { get; set; } = "";

        /// <summary>
        /// Instant the event was created.
        /// </summary>
        [Required]
        public DateTimeOffset TimeStamp { get; set; }

        /// <summary>
        /// Name of the event
        /// </summary>
        [Required]
        public string EventName { get; set; } = "";
    }

}
