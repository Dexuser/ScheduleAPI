namespace Application.Dtos;

public class SlotDto
{
        public int Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsTaken { get; set; }
}