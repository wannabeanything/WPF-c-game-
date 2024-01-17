using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Hosting;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Security.AccessControl;
using System.Diagnostics;

namespace WpfApp3
{
    

    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void window1_Loaded(object sender, RoutedEventArgs e)
        {
            Image imageControl = new Image();
            imageControl.Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\61691fda-8635-46b3-ba25-0b029dbe1911.jpg"));
            imageControl.Width = window1.ActualWidth;
            imageControl.Height = window1.ActualHeight;
            g1.Children.Add(imageControl);
            Button startgame = new Button();
            startgame.Content = "Спаси папича";
            startgame.Width = 120;
            startgame.Height = 30;
            startgame.Margin = new Thickness(0, 0, 0, 0);
            startgame.Click += startgame_Click;
            g1.Children.Add(startgame);
            Button leavegame = new Button();
            leavegame.Content = "Ливни с позором";
            leavegame.Width = 120;
            leavegame.Height = 30;
            leavegame.Margin = new Thickness(0, 90, 0, 0);
            leavegame.Click += leavegame_Click;
            g1.Children.Add(leavegame);
        }
        private void startgame_Click(object sender, RoutedEventArgs e)
        {
            AnotherWindow anotherWindow = new AnotherWindow();
            anotherWindow.Show();
            window1.Close();
        }
        private void leavegame_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
    
    public class EnemyBullet
    {
        public Image BulletImage { get; private set; }

