using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace KoruCosmetica.Models
{
	public class DiasSemana
	{
        [Key]
        public int DiaID { get; set; }
        public string Nombre {  get; set; } 
    }
}