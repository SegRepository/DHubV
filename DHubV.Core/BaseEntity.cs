using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHubV.Core
{
    public abstract class BaseEntity<T> : BaseGeneralModel
    {
        [Key, Column("Id", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public virtual T Id { get; set; }
    }

    public abstract class BaseGeneralModel
    {
        [Column(TypeName = "varchar(50)")]
        public string? CreatedBy { get; set; } = string.Empty; //RecordUserId.Empty;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [Column(TypeName = "varchar(50)")]
        public string? UpdatedBy { get; set; } = string.Empty;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        [DefaultValue(false)]
        public bool IsDeleted { get; set; } = false;
    }
}
