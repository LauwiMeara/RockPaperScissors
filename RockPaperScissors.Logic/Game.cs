namespace RockPaperScissors.Logic
{
    public class Game
    {
        public const int FrameRate = 50;
        public const int Speed = 10;
        public const int SignSize = 50;
        public const int RemovalCountDown = 20;

        private readonly Random _random = new();

        private readonly ITimer _timer;

        public Sign[] AllSigns { get; private set; }
        public Sign Winner { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Action Update { get; set; }

        public Game(ITimer timer) => this._timer = timer;

        public void Initialize(int width, int height, int numberOfSigns)
        {
            Width = width;
            Height = height;

            AllSigns = new Sign[numberOfSigns];

            for (int i = 0; i < numberOfSigns; i++)
            {
                Sign sign = new()
                {
                    Location = GetLocation(),
                    LocationOffset = GetLocationOffset()
                };

                sign.SignType = sign.GetSignType();

                AllSigns[i] = sign;
            }

            _timer.Interval = FrameRate;
            _timer.Tick += TimerElapsed;
            _timer.Start();
        }

        private void TimerElapsed(object? sender, EventArgs e)
        {
            for (int i = 0; i < AllSigns.Length; i++)
            {
                Sign currentSign = AllSigns[i];

                if (currentSign.IsActive)
                {
                    currentSign.Move();
                    AvoidBorderCollision(currentSign);

                    for (int j = i + 1; j < AllSigns.Length; j++)
                    {
                        Sign nextSign = AllSigns[j];

                        if (nextSign.IsActive && IsCollision(currentSign, nextSign))
                        {
                            DeactivateSign(DetermineCollisionLoser(currentSign, nextSign));
                        }
                    }
                }
                else if (!currentSign.CountDownExpired)
                {
                    currentSign.CountDown--;
                }

                if (AllSigns.Where(sign => sign.IsActive).GroupBy(sign => sign.SignType).Count() == 1)
                {
                    Winner = AllSigns.First(sign => sign.IsActive);
                    _timer.Stop();
                }
            }

            Update?.Invoke();
        }

        private Location GetLocation()
        {
            return new Location()
            {
                X = _random.Next(Width - SignSize),
                Y = _random.Next(Height - SignSize)
            };
        }

        private LocationOffset GetLocationOffset(int minValueX = -1, int maxValueX = 2, int minValueY = -1, int maxValueY = 2)
        {
            LocationOffset offset = new()
            {
                X = _random.Next(minValueX, maxValueX) * Speed,
                Y = _random.Next(minValueY, maxValueY) * Speed
            };

            while (offset.X == 0 && offset.Y == 0)
            {
                return GetLocationOffset(minValueX, maxValueX, minValueY, maxValueY);
            }

            return offset;
        }

        public void AvoidBorderCollision(Sign sign)
        {
            if (sign.Location.X <= 0)
            {
                if (sign.Location.Y <= 0)
                {
                    sign.LocationOffset = GetLocationOffset(minValueX: 0, minValueY: 0);
                }
                else if (sign.Location.Y >= Height - SignSize)
                {
                    sign.LocationOffset = GetLocationOffset(minValueX: 0, maxValueY: 1);
                }
                else
                {
                    sign.LocationOffset = GetLocationOffset(minValueX: 0);
                }
            }
            else if (sign.Location.X >= Width - SignSize)
            {
                if (sign.Location.Y <= 0)
                {
                    sign.LocationOffset = GetLocationOffset(maxValueX: 1, minValueY: 0);
                }
                else if (sign.Location.Y >= Height - SignSize)
                {
                    sign.LocationOffset = GetLocationOffset(maxValueX: 1, maxValueY: 1);
                }
                else
                {
                    sign.LocationOffset = GetLocationOffset(maxValueX: 1);
                }
            }
            else if (sign.Location.Y <= 0)
            {
                sign.LocationOffset = GetLocationOffset(minValueY: 0);
            }
            else if (sign.Location.Y >= Height - SignSize)
            {
                sign.LocationOffset = GetLocationOffset(maxValueY: 1);
            }
        }

        private static bool IsCollision(Sign currentSign, Sign nextSign)
        {
            Rectangle currentRectangle = new(currentSign.Location, SignSize);
            Rectangle nextRectangle = new(nextSign.Location, SignSize);

            return (RectanglesSidesIntersect(currentRectangle.Left, currentRectangle.Top, nextRectangle)) ||
                   RectanglesSidesIntersect(currentRectangle.Left, currentRectangle.Bottom, nextRectangle) ||
                   RectanglesSidesIntersect(currentRectangle.Right, currentRectangle.Top, nextRectangle) ||
                   RectanglesSidesIntersect(currentRectangle.Right, currentRectangle.Bottom, nextRectangle);
        }

        private static bool RectanglesSidesIntersect(int currentRectangleX, int currentRectangleY, Rectangle nextRectangle)
        {
            return (nextRectangle.Left <= currentRectangleX && currentRectangleX <= nextRectangle.Right &&
                nextRectangle.Top <= currentRectangleY && currentRectangleY <= nextRectangle.Bottom);
        }

        private static Sign DetermineCollisionLoser(Sign currentSign, Sign nextSign)
        {
            Sign[] signs = { currentSign, nextSign };

            if (signs.All(sign => sign.SignType == signs[0].SignType))
            {
                return signs[0];
            }
            if (signs.Any(sign => sign.SignType == SignType.Rock) &&
                     signs.Any(sign => sign.SignType == SignType.Paper))
            {
                return signs.Single(sign => sign.SignType == SignType.Rock);
            }
            if (signs.Any(sign => sign.SignType == SignType.Rock) &&
                     signs.Any(sign => sign.SignType == SignType.Scissors))
            {
                return signs.Single(sign => sign.SignType == SignType.Scissors);
            }
            
            return signs.Single(sign => sign.SignType == SignType.Paper);
        }

        private static void DeactivateSign(Sign sign)
        {
            sign.IsActive = false;
            sign.CountDown = RemovalCountDown;
            sign.LocationOffset = new LocationOffset { X = 0, Y = 0 };
        }
    }
}
