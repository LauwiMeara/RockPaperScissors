namespace RockPaperScissors
{
    internal class CustomPictureBox : PictureBox
    {
        public ImageTitle ImageTitle { get; set; }
        public Direction Direction { get; set; }
        public int ExplosionTimer { get; set; }
    }
}
