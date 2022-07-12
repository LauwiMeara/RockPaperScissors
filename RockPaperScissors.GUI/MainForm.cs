using RockPaperScissors.Logic;

namespace RockPaperScissors.GUI
{
    public partial class MainForm : Form
    {
        private const int NumberOfSigns = 20;

        private Game _game;
        private readonly Dictionary<Sign, PictureBox> _signsAndPictures = new();

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializePictures()
        {
            foreach (Sign sign in _game.AllSigns)
            {
                PictureBox picture = new()
                {
                    Size = new Size(Game.SignSize, Game.SignSize),
                    Location = new Point(sign.Location.X, sign.Location.Y),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = GetImage(sign.SignType)
                };
                
                Controls.Add(picture);
                _signsAndPictures.Add(sign, picture);
            }
        }

        private static Image GetImage(SignType signType)
        {
            switch (signType)
            {
                case SignType.Rock:
                    return Properties.Resources.Rock;
                case SignType.Paper:
                    return Properties.Resources.Paper;
                case SignType.Scissors:
                    return Properties.Resources.Scissors;
                default:
                    return Properties.Resources.Rock;
            }
        }

        private static void ShowExplosion(PictureBox picture)
        {
            picture.Size = new Size(Game.SignSize / 2, Game.SignSize / 2);
            picture.Image = Properties.Resources.Explosion;
        }

        private void EndGame(Image winnerImage)
        {
            Controls.Clear();

            PictureBox winnerDeclaration = new()
            {
                Dock = DockStyle.Fill,
                BackgroundImage = winnerImage,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = Properties.Resources.Win
            };

            Controls.Add(winnerDeclaration);
        } 

        private void OnLoad(object sender, EventArgs e)
        {   
            _game = new(new CustomTimer());
            _game.Initialize(ClientRectangle.Width, ClientRectangle.Height, NumberOfSigns);
            InitializePictures();
            _game.Update = OnGameUpdate;
        }

        private void OnClientSizeChanged(object sender, EventArgs e)
        {
            _game.Width = ClientRectangle.Width;
            _game.Height = ClientRectangle.Height;
        }

        private void OnGameUpdate()
        {
            if (_game.Winner != null)
            {
                PictureBox picture = _signsAndPictures[_game.Winner];
                EndGame(picture.Image);
            }
            foreach (Sign sign in _game.AllSigns)
            {
                PictureBox picture = _signsAndPictures[sign];

                if (!sign.IsActive)
                {
                    if (sign.CountDownExpired)
                    {
                        picture.Dispose();
                    }
                    else if (picture.Image != Properties.Resources.Explosion)
                    {
                        ShowExplosion(picture);
                    }
                }
                else
                {
                    picture.Location = new Point(sign.Location.X, sign.Location.Y);
                }
            }
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