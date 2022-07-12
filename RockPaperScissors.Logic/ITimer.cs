namespace RockPaperScissors.Logic
{
    public interface ITimer
    {
        public int Interval { get; set; }
        public event EventHandler Tick;

        public void Start();
        public void Stop();
    }
}
