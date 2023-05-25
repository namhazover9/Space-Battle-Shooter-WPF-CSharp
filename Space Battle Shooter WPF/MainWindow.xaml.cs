using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading; // add this for the timer

namespace Space_Battle_Shooter_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // make a game timer
        DispatcherTimer gameTimer = new DispatcherTimer();
        // move left and move right boolean declarcation
        bool moveLeft, moveRight;
        // make a new items remove list
        List<Rectangle> itemstoremove = new List<Rectangle>();
        // make a new random class to generate random numbers from
        Random rand = new Random();

        int enemySpriteCounter; // int to help change enemy images
        int enemyCounter = 100; // enemy spawn time
        int playerSpeed = 10; //player movement speed
        int limit = 50; // limit of enemy spawn
        int score = 0; // default score
        int damage = 0; // default damgage

        Rect playerHitBox; // player hit box to check for collision against enemy

        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            // link the game engine event to the timer
            gameTimer.Tick += gameEngine;
            // start the timer
            gameTimer.Start();
            // make my canvas focus of this game
            MyCanvas.Focus();

            // make a new image brush instance called bg
            ImageBrush bg = new ImageBrush();
            // pas the purple image as the bg image source
            bg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/purple.png"));
            // set bg to tile mode
            bg.TileMode = TileMode.Tile;
            // set the height and width of bg image brush
            bg.Viewport = new Rect(0, 0, 0.15, 0.15);
            // set the bg view port unit
            bg.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            // assign bg as the background of my canvas
            MyCanvas.Background = bg;

            // make a player image, image brush
            ImageBrush playerImage= new ImageBrush();
            // load the playerimage into it
            playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player.png"));
            // assign the player to the plater rectangle fill
            player.Fill = playerImage;
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left) // if the left key is pressed set move left to true
            {
                moveLeft = true;
            }
            if (e.Key == Key.Right) // if the right key is pressed set move right to true
            {
                moveRight = true;
            }
        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left) // if the left key is released set move left to false
            {
                moveLeft = false;
            }

            if (e.Key == Key.Right) // if the right key is released set move right to false
            {
                moveRight = false;
            }

            /* if the space key is released:
            - make a new rectangle called new bullet,
            - give this rectangle a tag called bullet,
            - height set to 20, witdth set to 5,
            - background color white and border color red */
            if (e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };

                // place the bullet on top of the player location
                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);
                // place the bullet middle of the player image
                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);
                // add the bullet to the screen
                MyCanvas.Children.Add(newBullet);
            }


        }

        private void MakeEnemies()
        {
            // this function will make the enemies for us including assigning their images
            ImageBrush enemySprite = new ImageBrush(); // make a new image brush called enemy sprite
            enemySpriteCounter = rand.Next(1, 6);      // generate a random number inside the ennemy sprite counter
            
            // Below, switch statement will check what number is generated inside the enemy sprite counter and then assign a new image to the enemy sprite image brush depending on the number

            switch (enemySpriteCounter)
            {
                case 1:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/1.png"));
                    break;
                case 2:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/2.png"));
                    break;
                case 3:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/3.png"));
                    break;
                case 4:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/4.png"));
                    break;
                case 5:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/5.png"));
                    break;
                default:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/1.png"));
                    break;
            }
            
            // make a new rectangle called new enemy, this rectangle has a enemy tag, height 50 & width 56px, background fill is assigned to the randomly enemy sprite from the switch
            Rectangle newEnemy = new Rectangle
            {
                Tag = "enemy",
                Height = 50,
                Width = 56,
                Fill = enemySprite
            };

            Canvas.SetTop(newEnemy, -100); // set the top position of the enemy to -100
            Canvas.SetLeft(newEnemy, rand.Next(30, 430)); // randomly generate the left position of the enemy
            MyCanvas.Children.Add(newEnemy);

            // garbage collection
            GC.Collect(); // collect any unused resources for this game
        }
        private void gameEngine(object sender, EventArgs e)
        {
            // link the player hit box to the player rectangle
            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            // reduce one from the enemy counter integer
            enemyCounter--;
            scoreText.Content = "Score: " + score; // link the score text to score integer
            damageText.Content = "Damaged " + damage; // link the damage text to damage integer
            // if enemy counter is less than 0
            if (enemyCounter < 0)
            {
                MakeEnemies(); // run the make enemies function
                enemyCounter = limit; //reset the enemy counter to the limit integer
            }
            // player movement begins
            if (moveLeft && Canvas.GetLeft(player) > 0)
            {
                // if move left is true AND player is inside the boundary then move player to the left
                Canvas.SetLeft(player, Canvas.GetLeft(player) - playerSpeed);
            }
            if (moveRight && Canvas.GetLeft(player) + 90 < Application.Current.MainWindow.Width)
            {
                // if move right is true AND player left + 90 is less than the width of the form
                // then move the player to the right
                Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
            }
            // player movement ends
            // search for bullets, enemies and collision begins
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                // if any rectangle has the tag bullet in it
                if (x is Rectangle && (string)x.Tag == "bullet")
                {
                    // move the bullet rectangle towards top of the screen
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);
                    // make a rect class with the bullet rectangles properties
                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    // check if bullet has reached top part of the screen
                    if (Canvas.GetTop(x) < 10)
                    {
                        // if it has then add it to the item to remove list
                        itemstoremove.Add(x);
                    }
                    // run another for each loop inside of the main loop this one has a local variable called y
                    foreach (var y in MyCanvas.Children.OfType<Rectangle>())
                    {
                        // if y is a rectangle and it has a tag called enemy
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {
                            // make a local rect called enemy and put the enemies properties into it
                            Rect enemy = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);
                            // now check if bullet and enemy is colliding or not
                            // if the bullet is colliding with the enemy rectangle
                            if (bullet.IntersectsWith(enemy))
                            {
                                itemstoremove.Add(x); // remove bullet
                                itemstoremove.Add(y); // remove enemy
                                score++; // add one to the score
                            }
                        }
                    }
                }
                // outside the second loop lets check for the enemy again
                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    // if we find a rectangle with the enemy tag
                    Canvas.SetTop(x, Canvas.GetTop(x) + 10); // move the enemy downwards
                    // make a new enemy rect for enemy hit box
                    Rect enemy = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    // first check if the enemy object has gone passed the player meaning
                    // its gone passed 700 pixels from the top
                    if (Canvas.GetTop(x) + 150 > 700)
                    {
                        // if so first remove the enemy object
                        itemstoremove.Add(x);
                        damage += 10; // add 10 to the damage
                    }
                    // if the player hit box and the enemy is colliding 
                    if (playerHitBox.IntersectsWith(enemy))
                    {
                        damage += 5; // add 5 to the damage
                        itemstoremove.Add(x); // remove the enemy object
                    }
                }
            }
            // search for bullets, enemies and collision ENDs
            // if the score is greater than 5
            if (score > 5)
            {
                limit = 20; // reduce the limit to 20
                // now the enemies will spawn faster
            }
            // if the damage integer is greater than 99
            if (damage > 99)
            {
                gameTimer.Stop(); // stop the main timer
                damageText.Content = "Damaged: 100"; // show this on the damaged text
                damageText.Foreground = Brushes.Red; // change the text colour to 100
                MessageBox.Show("Well Done Star Captain!" + Environment.NewLine + "You have destroyed " + score + " Alien ships");
                // show the message box with the message inside of it
            }
            // removing the rectangles
            // check how many rectangles are inside of the item to remove list
            foreach (Rectangle y in itemstoremove)
            {
                // remove them permanently from the canvas
                MyCanvas.Children.Remove(y);
            }
        }
    }
}
