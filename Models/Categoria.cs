using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShopDemo.Models
{
    [Table("t_categoria")]
    public class Categoria
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int ID{ get; set; }

        [Column("name")]
        public String Name { get; set; }

        [Column("mini_preview")]
        public byte[] Preview { get; set; }

        [Column("banner")]
        public byte[] Banner { get; set; }

        [NotMapped]
        public String imageData { get; set; }




    }
}