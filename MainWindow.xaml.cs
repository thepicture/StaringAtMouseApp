using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace StaringAtMouseApp2
{
    public class MovingBlock
    {
        private readonly List<SolidColorBrush> colors = new List<SolidColorBrush> {
        Brushes.Red,
        Brushes.Orange,
        Brushes.Yellow,
        Brushes.Green,
        Brushes.Blue,
        Brushes.Magenta
        };
        private static Canvas myCanvas;
        private static double mouseX;
        private static double mouseY;
        public static bool isWin = false;
        private static Rectangle rectangle;
        public void Init(double x0, double y0, double x1, double y1, Grid grid, int speed)
        {
            Rectangle movingBlock = new Rectangle
            {
                Fill = new SolidColorBrush(Color.FromRgb((byte)DateTime.Now.Millisecond, (byte)DateTime.Now.Millisecond, (byte)DateTime.Now.Millisecond)),
                RadiusX = 20,
                RadiusY = 20,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Width = 50,
                Height = 50,
                Margin = new Thickness(-25, -25, 0, 0),
            };
            movingBlock.MouseEnter += InitializeGameOver;
            DoubleAnimation animationX = new DoubleAnimation
            {
                From = x0,
                To = Math.Cos(Math.Atan2(y1 - y0, x1 - x0)) * Math.Sqrt(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2)) * 20 + x0,
                Duration = TimeSpan.FromSeconds(MainWindow.SECONDSTOWIN - speed + 20)
            };
            DoubleAnimation animationY = new DoubleAnimation
            {
                From = y0,
                To = Math.Sin(Math.Atan2(y1 - y0, x1 - x0)) * Math.Sqrt(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2)) * 20 + y0,
                Duration = TimeSpan.FromSeconds(MainWindow.SECONDSTOWIN - speed + 20)
            };
            TranslateTransform trans = new TranslateTransform();
            movingBlock.RenderTransform = trans;

            trans.BeginAnimation(TranslateTransform.XProperty, animationX);
            trans.BeginAnimation(TranslateTransform.YProperty, animationY);

            grid.Children.Add(movingBlock);
        }

        public static void SetRect(Rectangle rect)
        {
            rectangle = rect;
        }

        public void Init(double x0, double y0, double x1, double y1, Canvas canvas, int speed)
        {
            mouseX = x1;
            mouseY = y1;
            myCanvas = canvas;
            if (MainWindow.surviveTime > 12)
            {
                (canvas.Parent as Grid).Background = new SolidColorBrush(Color.FromRgb((byte)DateTime.Now.Millisecond, (byte)DateTime.Now.Millisecond, (byte)DateTime.Now.Millisecond));
            }

            if (canvas.Children.Count > 800)
            {
                canvas.Children.RemoveRange(0, 4);
            }
            Rectangle movingBlock = new Rectangle
            {
                Fill = MainWindow.surviveTime > 12 ? colors[DateTime.Now.Millisecond % 6] : Brushes.White,
                RadiusX = 20,
                RadiusY = 20,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Width = 50,
                Height = 50,
                Margin = new Thickness(-25, -25, 0, 0),
            };
            movingBlock.MouseEnter += InitializeGameOver;
            DoubleAnimation animationX = new DoubleAnimation
            {
                From = x0,
                To = Math.Cos(Math.Atan2(y1 - y0, x1 - x0)) * Math.Sqrt(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2)) * 20 + x0,
                Duration = TimeSpan.FromSeconds(MainWindow.SECONDSTOWIN - speed)
            };
            DoubleAnimation animationY = new DoubleAnimation
            {
                From = y0,
                To = Math.Sin(Math.Atan2(y1 - y0, x1 - x0)) * Math.Sqrt(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2)) * 20 + y0,
                Duration = TimeSpan.FromSeconds(MainWindow.SECONDSTOWIN - speed)
            };
            TranslateTransform trans = new TranslateTransform();
            movingBlock.RenderTransform = trans;

            trans.BeginAnimation(TranslateTransform.XProperty, animationX);
            trans.BeginAnimation(TranslateTransform.YProperty, animationY);

            canvas.Children.Add(movingBlock);
        }

        public static void InitializeGameOver(object sender, MouseEventArgs e)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(10),
                AutoReverse = false
            };
            Storyboard.SetTarget(da, rectangle);
            Storyboard.SetTargetProperty(da, new PropertyPath("Opacity"));

            sb.Children.Add(da);
            sb.Begin();
            Storyboard sb2 = new Storyboard();
            DoubleAnimation da2 = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(10),
                AutoReverse = false
            };
            Storyboard.SetTarget(da2, myCanvas.Parent as Grid);
            Storyboard.SetTargetProperty(da2, new PropertyPath("Opacity"));

            sb2.Children.Add(da2);
            sb2.Begin();
            if (isWin)
            {
                return;
            }
            MainWindow.player.Stop();
            for (int i = 0; i < 45; i++)
            {
                System.Collections.IEnumerator enumerator = ((myCanvas.Parent as Grid).Parent as Grid).Children.GetEnumerator();
                enumerator.MoveNext();
                new MovingBlock().Init(mouseX, mouseY, 500 * Math.Cos(i) + mouseX, 500 * Math.Sin(i) + mouseY, enumerator.Current as Grid, 5);
                MainWindow.player.Open(new Uri("../../Resources/lose.wav", UriKind.Relative));
                MainWindow.player.Play();
            }
            MainWindow.timer.Stop();
            if (MessageBox.Show("You have lose!\n" +
                "Seconds survived: " +
                MainWindow.surviveTime +
                "\nAgain?", "Game over", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            else
            {
                App.Current.Shutdown();
            }
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int SECONDSTOWIN = 40;
        public static Timer timer;
        public static MediaPlayer player = new MediaPlayer();
        public MainWindow()
        {
            InitializeComponent();
            MovingBlock.SetRect(MyRect);
            player.MediaEnded += Player_MediaEnded;
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            player.Position = TimeSpan.Zero;
            player.Play();
        }

        readonly Storyboard storyboard = new Storyboard();
        private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (timer == null)
            {
                player.Open(new Uri("../../Resources/sound.wav", UriKind.Relative));
                player.Play();
                timer = new Timer()
                {
                    Interval = 1000
                };
                timer.Elapsed += SecondCounter;
                timer.Start();
            }
            if (surviveTime == 13)
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation
                {
                    From = -20,
                    To = 20,
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    Duration = TimeSpan.FromSeconds(4),
                    AccelerationRatio = 0.3,
                    DecelerationRatio = 0.3,
                };
                Storyboard.SetTarget(doubleAnimation, MainGrid);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Grid.RenderTransform).(RotateTransform.Angle)"));

                storyboard.Children.Add(doubleAnimation);
                storyboard.Begin();
            }
            GetAngle(LeftEyeImage, e.GetPosition(this));
            GetAngle(RightEyeImage, e.GetPosition(this));
        }

        public static int surviveTime;
        private void SecondCounter(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var second = int.Parse(TimeBlock.Text.Substring(17));
                surviveTime = second + 1;
                if (SECONDSTOWIN - surviveTime == 0)
                {
                    MovingBlock.isWin = true;
                    timer.Stop();
                    storyboard.Stop();
                    player.Stop();
                    player.Open(new Uri("../../Resources/win.wav", UriKind.Relative));
                    player.Play();
                    if (MessageBox.Show("You win!" +
                        "\nPlay again?", "Victory", MessageBoxButton.YesNo, MessageBoxImage.Question)
                    == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        App.Current.Shutdown();
                    }
                }
                TimeBlock.Text = "Seconds survived: " + surviveTime.ToString();
            });
        }

        private readonly Random random = new Random();
        private void GetAngle(Image image, Point mousePoint)
        {
            var x0 = image.TransformToAncestor(Application.Current.MainWindow)
                .Transform(new Point(image.ActualWidth * 0.5, image.ActualHeight * 0.5)).X;
            var y0 = image.TransformToAncestor(Application.Current.MainWindow)
                .Transform(new Point(image.ActualWidth * 0.5, image.ActualHeight * 0.5)).Y;
            var x1 = mousePoint.X;
            var y1 = mousePoint.Y;
            if (surviveTime > 12)
            {
                TimeBlock.Opacity = random.NextDouble();
                if (Math.Sqrt(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2)) < image.ActualHeight / 2)
                {
                    image.Source = RightEyeImage.Source = new BitmapImage(
                        new Uri("pack://application:,,,/Resources/eye_zero_degrees_closed.png")
                        );
                }
                else
                {
                    image.Source = RightEyeImage.Source = new BitmapImage(
                        new Uri("pack://application:,,,/Resources/eye_zero_degrees_opened.png")
                        );
                }
            }
            var angle = Math.Atan2(y1 - y0, x1 - x0) * 180 * .1 * Math.PI;
            RotateTransform rotate = new RotateTransform
            {
                Angle = angle,
                CenterX = .5,
                CenterY = .5
            };
            image.RenderTransform = rotate;

            if (random.Next() % 7 == 0)
            {
                new MovingBlock().Init(x0, y0, x1, y1, TextBlockCanvas, surviveTime);
            }
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            MovingBlock.InitializeGameOver(null, null);
        }
    }
}