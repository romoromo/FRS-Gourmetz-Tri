namespace BAL.DTO.MealOrder
{
    public class TrayDTO
    {
        public int Id { get; set; }
        public string TrayId { get; set; }
        public int PLCId { get; set; }
        public string IPAddress { get; set; }
        public string Framework { get; set; }
        public int MotorOutputNumber { get; set; }
        public int LEDOutputNumber { get; set; }
    }
}
