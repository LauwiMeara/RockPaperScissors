namespace RockPaperScissors
{
    public partial class MainForm : Form
    {
        public const int speed = 5;
        public const int margin = 40;
        public const int sizeOfSign = 100;
        public const int explosionTimer = 10;

        readonly Random random = new();

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializePictures(int numberOfSigns)
        {
            for (int i = 0; i < numberOfSigns; i++)
            {
                CustomPictureBox sign = new()
                {
                    Size = new Size(sizeOfSign, sizeOfSign),
                    Location = new Point(random.Next(margin, this.ClientRectangle.Width - sizeOfSign - margin), random.Next(margin, this.ClientRectangle.Height - sizeOfSign - margin)),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Direction = GetDirection()
                };

                switch (random.Next(0, 3))
                {
                    case 0:
                        sign.Image = Properties.Resources.Rock;
                        sign.ImageTitle = ImageTitle.Rock;
                        break;
                    case 1:
                        sign.Image = Properties.Resources.Paper;
                        sign.ImageTitle = ImageTitle.Paper;
                        break;
                    case 2:
                        sign.Image = Properties.Resources.Scissors;
                        sign.ImageTitle = ImageTitle.Scissors;
                        break;
                    default:
                        break;
                }

                Controls.Add(sign);
            }
        }

        private Direction GetDirection(int minValueX = -1, int maxValueX = 2, int minValueY = -1, int maxValueY = 2)
        {
            Direction direction = new()
            {
                X = random.Next(minValueX, maxValueX) * speed,
                Y = random.Next(minValueY, maxValueY) * speed
            };

            while (direction.X == 0 && direction.Y == 0)
            {
                return GetDirection(minValueX, maxValueX, minValueY, maxValueY);
            }

            return direction;
        }

        private void EndGame(Image winnerImage)
        {
            timer.Stop();
            Controls.Clear();

            PictureBox winnerDeclaration = new()
            {
                Dock = DockStyle.Fill,
                BackgroundImage = winnerImage,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = Properties.Resources.Win
            };

            Controls.Add(winnerDeclaration);
            Refresh();
        }

        private void MoveSign(CustomPictureBox sign)
        {
            sign.Left += sign.Direction.X;
            sign.Top += sign.Direction.Y;
        }

        private void AvoidBorderCollision(CustomPictureBox sign)
        {
            if (sign.Left <= margin)
            {
                if (sign.Top <= margin)
                    sign.Direction = GetDirection(minValueX: 0, minValueY: 0);
                else if (sign.Top >= this.ClientRectangle.Height - sign.Height - margin)
                    sign.Direction = GetDirection(minValueX: 0, maxValueY: 1);
                else
                    sign.Direction = GetDirection(minValueX: 0);
            }
            else if (sign.Left >= this.ClientRectangle.Width - sign.Width - margin)
            {
                if (sign.Top <= margin)
                    sign.Direction = GetDirection(maxValueX: 1, minValueY: 0);
                else if (sign.Top >= this.ClientRectangle.Height - sign.Height - margin)
                    sign.Direction = GetDirection(maxValueX: 1, maxValueY: 1);
                else
                    sign.Direction = GetDirection(maxValueX: 1);
            }
            else if (sign.Top <= margin)
            {
                sign.Direction = GetDirection(minValueY: 0);
            }
            else if (sign.Top >= this.ClientRectangle.Height - sign.Height - margin)
            {
                sign.Direction = GetDirection(maxValueY: 1);
            }
        }
        
        private CustomPictureBox DetermineCollisionLoser(CustomPictureBox[] signs)
        {
            if (signs.All(sign => sign.ImageTitle.Equals(signs.First().ImageTitle)))
                return signs[0];
            else if (signs.Any(sign => sign.ImageTitle.Equals(ImageTitle.Rock)) && signs.Any(sign => sign.ImageTitle.Equals(ImageTitle.Paper)))
                return signs.Where(sign => sign.ImageTitle.Equals(ImageTitle.Rock)).Single();
            else if (signs.Any(sign => sign.ImageTitle.Equals(ImageTitle.Rock)) && signs.Any(sign => sign.ImageTitle.Equals(ImageTitle.Scissors)))
                return signs.Where(sign => sign.ImageTitle.Equals(ImageTitle.Scissors)).Single();
            else
                return signs.Where(sign => sign.ImageTitle.Equals(ImageTitle.Paper)).Single();
        }

        private void ExplodeCollisionLoser(CustomPictureBox sign)
        {
            sign.Size = new Size(sizeOfSign / 2, sizeOfSign / 2);
            sign.Direction.X = 0;
            sign.Direction.Y = 0;
            sign.Image = Properties.Resources.Explosion;
            sign.ImageTitle = ImageTitle.Explosion;
            sign.ExplosionTimer = explosionTimer;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CustomPictureBox[] allExplosions = Controls.OfType<CustomPictureBox>().Where(sign => sign.ImageTitle.Equals(ImageTitle.Explosion)).ToArray();
            
            foreach (CustomPictureBox explosion in allExplosions)
            {
                if (explosion.ExplosionTimer == 0)
                    explosion.Dispose();
                else
                    explosion.ExplosionTimer--;
            }

            CustomPictureBox[] allSigns = Controls.OfType<CustomPictureBox>().Except(allExplosions).ToArray();

            if (allSigns.All(sign => sign.ImageTitle.Equals(allSigns.First().ImageTitle)))
            {
                EndGame(allSigns.First().Image);
            }

            else
            {
                for (int i = 0; i < allSigns.Length; i++)
                {
                    CustomPictureBox currentSign = allSigns[i];

                    MoveSign(currentSign);
                    AvoidBorderCollision(currentSign);

                    for (int j = i + 1; j < allSigns.Length; j++)
                    {
                        CustomPictureBox nextSign = allSigns[j];

                        if (currentSign.ImageTitle != ImageTitle.Explosion &&
                            nextSign.ImageTitle != ImageTitle.Explosion &&
                            currentSign.Bounds.IntersectsWith(nextSign.Bounds))
                        {
                            CustomPictureBox[] signs = { currentSign, nextSign };
                            CustomPictureBox loser = DetermineCollisionLoser(signs);
                            ExplodeCollisionLoser(loser);
                        }
                    }
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitializePictures(50);
            timer.Start();
        }
    }
}

/* 
 * <a href="https://www.flaticon.com/free-icons/scissors" title="scissors icons">Scissors icons created by Freepik - Flaticon</a>
 * <a href="https://www.flaticon.com/free-icons/rock" title = "rock icons" > Rock icons created by Freepik - Flaticon</a>
 * <a href="https://www.flaticon.com/free-icons/paper" title="paper icons">Paper icons created by Freepik - Flaticon</a>
 * <a href="https://www.flaticon.com/free-icons/explosion" title="explosion icons">Explosion icons created by Freepik - Flaticon</a> 
 * <a href="https://www.flaticon.com/free-icons/winner" title="winner icons">Winner icons created by Freepik - Flaticon</a>
*/