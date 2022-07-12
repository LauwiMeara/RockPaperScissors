namespace RockPaperScissors.Logic
{
    public class Sign
    {
        private readonly Random _random = new();

        private bool _isActive = true;

        public SignType SignType { get; set; }
        public Location Location { get; set; }
        public LocationOffset LocationOffset { get; set; }
        public bool IsActive 
        { 
            get 
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                LocationOffset.X = 0;
                LocationOffset.Y = 0;
            }
        }
        public int CountDown { get; set; }
        public bool CountDownExpired => CountDown <= 0;

        public SignType GetSignType()
        {
            switch (_random.Next(0, 3))
            {
                case 0:
                    return SignType.Rock;
                case 1:
                    return SignType.Paper;
                case 2:
                    return SignType.Scissors;
                default:
                    return SignType.Rock;
            }
        }

        public void Move()
        {
            Location.X += LocationOffset.X;
            Location.Y += LocationOffset.Y;
        }
    }
}
