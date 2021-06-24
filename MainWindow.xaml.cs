using System;
using System.Collections.Generic;
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
        public void Init(double x0, double y0, double x1, double y1, Canvas canvas, int speed)
        {
            (canvas.Parent as Grid).Background = new SolidColorBrush(Color.FromRgb((byte)DateTime.Now.Millisecond, (byte)DateTime.Now.Millisecond, (byte)DateTime.Now.Millisecond));
            if (canvas.Children.Count > 500)
            {
                canvas.Children.RemoveRange(0, 2);
            }
            Rectangle movingBlock = new Rectangle
            {
                Fill = colors[DateTime.Now.Millisecond % 6],
                RadiusX = 20,
                RadiusY = 20,
                Width = 50,
                Height = 50,
                Margin = new Thickness(-25, -25, 0, 0),
            };
            movingBlock.MouseEnter += InitializeGameOver;
            DoubleAnimation animationX = new DoubleAnimation
            {
                From = x0,
                To = Math.Cos(Math.Atan2(y1 - y0, x1 - x0)) * Math.Sqrt(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2)) * 20 + x0,
                Duration = TimeSpan.FromSeconds(40 - speed)
            };
            DoubleAnimation animationY = new DoubleAnimation
            {
                From = y0,
                To = Math.Sin(Math.Atan2(y1 - y0, x1 - x0)) * Math.Sqrt(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2)) * 20 + y0,
                Duration = TimeSpan.FromSeconds(40 - speed)
            };
            TranslateTransform trans = new TranslateTransform();
            movingBlock.RenderTransform = trans;

            trans.BeginAnimation(TranslateTransform.XProperty, animationX);
            trans.BeginAnimation(TranslateTransform.YProperty, animationY);

            canvas.Children.Add(movingBlock);
        }

        public static void InitializeGameOver(object sender, MouseEventArgs e)
        {
            MainWindow.timer.Stop();
            if (MessageBox.Show("You have lose!\n" +
                "Time survived: " +
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
        public static DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (timer == null)
            {
                timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                timer.Tick += SecondCounter;
                timer.Start();
            }
            GetAngle(LeftEyeImage, e.GetPosition(this));
            GetAngle(RightEyeImage, e.GetPosition(this));
        }

        public static int surviveTime;
        private void SecondCounter(object sender, EventArgs e)
        {
            var second = int.Parse(TimeBlock.Text.Substring(15));
            surviveTime = second + 1;
            TimeBlock.Text = "Time survived: " + surviveTime.ToString();
        }

        private void GetAngle(Image image, Point mousePoint)
        {
            var x0 = image.TransformToAncestor(Application.Current.MainWindow)
                .Transform(new Point(image.ActualWidth * 0.5, image.ActualHeight * 0.5)).X;
            var y0 = image.TransformToAncestor(Application.Current.MainWindow)
                .Transform(new Point(image.ActualWidth * 0.5, image.ActualHeight * 0.5)).Y;
            var x1 = mousePoint.X;
            var y1 = mousePoint.Y;
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
            var angle = Math.Atan2(y1 - y0, x1 - x0) * 180 * .1 * Math.PI;
            RotateTransform rotate = new RotateTransform
            {
                Angle = angle,
                CenterX = .5,
                CenterY = .5
            };
            image.RenderTransform = rotate;

            new MovingBlock().Init(x0, y0, x1, y1, TextBlockCanvas, surviveTime);
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            MovingBlock.InitializeGameOver(null, null);
        }
    }
}