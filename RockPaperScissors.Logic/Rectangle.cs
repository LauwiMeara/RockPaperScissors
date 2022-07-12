namespace RockPaperScissors.Logic
{
    public class Rectangle
    {
        public int Left { get; }
        public int Right { get; }
        public int Top { get; }
        public int Bottom { get; }


        public Rectangle(Location location, int signSize)
        {
            Left = location.X;
            Right = location.X + signSize;
            Top = location.Y;
            Bottom = location.Y + signSize;
        }
    }
}
