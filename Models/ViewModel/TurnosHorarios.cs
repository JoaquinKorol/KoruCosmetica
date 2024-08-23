namespace KoruCosmetica.Models.ViewModel
{
    public class TurnosHorarios
    {
        public Turnos Turnos {  get; set; }
        public HorariosDisponibles? HorariosDisponibles { get; set; }
        public DiasSemana? DiasSemana { get; set; }

        public static int GetDiaIDFromDayOfWeek(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday: return 1; // Lunes
                case DayOfWeek.Tuesday: return 2; // Martes
                case DayOfWeek.Wednesday: return 3; // Miércoles
                case DayOfWeek.Thursday: return 4; // Jueves
                case DayOfWeek.Friday: return 5; // Viernes
                case DayOfWeek.Saturday: return 6; // Sábado
                case DayOfWeek.Sunday: return 7; // Domingo
                default: return 1;
            }
        }


    }
}
