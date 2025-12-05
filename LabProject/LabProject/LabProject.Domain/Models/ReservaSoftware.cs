namespace LabProject.Domain.Models;

public class ReservaSoftware
{
    public int ReservaId { get; set; }
    public Reserva? Reserva { get; set; }
    public int SoftwareId { get; set; }
    public Software? Software { get; set; }
}