        public EnemyBullet(string imagePath)
        {
            BulletImage = new Image
            {
                Width = 20,
                Height = 20,
                Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute)),
            };
        }
    }
    public class Enemy
    {
        public Image EnemyImage { get; private set; }
        public int Health { get; private set; }
        
        private Canvas canvas;
        public List<EnemyBullet> EnemyBullets { get; private set; }

        public Enemy(Canvas canvas)
        {
            this.canvas = canvas;
            EnemyBullets = new List<EnemyBullet>();
        }
        
        public void InitializeEnemy(Random random, string imagePath, Rectangle arena)
        {
            EnemyImage = new Image
            {
                Width = 100,
                Height = 100,
                Source = new BitmapImage(new Uri(imagePath, UriKind.Relative)),
            };

            double left = random.NextDouble() * (arena.Width - EnemyImage.Width) + arena.Margin.Left;
            double top = random.NextDouble() * (arena.Height - EnemyImage.Height) + arena.Margin.Top;

            EnemyImage.Margin = new Thickness(left, top, 0, 0);

            Health = 2;

            canvas.Children.Add(EnemyImage);
        }

        

        

        public void DecreaseHealth()
        {
            Health--;
        }
    }

    public class AnotherWindow : Window
    {
        private Canvas myCanvas;
        private Image playerImage;
        private Rectangle arena;
        private Image healthBarImage;
        private List<Image> enemyBullets = new List<Image>();
        private Random random;
        public int healthBar = 5;
        private List<Ellipse> bullets;
        private string bulletImagePath = "C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\dcf7c3bd-7acb-4be7-b251-708c0d1ebada.jpg"; // Replace with the actual path to your bullet image
        private bool canShoot = true;
        private DispatcherTimer shootCooldownTimer;
        private double playerVelocityX = 0.0;
        private double playerVelocityY = 0.0;
        private const double acceleration = 0.5;
        private const double deceleration = 0.1;
        private DispatcherTimer movementTimer;
        private DispatcherTimer decelerationTimer;
        private DispatcherTimer enemyBulletTimer;
        private List<Enemy> enemies;
        private string enemyImagePath = "C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\Снимок экрана 2024-01-02 140832333.png";
       
        private string enemyBulletImagePath = "C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\выстрел-рубик.png";
        public string ArenaBackgroundImagePath { get; set; } = "C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\msg6246032635-254877.jpg";
        
        public AnotherWindow()
        {
            Title = "lvl1";
            Height = 1080;
            Width = 1920;
            enemies = new List<Enemy>();
            InitializeComponents();
            bullets = new List<Ellipse>();
            InitializeTimers();
            
            InitializeMovementTimer();
            
            
            GenerateEnemies();
            random = new Random();
            /*Button myButton = new Button();
            myButton.Content = "Click me!";

            // Attach an event handler (leave it empty)
            myButton.Click += MyButton_Click;
            myButton.Margin = new Thickness(0, 0, 0, 0);
            myCanvas.Children.Add(myButton);*/


        }
        /*private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            TheLastLevel theLastLevel = new TheLastLevel();
            theLastLevel.Show();
            this.Close();
        }*/
        
        
        


        



        private void GenerateEnemies()
        {
            Random random = new Random();

            for (int i = 0; i < 5; i++)
            {
                Enemy enemy = new Enemy(myCanvas);
                enemy.InitializeEnemy(random, enemyImagePath, arena);

                // Add the enemy to the list
                enemies.Add(enemy);
            }
        }
        private void HandleEnemyShooting()
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy != null && enemy.EnemyImage != null)
                {
                    MoveEnemyBullet(enemy.EnemyImage);
                }
            }
        }

        private void MoveEnemyBullet(Image enemyImage)
        {
            if (enemyImage != null)
            {
                // Create a new enemy bullet (ellipse) with an image
                Ellipse enemyBullet = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    Fill = new ImageBrush(new BitmapImage(new Uri(enemyBulletImagePath, UriKind.Relative))),
                };

                // Set the initial position of the bullet at the enemy's position
                double enemyLeft = Canvas.GetLeft(enemyImage) + enemyImage.Width / 2;
                double enemyTop = Canvas.GetTop(enemyImage) + enemyImage.Height / 2;

                Canvas.SetLeft(enemyBullet, enemyLeft - enemyBullet.Width / 2);
                Canvas.SetTop(enemyBullet, enemyTop - enemyBullet.Height / 2);

                // Add the bullet to the canvas
                myCanvas.Children.Add(enemyBullet);

                // Calculate the direction vector towards the player
                double directionX = playerImage.Margin.Left + playerImage.Width / 2 - Canvas.GetLeft(enemyBullet) - enemyBullet.Width / 2;
                double directionY = playerImage.Margin.Top + playerImage.Height / 2 - Canvas.GetTop(enemyBullet) - enemyBullet.Height / 2;

                // Normalize the direction vector
                double length = Math.Sqrt(directionX * directionX + directionY * directionY);
                directionX /= length;
                directionY /= length;

                // Move the bullet towards the player
                double speed = 5; // Adjust the speed as needed

                // Use a DispatcherTimer to update the bullet position
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(16); // Update every ~16 milliseconds

                // Define the Tick event handler
                timer.Tick += (sender, e) =>
                {
                    // Move the bullet towards the player
                    Canvas.SetLeft(enemyBullet, Canvas.GetLeft(enemyBullet) + directionX * speed);
                    Canvas.SetTop(enemyBullet, Canvas.GetTop(enemyBullet) + directionY * speed);

                    // Check if the bullet is out of bounds
                    if (Canvas.GetLeft(enemyBullet) < 0 || Canvas.GetTop(enemyBullet) < 0 || Canvas.GetLeft(enemyBullet) > Width || Canvas.GetTop(enemyBullet) > Height)
                    {
                        // Remove the bullet from the canvas
                        myCanvas.Children.Remove(enemyBullet);
                        timer.Stop(); // Stop the timer
                    }
                };

                // Start the timer
                timer.Start();
            }
        }







        private bool CheckCollision(UIElement element1, UIElement element2)
        {
            Rect bounds1 = element1.TransformToVisual(myCanvas).TransformBounds(new Rect(0, 0, element1.RenderSize.Width, element1.RenderSize.Height));
            Rect bounds2 = element2.TransformToVisual(myCanvas).TransformBounds(new Rect(0, 0, element2.RenderSize.Width, element2.RenderSize.Height));

            return bounds1.IntersectsWith(bounds2);
        }



        private void InitializeMovementTimer()
        {
            movementTimer = new DispatcherTimer();
            movementTimer.Interval = TimeSpan.FromMilliseconds(16); // Adjust the interval as needed
            movementTimer.Tick += (sender, e) => UpdatePlayerPosition();
            movementTimer.Start();
        }
        private void InitializeTimers()
        {
            shootCooldownTimer = new DispatcherTimer();
            shootCooldownTimer.Interval = TimeSpan.FromSeconds(2); // Set the cooldown time (2 seconds in this example)
            shootCooldownTimer.Tick += (sender, e) => canShoot = true;

            decelerationTimer = new DispatcherTimer();
            decelerationTimer.Interval = TimeSpan.FromMilliseconds(16); // Adjust the interval as needed
            decelerationTimer.Tick += (sender, e) => DeceleratePlayer();


            
        }
        
        

        private void HandlePlayerStop(object sender, KeyEventArgs e)
        {
            // Stop player movement when a movement key is released
            if (e.Key == Key.W || e.Key == Key.S)
            {
                playerVelocityY = 0.0;
            }
            if (e.Key == Key.A || e.Key == Key.D)
            {
                playerVelocityX = 0.0;
            }
        }
        private void InitializeComponents()
        {
            myCanvas = new Canvas();
            Content = myCanvas;

            arena = new Rectangle
            {
                Width = 1600,
                Height = 800,
                Stroke = Brushes.Black,
            };

            arena.Fill = new ImageBrush(new BitmapImage(new Uri(ArenaBackgroundImagePath, UriKind.Relative)));

            arena.Margin = new Thickness((Width - arena.Width) / 2, (Height - arena.Height) / 2, 0, 0);

            playerImage = new Image
            {
                Width = 100,
                Height = 100,
                Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\Снимок экрана 2024-01-02 14024000.png", UriKind.Relative)),
            };

            playerImage.Margin = new Thickness((arena.Width - playerImage.Width) / 2 + arena.Margin.Left,
                                               (arena.Height - playerImage.Height) / 2 + arena.Margin.Top, 0, 0);

            healthBarImage = new Image
            {
                Width = 200,
                Height = 30,
                Source = new BitmapImage(new Uri("path/to/your/healthBarImage.png", UriKind.Relative)),
            };

            healthBarImage.Margin = new Thickness((Width - healthBarImage.Width) / 2, 10, 0, 0);

            myCanvas.Children.Add(arena);
            myCanvas.Children.Add(playerImage);
            myCanvas.Children.Add(healthBarImage);

            

            this.KeyDown += HandlePlayerMovement;
            this.MouseDown += HandleShooting;
        }
        private void UpdatePlayerPosition()
        {
            // Update player position based on velocity
            double newLeft = playerImage.Margin.Left + playerVelocityX;
            double newTop = playerImage.Margin.Top + playerVelocityY;

            // Clamp player position within the arena bounds
            newLeft = Math.Max(Math.Min(newLeft, arena.Margin.Left + arena.Width - playerImage.Width), arena.Margin.Left);
            newTop = Math.Max(Math.Min(newTop, arena.Margin.Top + arena.Height - playerImage.Height), arena.Margin.Top);

            playerImage.Margin = new Thickness(newLeft, newTop, 0, 0);

            UpdateHealthBarImage();
            HandleEnemyShooting();
        }

        private void HandlePlayerMovement(object sender, KeyEventArgs e)
        {
            
            const double maxSpeed = 5.0; // You can adjust the maximum speed

            if (e.Key == Key.W)
            {
                playerVelocityY = Math.Max(playerVelocityY - acceleration, -maxSpeed);
            }
            if (e.Key == Key.A)
            {
                playerVelocityX = Math.Max(playerVelocityX - acceleration, -maxSpeed);
            }
            if (e.Key == Key.S)
            {
                playerVelocityY = Math.Min(playerVelocityY + acceleration, maxSpeed);
            }
            if (e.Key == Key.D)
            {
                playerVelocityX = Math.Min(playerVelocityX + acceleration, maxSpeed);
            }
            
            // Update player position based on velocity
            double newLeft = playerImage.Margin.Left + playerVelocityX;
            double newTop = playerImage.Margin.Top + playerVelocityY;

            // Clamp player position within the arena bounds
            newLeft = Math.Max(Math.Min(newLeft, arena.Margin.Left + arena.Width - playerImage.Width), arena.Margin.Left);
            newTop = Math.Max(Math.Min(newTop, arena.Margin.Top + arena.Height - playerImage.Height), arena.Margin.Top);

            playerImage.Margin = new Thickness(newLeft, newTop, 0, 0);
            decelerationTimer.Start();
            UpdateHealthBarImage();
        }


        private void UpdateHealthBarImage()
        {
            switch (healthBar)
            {
                case 0:
                    this.Close();
                    break;
                case 1:
                    healthBarImage.Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\hp1.jpg"));
                    break;
                case 2:
                    healthBarImage.Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\hp2.jpg"));
                    break;
                case 3:
                    healthBarImage.Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\hp3.jpg"));
                    break;
                case 4:
                    healthBarImage.Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\hp4.jpg"));
                    break;
                case 5:
                    healthBarImage.Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\hp5.jpg"));
                    break;
            }
        }

        
        private void HandleShooting(object sender, MouseButtonEventArgs e)
        {
            if (canShoot)
            {
                // Get player position
                double playerLeft = playerImage.Margin.Left + playerImage.Width / 2;
                double playerTop = playerImage.Margin.Top + playerImage.Height / 2;

                // Create a new bullet (ellipse) with an image
                Ellipse bullet = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    Fill = new ImageBrush(new BitmapImage(new Uri(bulletImagePath, UriKind.Relative))),
                };

                // Set initial position of the bullet at the player's position
                Canvas.SetLeft(bullet, playerLeft - bullet.Width / 2);
                Canvas.SetTop(bullet, playerTop - bullet.Height / 2);

                // Add the bullet to the canvas
                myCanvas.Children.Add(bullet);

                // Add the bullet to the list
                bullets.Add(bullet);

                // Move the bullet in the direction of the mouse pointer
                MoveBullet(bullet, e.GetPosition(myCanvas));

                canShoot = false; // Set the flag to false
                shootCooldownTimer.Start(); // Start the cooldown timer
            }
        }
        private void DeceleratePlayer()
        {
            // Decelerate the player smoothly
            if (playerVelocityX > 0)
            {
                playerVelocityX = Math.Max(playerVelocityX - deceleration, 0);
            }
            else if (playerVelocityX < 0)
            {
                playerVelocityX = Math.Min(playerVelocityX + deceleration, 0);
            }

            if (playerVelocityY > 0)
            {
                playerVelocityY = Math.Max(playerVelocityY - deceleration, 0);
            }
            else if (playerVelocityY < 0)
            {
                playerVelocityY = Math.Min(playerVelocityY + deceleration, 0);
            }
        }
        private void MoveBullet(Ellipse bullet, Point targetPosition)
        {
            // Calculate the direction vector
            double directionX = targetPosition.X - Canvas.GetLeft(bullet) - bullet.Width / 2;
            double directionY = targetPosition.Y - Canvas.GetTop(bullet) - bullet.Height / 2;

            // Normalize the direction vector
            double length = Math.Sqrt(directionX * directionX + directionY * directionY);
            directionX /= length;
            directionY /= length;

            // Move the bullet in the direction of the mouse pointer
            double speed = 20; // You can adjust the speed

            // Use a DispatcherTimer to update the bullet position
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16); // Update every ~16 milliseconds

            // Define the Tick event handler
            EventHandler timerTick = null;
            timerTick = (sender, e) =>
            {
                Canvas.SetLeft(bullet, Canvas.GetLeft(bullet) + directionX * speed);
                Canvas.SetTop(bullet, Canvas.GetTop(bullet) + directionY * speed);

                // Check if the bullet is out of bounds
                if (Canvas.GetLeft(bullet) < 0 || Canvas.GetTop(bullet) < 0 || Canvas.GetLeft(bullet) > Width || Canvas.GetTop(bullet) > Height)
                {
                    // Remove the bullet from the canvas and the list
                    myCanvas.Children.Remove(bullet);
                    bullets.Remove(bullet);
                    timer.Tick -= timerTick; // Remove the event handler
                    timer.Stop(); // Stop the timer
                }
                else
                {
                    // Check for collisions with enemies
                    foreach (Enemy enemy in enemies.ToList()) // Use ToList() to avoid modification while iterating
                    {
                        if (CheckCollision(enemy.EnemyImage, bullet))
                        {
                            // Remove the bullet from the canvas and the list
                            myCanvas.Children.Remove(bullet);
                            bullets.Remove(bullet);
                            timer.Tick -= timerTick; // Remove the event handler
                            timer.Stop(); // Stop the timer

                            // Decrease enemy health
                            enemy.DecreaseHealth();

                            // Check if the enemy is defeated
                            if (enemy.Health <= 0)
                            {
                                // Remove the enemy from the canvas and the list
                                myCanvas.Children.Remove(enemy.EnemyImage);
                                enemies.Remove(enemy);
                            }

                            // Check if all enemies are defeated
                            if (enemies.Count == 0)
                            {
                                // Close the current window
                                BlankGridWindow blankGridWindow = new BlankGridWindow();
                                blankGridWindow.Show();
                                this.Close();
                            }

                            break; // Exit the loop since the bullet hit an enemy
                        }
                    }
                }
            };

            // Attach the event handler to the Tick event
            timer.Tick += timerTick;

            // Start the timer
            timer.Start();
        }
    }

    public class BlankGridWindow : Window
    {
        private Image playerImage;
        private int[,] array;

        public BlankGridWindow()
        {
            Title = "lvl2";
            Width = 1920;
            Height = 1080;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            array = GenerateRandomizedArray(20, 38);
            PrintArray(array);

            Canvas canvas = new Canvas(); 
            Content = canvas; 

            playerImage = new Image
            {
                Width = 50,
                Height = 50,
                Margin = new Thickness(10, 10, 0, 0),
                Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\Снимок экрана 2024-01-02 14024000.png", UriKind.Relative)),
            };

            canvas.Children.Add(playerImage); 

            GenerateRandomCorridorGrid(array, canvas); 

            
            this.KeyDown += HandlePlayerMovement;
        }

        private void GenerateRandomCorridorGrid(int[,] array, Canvas canvas)
        {
            int rectangleWidth = 0;
            int onesCount = 0;

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    if (array[i, j] == 1)
                    {
                        onesCount++;
                        rectangleWidth += 50; 
                    }
                    else
                    {
                        if (onesCount >= 2)
                        {
                            
                            Rectangle cell = new Rectangle
                            {
                                Width = rectangleWidth,
                                Height = 50,
                                Stroke = Brushes.Black,
                            };

                            
                            Canvas.SetLeft(cell, 10 + j * 50 - rectangleWidth);
                            Canvas.SetTop(cell, 10 + i * 50);

                            canvas.Children.Add(cell);
                        }

                        
                        rectangleWidth = 0;
                        onesCount = 0;
                    }
                }

                
                if (onesCount >= 2)
                {
                    Rectangle lastCell = new Rectangle
                    {
                        Width = rectangleWidth,
                        Height = 50,
                        Stroke = Brushes.Black,
                    };

                    
                    Canvas.SetLeft(lastCell, 10 + (array.GetLength(1) - onesCount) * 50); 
                    Canvas.SetTop(lastCell, 10 + i * 50);

                    canvas.Children.Add(lastCell);
                }

                
                rectangleWidth = 0;
                onesCount = 0;
            }

            
            for (int j = 0; j < array.GetLength(1); j++)
            {
                int startDown = -1;

                for (int i = 0; i < array.GetLength(0); i++)
                {
                    if (array[i, j] == 1)
                    {
                        onesCount++;

                        if (startDown == -1)
                        {
                            startDown = i;
                        }
                    }
                    else if (startDown != -1)
                    {
                       
                        Rectangle cell = new Rectangle
                        {
                            Width = 50,
                            Height = onesCount * 50,
                            Stroke = Brushes.Black,
                        };

                        
                        Canvas.SetLeft(cell, 10 + j * 50);
                        Canvas.SetTop(cell, 10 + startDown * 50);

                        canvas.Children.Add(cell);

                       
                        onesCount = 0;
                        startDown = -1;
                    }
                }

                
                if (onesCount >= 2)
                {
                    Rectangle lastCell = new Rectangle
                    {
                        Width = 50,
                        Height = onesCount * 50, 
                        Stroke = Brushes.Black,
                    };

                    
                    Canvas.SetLeft(lastCell, 10 + j * 50);
                    Canvas.SetTop(lastCell, 10 + startDown * 50);

                    canvas.Children.Add(lastCell);
                }

                
                onesCount = 0;
            }
        }

        private void HandlePlayerMovement(object sender, KeyEventArgs e)
        {
            const int stepSize = 50;

            double newLeft = playerImage.Margin.Left;
            double newTop = playerImage.Margin.Top;

            int arrayRow = (int)((newTop + playerImage.Height / 2) / 50);
            int arrayColumn = (int)((newLeft + playerImage.Width / 2) / 50);

            if (e.Key == Key.W && arrayRow - 1 >= 0 && array[arrayRow - 1, arrayColumn] == 1)
            {
                newTop = Math.Max(playerImage.Margin.Top - stepSize, 0);
            }
            else if (e.Key == Key.A && arrayColumn - 1 >= 0 && array[arrayRow, arrayColumn - 1] == 1)
            {
                newLeft = Math.Max(playerImage.Margin.Left - stepSize, 0);
            }
            else if (e.Key == Key.S && arrayRow + 1 < array.GetLength(0) && array[arrayRow + 1, arrayColumn] == 1)
            {
                newTop = Math.Min(playerImage.Margin.Top + stepSize, Height - playerImage.Height);
            }
            else if (e.Key == Key.D && arrayColumn + 1 < array.GetLength(1) && array[arrayRow, arrayColumn + 1] == 1)
            {
                
                playerImage.Margin = new Thickness(newLeft + stepSize, newTop, 0, 0);

                
                if (arrayRow == array.GetLength(0) - 1 && arrayColumn == array.GetLength(1) - 2)
                {

                    SecondArena secondArena = new SecondArena();
                    secondArena.Show();
                    this.Close();
                    return;
                }

                return; 
            }

            
            playerImage.Margin = new Thickness(newLeft, newTop, 0, 0);
        }

        private int[,] GenerateRandomizedArray(int rows, int columns)
        {
            int[,] array = new int[rows, columns];

            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    array[i, j] = 0;
                }
            }

            
            array[0, 0] = 1;

            Random rand = new Random();

            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (array[i, j] == 1)
                    {
                        
                        if (rand.Next(2) == 0 && i + 1 < rows && array[i + 1, j] == 0)
                        {
                            array[i + 1, j] = 1;
                        }
                        else if (j + 1 < columns && array[i, j + 1] == 0)
                        {
                            array[i, j + 1] = 1;
                        }
                    }
                }
            }

            PrintArray(array);

            return array;
        }

        private void PrintArray(int[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    Console.Write(array[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
    public class SecondArena : Window
    {
        private Canvas myCanvas;
        private Image playerImage;
        private Rectangle arena;
        private Image healthBarImage;
        private List<Image> enemyBullets = new List<Image>();
        private Random random;
        public int healthBar = 5;
        private List<Ellipse> bullets;
        private string bulletImagePath = "C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\dcf7c3bd-7acb-4be7-b251-708c0d1ebada.jpg"; // Replace with the actual path to your bullet image
        private bool canShoot = true;
        private DispatcherTimer shootCooldownTimer;
        private double playerVelocityX = 0.0;
        private double playerVelocityY = 0.0;
        private const double acceleration = 0.5;
        private const double deceleration = 0.1;
        private DispatcherTimer movementTimer;
        private DispatcherTimer decelerationTimer;
        private bool canSpawn = false;
        private List<Enemy> enemies;
        private string enemyImagePath = "C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\крыса.png";
        private Image healthKit;
        private string enemyBulletImagePath = "C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\выстрел-рубик.png";
        public string ArenaBackgroundImagePath { get; set; } = "C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\56bc1fc5-cc74-4470-9b3f-c202a76aff31.jpg";

        public SecondArena()
        {
            Title = "lvl3";
            Height = 1080;
            Width = 1920;
            enemies = new List<Enemy>();
            InitializeComponents();
            bullets = new List<Ellipse>();
            InitializeTimers();

            InitializeMovementTimer();
            /*Button myButton = new Button();
            myButton.Content = "Click me!";

            // Attach an event handler (leave it empty)
            myButton.Click += MyButton_Click;
            myButton.Margin = new Thickness(0, 0, 0, 0);
            myCanvas.Children.Add(myButton);*/

            random = new Random();
            
            

        }

        /*private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            BridgeBetween BridgeBetween = new BridgeBetween();
            BridgeBetween.Show();
            this.Close();
        }*/






        private void GenerateEnemies()
        {
            if (canSpawn)
            {
                canSpawn = false;
                Random random = new Random();

                for (int i = 0; i < 4; i++)
                {
                    Enemy enemy = new Enemy(myCanvas);
                    enemy.InitializeEnemy(random, enemyImagePath, arena);

                    // Add the enemy to the list
                    enemies.Add(enemy);
                }
            }
        }

        private void healthKit_pickedUp()
        {
            double playerCenterX = playerImage.Margin.Left + playerImage.ActualWidth / 2;
            double playerCenterY = playerImage.Margin.Top + playerImage.ActualHeight / 2;

            double healthKitCenterX = healthKit.Margin.Left + healthKit.ActualWidth / 2;
            double healthKitCenterY = healthKit.Margin.Top + healthKit.ActualHeight / 2;

            double distance = Math.Sqrt(Math.Pow(playerCenterX - healthKitCenterX, 2) + Math.Pow(playerCenterY - healthKitCenterY, 2));

            if (distance < (playerImage.ActualWidth + healthKit.ActualWidth) / 2)
            {
                healthKit.Margin = new Thickness(250, 0, 0, 0);
                canSpawn = true;
                GenerateEnemies();
            }



        }


        private bool CheckCollision(UIElement element1, UIElement element2)
        {
            Rect bounds1 = element1.TransformToVisual(myCanvas).TransformBounds(new Rect(0, 0, element1.RenderSize.Width, element1.RenderSize.Height));
            Rect bounds2 = element2.TransformToVisual(myCanvas).TransformBounds(new Rect(0, 0, element2.RenderSize.Width, element2.RenderSize.Height));

            return bounds1.IntersectsWith(bounds2);
        }



        private void InitializeMovementTimer()
        {
            movementTimer = new DispatcherTimer();
            movementTimer.Interval = TimeSpan.FromMilliseconds(16); // Adjust the interval as needed
            movementTimer.Tick += (sender, e) => UpdatePlayerPosition();
            movementTimer.Start();
        }
        private void InitializeTimers()
        {
            shootCooldownTimer = new DispatcherTimer();
            shootCooldownTimer.Interval = TimeSpan.FromSeconds(2); // Set the cooldown time (2 seconds in this example)
            shootCooldownTimer.Tick += (sender, e) => canShoot = true;

            decelerationTimer = new DispatcherTimer();
            decelerationTimer.Interval = TimeSpan.FromMilliseconds(16); // Adjust the interval as needed
            decelerationTimer.Tick += (sender, e) => DeceleratePlayer();



        }



        private void HandlePlayerStop(object sender, KeyEventArgs e)
        {
            // Stop player movement when a movement key is released
            if (e.Key == Key.W || e.Key == Key.S)
            {
                playerVelocityY = 0.0;
            }
            if (e.Key == Key.A || e.Key == Key.D)
            {
                playerVelocityX = 0.0;
            }
        }
        private void InitializeComponents()
        {
            myCanvas = new Canvas();
            Content = myCanvas;

            arena = new Rectangle
            {
                Width = 1600,
                Height = 800,
                Stroke = Brushes.Black,
            };

            arena.Fill = new ImageBrush(new BitmapImage(new Uri(ArenaBackgroundImagePath, UriKind.Relative)));

            arena.Margin = new Thickness((Width - arena.Width) / 2, (Height - arena.Height) / 2, 0, 0);

            playerImage = new Image
            {
                Width = 100,
                Height = 100,
                Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\Снимок экрана 2024-01-02 14024000.png", UriKind.Relative)),
            };

            playerImage.Margin = new Thickness(250,750,0,0);

            healthBarImage = new Image
            {
                Width = 200,
                Height = 30,
                Source = new BitmapImage(new Uri("path/to/your/healthBarImage.png", UriKind.Relative)),
            };

            healthBarImage.Margin = new Thickness((Width - healthBarImage.Width) / 2, 10, 0, 0);

            healthKit = new Image
            {
                Width = 100,
                Height = 100,
                Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\heal_firstaid-57fc8495.png", UriKind.Relative)),
            };
            healthKit.Margin = new Thickness((arena.Width - playerImage.Width) / 2 + arena.Margin.Left,
                                               (arena.Height - playerImage.Height) / 2 + arena.Margin.Top, 0, 0);
            myCanvas.Children.Add(arena);
            myCanvas.Children.Add(playerImage);
            myCanvas.Children.Add(healthKit);
            myCanvas.Children.Add(healthBarImage);



            this.KeyDown += HandlePlayerMovement;
            this.MouseDown += HandleShooting;
        }
        private void UpdatePlayerPosition()
        {
            // Update player position based on velocity
            double newLeft = playerImage.Margin.Left + playerVelocityX;
            double newTop = playerImage.Margin.Top + playerVelocityY;

            // Clamp player position within the arena bounds
            newLeft = Math.Max(Math.Min(newLeft, arena.Margin.Left + arena.Width - playerImage.Width), arena.Margin.Left);
            newTop = Math.Max(Math.Min(newTop, arena.Margin.Top + arena.Height - playerImage.Height), arena.Margin.Top);

            playerImage.Margin = new Thickness(newLeft, newTop, 0, 0);
            healthKit_pickedUp();
            UpdateHealthBarImage();
        }

        private void HandlePlayerMovement(object sender, KeyEventArgs e)
        {

            const double maxSpeed = 5.0; // You can adjust the maximum speed

            if (e.Key == Key.W)
            {
                playerVelocityY = Math.Max(playerVelocityY - acceleration, -maxSpeed);
            }
            if (e.Key == Key.A)
            {
                playerVelocityX = Math.Max(playerVelocityX - acceleration, -maxSpeed);
            }
            if (e.Key == Key.S)
            {
                playerVelocityY = Math.Min(playerVelocityY + acceleration, maxSpeed);
            }
            if (e.Key == Key.D)
            {
                playerVelocityX = Math.Min(playerVelocityX + acceleration, maxSpeed);
            }

            // Update player position based on velocity
            double newLeft = playerImage.Margin.Left + playerVelocityX;
            double newTop = playerImage.Margin.Top + playerVelocityY;

            // Clamp player position within the arena bounds
            newLeft = Math.Max(Math.Min(newLeft, arena.Margin.Left + arena.Width - playerImage.Width), arena.Margin.Left);
            newTop = Math.Max(Math.Min(newTop, arena.Margin.Top + arena.Height - playerImage.Height), arena.Margin.Top);

            playerImage.Margin = new Thickness(newLeft, newTop, 0, 0);
            healthKit_pickedUp();
            decelerationTimer.Start();
            UpdateHealthBarImage();
        }


        private void UpdateHealthBarImage()
        {
            switch (healthBar)
            {
                case 0:
                    this.Close();
                    break;
                case 1:
                    healthBarImage.Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\hp1.jpg"));
                    break;
                case 2:
                    healthBarImage.Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\hp2.jpg"));
                    break;
                case 3:
                    healthBarImage.Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\hp3.jpg"));
                    break;
                case 4:
                    healthBarImage.Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\hp4.jpg"));
                    break;
                case 5:
                    healthBarImage.Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\hp5.jpg"));
                    break;
            }
        }


        private void HandleShooting(object sender, MouseButtonEventArgs e)
        {
            if (canShoot)
            {
                // Get player position
                double playerLeft = playerImage.Margin.Left + playerImage.Width / 2;
                double playerTop = playerImage.Margin.Top + playerImage.Height / 2;

                // Create a new bullet (ellipse) with an image
                Ellipse bullet = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    Fill = new ImageBrush(new BitmapImage(new Uri(bulletImagePath, UriKind.Relative))),
                };

                // Set initial position of the bullet at the player's position
                Canvas.SetLeft(bullet, playerLeft - bullet.Width / 2);
                Canvas.SetTop(bullet, playerTop - bullet.Height / 2);

                // Add the bullet to the canvas
                myCanvas.Children.Add(bullet);

                // Add the bullet to the list
                bullets.Add(bullet);

                // Move the bullet in the direction of the mouse pointer
                MoveBullet(bullet, e.GetPosition(myCanvas));

                canShoot = false; // Set the flag to false
                shootCooldownTimer.Start(); // Start the cooldown timer
            }
        }
        private void DeceleratePlayer()
        {
            // Decelerate the player smoothly
            if (playerVelocityX > 0)
            {
                playerVelocityX = Math.Max(playerVelocityX - deceleration, 0);
            }
            else if (playerVelocityX < 0)
            {
                playerVelocityX = Math.Min(playerVelocityX + deceleration, 0);
            }

            if (playerVelocityY > 0)
            {
                playerVelocityY = Math.Max(playerVelocityY - deceleration, 0);
            }
            else if (playerVelocityY < 0)
            {
                playerVelocityY = Math.Min(playerVelocityY + deceleration, 0);
            }
        }
        private void MoveBullet(Ellipse bullet, Point targetPosition)
        {
            // Calculate the direction vector
            double directionX = targetPosition.X - Canvas.GetLeft(bullet) - bullet.Width / 2;
            double directionY = targetPosition.Y - Canvas.GetTop(bullet) - bullet.Height / 2;

            // Normalize the direction vector
            double length = Math.Sqrt(directionX * directionX + directionY * directionY);
            directionX /= length;
            directionY /= length;

            // Move the bullet in the direction of the mouse pointer
            double speed = 20; // You can adjust the speed

            // Use a DispatcherTimer to update the bullet position
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16); // Update every ~16 milliseconds

            // Define the Tick event handler
            EventHandler timerTick = null;
            timerTick = (sender, e) =>
            {
                Canvas.SetLeft(bullet, Canvas.GetLeft(bullet) + directionX * speed);
                Canvas.SetTop(bullet, Canvas.GetTop(bullet) + directionY * speed);

                // Check if the bullet is out of bounds
                if (Canvas.GetLeft(bullet) < 0 || Canvas.GetTop(bullet) < 0 || Canvas.GetLeft(bullet) > Width || Canvas.GetTop(bullet) > Height)
                {
                    // Remove the bullet from the canvas and the list
                    myCanvas.Children.Remove(bullet);
                    bullets.Remove(bullet);
                    timer.Tick -= timerTick; // Remove the event handler
                    timer.Stop(); // Stop the timer
                }
                else
                {
                    // Check for collisions with enemies
                    foreach (Enemy enemy in enemies.ToList()) // Use ToList() to avoid modification while iterating
                    {
                        if (CheckCollision(enemy.EnemyImage, bullet))
                        {
                            // Remove the bullet from the canvas and the list
                            myCanvas.Children.Remove(bullet);
                            bullets.Remove(bullet);
                            timer.Tick -= timerTick; // Remove the event handler
                            timer.Stop(); // Stop the timer

                            // Decrease enemy health
                            enemy.DecreaseHealth();

                            // Check if the enemy is defeated
                            if (enemy.Health <= 0)
                            {
                                // Remove the enemy from the canvas and the list
                                myCanvas.Children.Remove(enemy.EnemyImage);
                                enemies.Remove(enemy);
                            }

                            // Check if all enemies are defeated
                            if (enemies.Count == 0)
                            {
                                // Close the current window
                                BridgeBetween BridgeBetween = new BridgeBetween();
                                BridgeBetween.Show();
                                this.Close();
                            }

                            break; // Exit the loop since the bullet hit an enemy
                        }
                    }
                }
            };

            // Attach the event handler to the Tick event
            timer.Tick += timerTick;

            // Start the timer
            timer.Start();
        }
    }


    public class BridgeBetween : Window
    {
        private Canvas myCanvas1;
        private int[,] array;
        private Image playerImage;
        private int playerRow;
        private int playerColumn;
        private Label eLabel;

        private bool topRightWhite = false;
        private bool bottomLeftWhite = false;
        private bool canCloseLevel = false;

        private Rectangle topRightRectangle;
        private Rectangle bottomLeftRectangle;
        private Rectangle bottomRightRectangle;
        private Rectangle greenRectangle;

        private int leversPulled = 1;

        public BridgeBetween()
        {
            Title = "lvl4";
            Height = 1080;
            Width = 1920;

            InitializeComponents();
            GenerateRandomPath();
            DrawRectanglesOnCanvas();
            InitializePlayer();
            UpdatePlayerPosition();

            PreviewKeyDown += HandlePlayerMovement;
        }

        private void InitializeComponents()
        {
            myCanvas1 = new Canvas();
            Content = myCanvas1;

            eLabel = new Label
            {
                Content = "e",
                Foreground = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 18,
            };
        }

        private void GenerateRandomPath()
        {
            int rows = 19;
            int columns = 37;

            array = new int[rows, columns];

            
            int middleRow = rows / 2;
            int middleColumn = columns / 2;
            array[middleRow, middleColumn] = 1;

            Random rand = new Random();

            int currentRow = 0;
            int currentColumn = 0;

            
            while (currentRow != middleRow || currentColumn != middleColumn)
            {
                array[currentRow, currentColumn] = 1;

                if (currentRow < middleRow && rand.Next(2) == 1)
                {
                    currentRow++;
                }
                else if (currentColumn < middleColumn)
                {
                    currentColumn++;
                }
            }

            
            currentRow = middleRow;
            currentColumn = middleColumn;

            
            while (currentRow > 0 || currentColumn < columns - 1)
            {
                if (currentRow > 0 && rand.Next(2) == 1)
                {
                    currentRow--;
                }
                else if (currentColumn < columns - 1)
                {
                    currentColumn++;
                }

                array[currentRow, currentColumn] = 1;
            }

            
            currentRow = middleRow;
            currentColumn = middleColumn;

            
            while (currentRow < rows - 1 || currentColumn > 0)
            {
                if (currentRow < rows - 1 && rand.Next(2) == 1)
                {
                    currentRow++;
                }
                else if (currentColumn > 0)
                {
                    currentColumn--;
                }

                array[currentRow, currentColumn] = 1;
            }

            
            currentRow = middleRow;
            currentColumn = middleColumn;

           
            while (currentRow < rows - 1 || currentColumn < columns - 1)
            {
                if (currentRow < rows - 1 && rand.Next(2) == 1)
                {
                    currentRow++;
                }
                else if (currentColumn < columns - 1)
                {
                    currentColumn++;
                }

                array[currentRow, currentColumn] = 1;
            }

            
            PrintArray(array);
        }

        private void DrawRectanglesOnCanvas()
        {
            double cellSize = 50; 

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    
                    if (array[i, j] == 0)
                    {
                        continue;
                    }

                    Rectangle cell = new Rectangle
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Stroke = Brushes.Black,
                    };

                    
                    if (i == 0 && j == array.GetLength(1) - 1)
                    {
                        cell.Fill = Brushes.Gray;
                        topRightRectangle = cell;
                    }
                    
                    else if (i == array.GetLength(0) - 1 && j == 0)
                    {
                        cell.Fill = Brushes.Gray;
                        bottomLeftRectangle = cell;
                    }
                    
                    else if (i == array.GetLength(0) - 1 && j == array.GetLength(1) - 1)
                    {
                        cell.Fill = Brushes.Red;
                        bottomRightRectangle = cell;
                    }

                   
                    double leftMargin = 50;
                    double topMargin = 50;

                    Canvas.SetLeft(cell, j * cellSize + leftMargin);
                    Canvas.SetTop(cell, i * cellSize + topMargin);

                    myCanvas1.Children.Add(cell);
                }
            }
        }

        private void InitializePlayer()
        {
            playerImage = new Image
            {
                Width = 50,
                Height = 50,
                Margin = new Thickness(0, 0, 0, 0),
                Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\Снимок экрана 2024-01-02 14024000.png", UriKind.Relative)), 
            };

            myCanvas1.Children.Add(playerImage);

            playerRow = 0;
            playerColumn = 0;
        }

        private void UpdatePlayerPosition()
        {
            double cellSize = 50;
            double leftMargin = 50;
            double topMargin = 50;

            double newLeft = playerColumn * cellSize + leftMargin;
            double newTop = playerRow * cellSize + topMargin;

            playerImage.Margin = new Thickness(newLeft, newTop, 0, 0);


            if (playerRow == array.GetLength(0) - 1 && playerColumn == array.GetLength(1) - 1 && canCloseLevel)
            {
                TheLastLevel theLastLevel = new TheLastLevel();
                theLastLevel.Show();
                this.Close();
            }

            if (playerRow == 0 && playerColumn == array.GetLength(1) - 1)
            {
                
                Canvas.SetLeft(eLabel, newLeft);
                Canvas.SetTop(eLabel, newTop - 30); 
                myCanvas1.Children.Add(eLabel);
            }
           
            else if (playerRow == array.GetLength(0) - 1 && playerColumn == 0)
            {
                
                Canvas.SetLeft(eLabel, newLeft);
                Canvas.SetTop(eLabel, newTop - 30); 
                myCanvas1.Children.Add(eLabel);
            }
            else
            {
               
                myCanvas1.Children.Remove(eLabel);
            }
        }

        private void HandlePlayerMovement(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    TryMovePlayer(-1, 0);
                    break;
                case Key.A:
                    TryMovePlayer(0, -1);
                    break;
                case Key.S:
                    TryMovePlayer(1, 0);
                    break;
                case Key.D:
                    TryMovePlayer(0, 1);
                    break;
                case Key.E:
                    HandleLeverInteraction();
                    break;
            }
        }

        private void HandleLeverInteraction()
        {
            
            if (playerRow == 0 && playerColumn == array.GetLength(1) - 1)
            {
                
                topRightWhite = true;
                topRightRectangle.Fill = Brushes.White;
                myCanvas1.Children.Remove(eLabel);
            }
            
            else if (playerRow == array.GetLength(0) - 1 && playerColumn == 0)
            {
                
                bottomLeftWhite = true;
                bottomLeftRectangle.Fill = Brushes.White;
                myCanvas1.Children.Remove(eLabel);
            }

            
            if (topRightWhite && bottomLeftWhite)
            {
                myCanvas1.Children.Remove(bottomRightRectangle);

                greenRectangle = new Rectangle
                {
                    Width = bottomRightRectangle.Width,
                    Height = bottomRightRectangle.Height,
                    Stroke = Brushes.Black,
                    Fill = Brushes.Green,
                };

                Canvas.SetLeft(greenRectangle, Canvas.GetLeft(bottomRightRectangle));
                Canvas.SetTop(greenRectangle, Canvas.GetTop(bottomRightRectangle));

                myCanvas1.Children.Add(greenRectangle);

                
                bottomRightRectangle = greenRectangle;

                
                leversPulled++;

                
                if (leversPulled == 2)
                {
                    
                    canCloseLevel = true;
                }
            }
        }

        private void TryMovePlayer(int rowChange, int columnChange)
        {
            int newRow = playerRow + rowChange;
            int newColumn = playerColumn + columnChange;

            if (IsWithinBounds(newRow, newColumn) && array[newRow, newColumn] == 1)
            {
                playerRow = newRow;
                playerColumn = newColumn;
                UpdatePlayerPosition();
            }
        }

        private bool IsWithinBounds(int row, int column)
        {
            return row >= 0 && row < array.GetLength(0) && column >= 0 && column < array.GetLength(1);
        }

        private void PrintArray(int[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    Console.Write(array[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }

    public class TheLastLevel: Window
    {
        private Canvas myCanvas;
        private Image playerImage;
        private Image Bogdan;
        private bool BogdanAlive = true;
        private int bogdanHealth = 5;
        private Rectangle arena;
        private Image healthBarImage;
        private List<Image> enemyBullets = new List<Image>();
        private Random random;
        public int healthBar = 5;
        private List<Ellipse> bullets;
        private string bulletImagePath = "C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\dcf7c3bd-7acb-4be7-b251-708c0d1ebada.jpg"; // Replace with the actual path to your bullet image
        private bool canShoot = true;
        private DispatcherTimer shootCooldownTimer;
        private double playerVelocityX = 0.0;
        private double playerVelocityY = 0.0;
        private const double acceleration = 0.5;
        private const double deceleration = 0.1;
        private DispatcherTimer movementTimer;
        private DispatcherTimer decelerationTimer;
        private DispatcherTimer enemyBulletTimer;
        private List<Enemy> enemies;
        private string enemyImagePath = "C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\pxArt (1).png";

        private string enemyBulletImagePath = "C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\выстрел-богдан.png";
        public string ArenaBackgroundImagePath { get; set; } = "C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\msg6246032635-254877.jpg";

        public TheLastLevel()
        {
            Title = "lvl5";
            Height = 1080;
            Width = 1920;
            enemies = new List<Enemy>();
            InitializeComponents();
            bullets = new List<Ellipse>();
            InitializeTimers();

            InitializeMovementTimer();
            
            GenerateEnemies();
            shooting_Bogdan();
            random = new Random();



        }

        







        private void GenerateEnemies()
        {
            Bogdan = new Image
            {
                Width = 100,
                Height = 100,
                Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\pxArt (1).png", UriKind.Relative)),
                
                
            };
            Bogdan.Margin = new Thickness((arena.Width - playerImage.Width) / 2 + arena.Margin.Left,
                                               (arena.Height - playerImage.Height) / 2 + arena.Margin.Top, 0, 0);
            myCanvas.Children.Add(Bogdan);
            
        }
        private void shooting_Bogdan()
        {
            double bogdanCenterX = Bogdan.Margin.Left + Bogdan.Width / 2;
            double bogdanCenterY = Bogdan.Margin.Top + Bogdan.Height / 2;

            // Number of bullets
            int numberOfBullets = 8;

            // Angle between each bullet
            double angleBetweenBullets = 360.0 / numberOfBullets;

            // Distance from the center of Bogdan to the bullets
            double distance = 50; // Adjust the distance as needed

            for (int i = 0; i < numberOfBullets; i++)
            {
                double angle = i * angleBetweenBullets;

                // Calculate the position of the bullet
                double bulletX = bogdanCenterX + distance * Math.Cos(angle * Math.PI / 180);
                double bulletY = bogdanCenterY + distance * Math.Sin(angle * Math.PI / 180);

                // Create a new bullet (ellipse) with an image
                Ellipse bullet = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    Fill = new ImageBrush(new BitmapImage(new Uri(enemyBulletImagePath, UriKind.Relative))),
                };

                // Set the position of the bullet
                Canvas.SetLeft(bullet, bulletX - bullet.Width / 2);
                Canvas.SetTop(bullet, bulletY - bullet.Height / 2);

                // Add the bullet to the canvas
                myCanvas.Children.Add(bullet);

            }
        }



        private bool CheckCollision(UIElement element1, UIElement element2)
        {
            Rect bounds1 = element1.TransformToVisual(myCanvas).TransformBounds(new Rect(0, 0, element1.RenderSize.Width, element1.RenderSize.Height));
            Rect bounds2 = element2.TransformToVisual(myCanvas).TransformBounds(new Rect(0, 0, element2.RenderSize.Width, element2.RenderSize.Height));

            return bounds1.IntersectsWith(bounds2);
        }



        private void InitializeMovementTimer()
        {
            movementTimer = new DispatcherTimer();
            movementTimer.Interval = TimeSpan.FromMilliseconds(16); // Adjust the interval as needed
            movementTimer.Tick += (sender, e) => UpdatePlayerPosition();
            movementTimer.Start();
        }
        private void InitializeTimers()
        {
            shootCooldownTimer = new DispatcherTimer();
            shootCooldownTimer.Interval = TimeSpan.FromSeconds(2); // Set the cooldown time (2 seconds in this example)
            shootCooldownTimer.Tick += (sender, e) => canShoot = true;

            decelerationTimer = new DispatcherTimer();
            decelerationTimer.Interval = TimeSpan.FromMilliseconds(16); // Adjust the interval as needed
            decelerationTimer.Tick += (sender, e) => DeceleratePlayer();

            


        }
        


        private void HandlePlayerStop(object sender, KeyEventArgs e)
        {
            // Stop player movement when a movement key is released
            if (e.Key == Key.W || e.Key == Key.S)
            {
                playerVelocityY = 0.0;
            }
            if (e.Key == Key.A || e.Key == Key.D)
            {
                playerVelocityX = 0.0;
            }
        }
        private void InitializeComponents()
        {
            myCanvas = new Canvas();
            Content = myCanvas;

            arena = new Rectangle
            {
                Width = 1600,
                Height = 800,
                Stroke = Brushes.Black,
            };

            arena.Fill = new ImageBrush(new BitmapImage(new Uri(ArenaBackgroundImagePath, UriKind.Relative)));

            arena.Margin = new Thickness((Width - arena.Width) / 2, (Height - arena.Height) / 2, 0, 0);

            playerImage = new Image
            {
                Width = 100,
                Height = 100,
                Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\Снимок экрана 2024-01-02 14024000.png", UriKind.Relative)),
            };

            playerImage.Margin = new Thickness(0,750,0,0);

            healthBarImage = new Image
            {
                Width = 200,
                Height = 30,
                Source = new BitmapImage(new Uri("path/to/your/healthBarImage.png", UriKind.Relative)),
            };

            healthBarImage.Margin = new Thickness((Width - healthBarImage.Width) / 2, 10, 0, 0);

            myCanvas.Children.Add(arena);
            myCanvas.Children.Add(playerImage);
            myCanvas.Children.Add(healthBarImage);



            this.KeyDown += HandlePlayerMovement;
            this.MouseDown += HandleShooting;
        }
        private void UpdatePlayerPosition()
        {
            // Update player position based on velocity
            double newLeft = playerImage.Margin.Left + playerVelocityX;
            double newTop = playerImage.Margin.Top + playerVelocityY;

            // Clamp player position within the arena bounds
            newLeft = Math.Max(Math.Min(newLeft, arena.Margin.Left + arena.Width - playerImage.Width), arena.Margin.Left);
            newTop = Math.Max(Math.Min(newTop, arena.Margin.Top + arena.Height - playerImage.Height), arena.Margin.Top);

            playerImage.Margin = new Thickness(newLeft, newTop, 0, 0);
            
            UpdateHealthBarImage();
        }

        private void HandlePlayerMovement(object sender, KeyEventArgs e)
        {

            const double maxSpeed = 5.0; // You can adjust the maximum speed

            if (e.Key == Key.W)
            {
                playerVelocityY = Math.Max(playerVelocityY - acceleration, -maxSpeed);
            }
            if (e.Key == Key.A)
            {
                playerVelocityX = Math.Max(playerVelocityX - acceleration, -maxSpeed);
            }
            if (e.Key == Key.S)
            {
                playerVelocityY = Math.Min(playerVelocityY + acceleration, maxSpeed);
            }
            if (e.Key == Key.D)
            {
                playerVelocityX = Math.Min(playerVelocityX + acceleration, maxSpeed);
            }

            // Update player position based on velocity
            double newLeft = playerImage.Margin.Left + playerVelocityX;
            double newTop = playerImage.Margin.Top + playerVelocityY;

            // Clamp player position within the arena bounds
            newLeft = Math.Max(Math.Min(newLeft, arena.Margin.Left + arena.Width - playerImage.Width), arena.Margin.Left);
            newTop = Math.Max(Math.Min(newTop, arena.Margin.Top + arena.Height - playerImage.Height), arena.Margin.Top);

            playerImage.Margin = new Thickness(newLeft, newTop, 0, 0);
            decelerationTimer.Start();
            UpdateHealthBarImage();
        }


        private void UpdateHealthBarImage()
        {
            switch (healthBar)
            {
                case 0:
                    this.Close();
                    break;
                case 1:
                    healthBarImage.Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\hp1.jpg"));
                    break;
                case 2:
                    healthBarImage.Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\hp2.jpg"));
                    break;
                case 3:
                    healthBarImage.Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\hp3.jpg"));
                    break;
                case 4:
                    healthBarImage.Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\hp4.jpg"));
                    break;
                case 5:
                    healthBarImage.Source = new BitmapImage(new Uri("C:\\Users\\Artelpc\\source\\repos\\WpfApp3\\WpfApp3\\hp5.jpg"));
                    break;
            }
        }


        private void HandleShooting(object sender, MouseButtonEventArgs e)
        {
            if (canShoot)
            {
                // Get player position
                double playerLeft = playerImage.Margin.Left + playerImage.Width / 2;
                double playerTop = playerImage.Margin.Top + playerImage.Height / 2;

                // Create a new bullet (ellipse) with an image
                Ellipse bullet = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    Fill = new ImageBrush(new BitmapImage(new Uri(bulletImagePath, UriKind.Relative))),
                };

                // Set initial position of the bullet at the player's position
                Canvas.SetLeft(bullet, playerLeft - bullet.Width / 2);
                Canvas.SetTop(bullet, playerTop - bullet.Height / 2);

                // Add the bullet to the canvas
                myCanvas.Children.Add(bullet);

                // Add the bullet to the list
                bullets.Add(bullet);

                // Move the bullet in the direction of the mouse pointer
                MoveBullet(bullet, e.GetPosition(myCanvas));

                canShoot = false; // Set the flag to false
                shootCooldownTimer.Start(); // Start the cooldown timer
            }
        }
        private void DeceleratePlayer()
        {
            // Decelerate the player smoothly
            if (playerVelocityX > 0)
            {
                playerVelocityX = Math.Max(playerVelocityX - deceleration, 0);
            }
            else if (playerVelocityX < 0)
            {
                playerVelocityX = Math.Min(playerVelocityX + deceleration, 0);
            }

            if (playerVelocityY > 0)
            {
                playerVelocityY = Math.Max(playerVelocityY - deceleration, 0);
            }
            else if (playerVelocityY < 0)
            {
                playerVelocityY = Math.Min(playerVelocityY + deceleration, 0);
            }
        }
        private void MoveBullet(Ellipse bullet, Point targetPosition)
        {
            // Calculate the direction vector
            double directionX = targetPosition.X - Canvas.GetLeft(bullet) - bullet.Width / 2;
            double directionY = targetPosition.Y - Canvas.GetTop(bullet) - bullet.Height / 2;

            // Normalize the direction vector
            double length = Math.Sqrt(directionX * directionX + directionY * directionY);
            directionX /= length;
            directionY /= length;

            // Move the bullet in the direction of the mouse pointer
            double speed = 20; // You can adjust the speed

            // Use a DispatcherTimer to update the bullet position
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16); // Update every ~16 milliseconds

            // Define the Tick event handler
            EventHandler timerTick = null;
            timerTick = (sender, e) =>
            {
                Canvas.SetLeft(bullet, Canvas.GetLeft(bullet) + directionX * speed);
                Canvas.SetTop(bullet, Canvas.GetTop(bullet) + directionY * speed);

                // Check if the bullet is out of bounds
                if (Canvas.GetLeft(bullet) < 0 || Canvas.GetTop(bullet) < 0 || Canvas.GetLeft(bullet) > Width || Canvas.GetTop(bullet) > Height)
                {
                    // Remove the bullet from the canvas and the list
                    myCanvas.Children.Remove(bullet);
                    bullets.Remove(bullet);
                    timer.Tick -= timerTick; // Remove the event handler
                    timer.Stop(); // Stop the timer
                }
                else
                {
                    // Check for collisions with enemies
                    if (CheckCollision(Bogdan, bullet))
                    {
                        // Remove the bullet from the canvas and the list
                        myCanvas.Children.Remove(bullet);
                        bullets.Remove(bullet);
                        timer.Tick -= timerTick; // Remove the event handler
                        timer.Stop(); // Stop the timer

                        // Decrease enemy health
                        bogdanHealth--;

                        // Check if the enemy is defeated
                        if (bogdanHealth <= 0)
                        {
                            // Remove the enemy from the canvas and the list
                            myCanvas.Children.Remove(Bogdan);
                            this.Close();
                        }



                         
                    }
                }
            };

            // Attach the event handler to the Tick event
            timer.Tick += timerTick;

            // Start the timer
            timer.Start();
        }
    }









}
