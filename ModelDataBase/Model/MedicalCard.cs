namespace ModelDataBase.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MedicalCard")]
    public partial class MedicalCard
    {
        public int ID { get; set; }

        public int IDPatient { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [StringLength(150)]
        public string PathQRCode { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
