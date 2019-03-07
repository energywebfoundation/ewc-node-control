namespace src
{
    public class StateChangeAction
    {
        public UpdateMode Mode { get; set; }
        public string Payload { get; set; }
        public string PayloadHash { get; set; }
    }
}