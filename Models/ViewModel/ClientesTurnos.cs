namespace KoruCosmetica.Models.ViewModel
{
    public class ClientesTurnos
    {
        public int ID {  get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateOnly Fecha { get; set; }
        public TimeOnly Hora { get; set; }
        public string Servicio { get; set; }

    }
}
