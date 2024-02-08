using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ModelDataBase.Model
{
    public partial class dbModel : DbContext
    {
        public dbModel()
            : base("name=dbModel")
        {
        }

        public virtual DbSet<DiseaseHistory> DiseaseHistory { get; set; }
        public virtual DbSet<MedicalCard> MedicalCard { get; set; }
        public virtual DbSet<Patient> Patient { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .HasMany(e => e.DiseaseHistory)
                .WithRequired(e => e.Patient)
                .HasForeignKey(e => e.IDPatient)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Patient>()
                .HasMany(e => e.MedicalCard)
                .WithRequired(e => e.Patient)
                .HasForeignKey(e => e.IDPatient)
                .WillCascadeOnDelete(false);
        }
    }
}
