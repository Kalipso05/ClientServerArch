namespace ModelDataBase.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DiseaseHistory")]
    public partial class DiseaseHistory
    {
        public int ID { get; set; }

        public int IDPatient { get; set; }

        [Required]
        [StringLength(50)]
        public string NameDisease { get; set; }

        [Required]
        [StringLength(250)]
        public string Description { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateOfDisease { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
