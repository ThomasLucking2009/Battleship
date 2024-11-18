using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        private Timer timer;
        private int seconds;
        private Label timerLabel;
        private Label Attemptedtries;
        private Label shipsunken;
        private Label hitcount;
        private Button btnRestart;
        private Label[,] labels;  // 2D array to store the labels
        private Random random = new Random();  // Random generator
        private List<List<Label>> ships = new List<List<Label>>(); // List to store ships
        public Form1()

        {
            

            InitializeComponent();
            labels = new Label[10, 10];  // Initialize the 2D array of labels
            InitializeLabels();
            PlaceShips();
            btnRestart = new Button();
            btnRestart.Text = "Restart";
            btnRestart.Size = new Size(100, 40);
            btnRestart.Location = new Point(375, 40); // You can adjust the location as needed
            btnRestart.Click += BtnRestart_Click; // Attach the click event handler

            this.Controls.Add(btnRestart);
            hitcount = new Label();
            hitcount.Text = "Hit Count: ";
            hitcount.Size = new Size(100, 40);
            hitcount.Location = new Point(550, 40);
            this.Controls.Add(hitcount);

            shipsunken = new Label();
            shipsunken.Text = "Ships Sunken: ";
            shipsunken.Size = new Size(100, 40);
            shipsunken.Location = new Point(550, 80);
            this.Controls.Add(shipsunken);

            Attemptedtries = new Label();
            Attemptedtries.Text = "Attempted Tries: ";
            Attemptedtries.Size = new Size(100, 40);
            Attemptedtries.Location = new Point(550, 120);
            this.Controls.Add(Attemptedtries);

            timerLabel = new Label();
            timerLabel.Text = "Time: 00:00";
            timerLabel.Font = new Font("Arial", 20);
            timerLabel.Size = new Size(200, 50);
            timerLabel.Location = new Point(550, 160);  // Position it where you need on the form
            this.Controls.Add(timerLabel);

            // Initialize the Timer
            timer = new Timer();
            timer.Interval = 1000;  // Timer will tick every second (1000 milliseconds)
            timer.Tick += Timer_Tick;  // Subscribe to the Tick event

            seconds = 0;  // Initialize the time counter






        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartTimer();

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            seconds++;  // Increment the seconds counter

            // Calculate minutes and seconds
            int minutes = seconds / 60;
            int displaySeconds = seconds % 60;

            // Update the label to display the current time
            timerLabel.Text = $"Time: {minutes:D2}:{displaySeconds:D2}";
        }
        private void StartTimer()
        {
            timer.Start();  // Start the timer
        }
        private void ShowMessageBox()
        {
            // Show message box with Yes and No buttons
            DialogResult result = MessageBox.Show("Would you like to save your score?", "Confirm", MessageBoxButtons.YesNo);

            // Check the result of the message box
            if (result == DialogResult.Yes)
            {
                using (StreamWriter writetext = new StreamWriter("C:\\Users\\tholucking\\Desktop\\History.txt", append: true))
                {
                    seconds++;  // Increment the seconds counter

                    // Calculate minutes and seconds
                    int minutes = seconds / 60;
                    int displaySeconds = seconds % 60;

                    writetext.WriteLine("Your score is " + attemptedtries);
                    writetext.WriteLine($"Time: {minutes:D2}:{displaySeconds:D2}");


                    Attemptedtries.Text = "Attempted tries: 0";
                    attemptedtries = 0;
                    seconds = -1;

                }
            }
            else if (result == DialogResult.No)
            {
                ResetGrid();
                PlaceShips();
                shipsunken.Text = "Ships Sunken: 0 ";
                hitcount.Text = "Hit Count: 0  ";
                Attemptedtries.Text = "Attempted tries: 0";
                totalshipssunken = 0;
                totalHits = 0;
                attemptedtries = 0;
                seconds = -1;

                // Optionally, reset other game state variables like score, Aturn, etc.
                // Example: score = 0;
                // Update any UI components to reflect the restart state
                MessageBox.Show("The game has automatically been restarted");
            }
        }


        private void InitializeLabels()
        {
            int labelWidth = 30;
            int labelHeight = 30;
            int margin = 5;

            // Create and place labels in a grid
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    labels[row, col] = new Label();
                    labels[row, col].Size = new Size(labelWidth, labelHeight);
                    labels[row, col].Location = new Point(col * (labelWidth + margin), row * (labelHeight + margin));
                    labels[row, col].TextAlign = ContentAlignment.MiddleCenter;
                    labels[row, col].BackColor = Color.LightBlue; // Default color
                    labels[row, col].Click += Label_Click;
                    this.Controls.Add(labels[row, col]);
                }
            }

        }
        private int attemptedtries = 0;
        private void Label_Click(object sender, EventArgs e)
        {
            Label clickedLabel = sender as Label;

            // If the clicked label is part of a sunk ship (black color), do nothing
            if (clickedLabel != null && clickedLabel.BackColor == Color.Black)
            {
                return; // Skip the label click action if the ship part is already sunk (black)
            }

            // Only process the click if the label is not marked as sunk (not black)
            if (clickedLabel != null && clickedLabel.BackColor != Color.White)
            {
                clickedLabel.BackColor = Color.White;
                attemptedtries++;
                Attemptedtries.Text = "Attempted Tries:  " + attemptedtries;  // Update attempts
            }
        }


        // Place all ships on the grid
        private void PlaceShips()
        {
            Torpedo();
            AircraftCarrier();
            Cruiser();
            Submarine();
            Destroyer();
        }

        private void Torpedo()
        {
            TryToPlaceShip(2, Color.LightBlue);
        }

        private void AircraftCarrier()
        {
            TryToPlaceShip(5, Color.LightBlue);
        }

        private void Cruiser()
        {
            TryToPlaceShip(4, Color.LightBlue);
        }

        private void Submarine()
        {
            TryToPlaceShip(3, Color.LightBlue);
        }

        private void Destroyer()
        {
            TryToPlaceShip(3, Color.LightBlue);
        }

        // Tries to place a ship of the given size and color
        private void TryToPlaceShip(int shipSize, Color shipColor)
        {
            int rows = 10;
            int cols = 10;
            bool isHorizontal = random.Next(2) == 0;  // Randomly choose orientation
            bool canPlace = false;
            int retries = 0;
            int maxRetries = 10000;

            while (!canPlace && retries < maxRetries)
            {
                int row = random.Next(rows);
                int col = random.Next(cols);

                // If the ship is horizontal, check if it fits within bounds and if the space is unoccupied
                if (isHorizontal)
                {
                    if (col + shipSize <= cols && !IsOccupied(row, col, shipSize, isHorizontal))
                    {
                        PlaceShip(row, col, shipSize, isHorizontal, shipColor);
                        canPlace = true;
                    }
                }
                // If the ship is vertical, check if it fits within bounds and if the space is unoccupied
                else
                {
                    if (row + shipSize <= rows && !IsOccupied(row, col, shipSize, isHorizontal))
                    {
                        PlaceShip(row, col, shipSize, isHorizontal, shipColor);
                        canPlace = true;
                    }
                }

                retries++;
            }

            if (!canPlace)
            {
                MessageBox.Show($"Unable to place ship of size {shipSize} after {maxRetries} attempts.");
            }
        }

        // Checks if any cells are occupied for the given ship size and orientation
        private bool[,] gridOccupied = new bool[10, 10];  // Track occupied cells (10x10 grid)

        // Checks if the area is occupied
        private bool IsOccupied(int row, int col, int shipSize, bool isHorizontal)
        {
            if (isHorizontal)
            {
                // Check if the ship goes out of bounds
                if (col + shipSize > 10) return true;

                // Check each cell in the horizontal placement of the ship
                for (int i = 0; i < shipSize; i++)
                {
                    if (gridOccupied[row, col + i]) return true;  // Occupied by another ship
                }
            }
            else
            {
                // Check if the ship goes out of bounds
                if (row + shipSize > 10) return true;

                // Check each cell in the vertical placement of the ship
                for (int i = 0; i < shipSize; i++)
                {
                    if (gridOccupied[row + i, col]) return true;  // Occupied by another ship
                }
            }
            return false;  // No overlap detected
        }

        // Places the ship on the grid
        private void PlaceShip(int row, int col, int shipSize, bool isHorizontal, Color shipColor)
        {
            // Check if the placement is valid (i.e., not occupied)
            if (IsOccupied(row, col, shipSize, isHorizontal))
            {
                MessageBox.Show("Cannot place ship here! The area is already occupied or out of bounds.");
                return; // Don't place the ship if it's occupied
            }

            // Proceed with placing the ship
            List<Label> shipLabels = new List<Label>();
            if (isHorizontal)
            {
                for (int i = 0; i < shipSize; i++)
                {
                    labels[row, col + i].BackColor = shipColor;  // Change the label's background color to the ship's color
                    shipLabels.Add(labels[row, col + i]);  // Add the label to the ship's list
                    labels[row, col + i].Click += shiphit;  // Add the click event handler for ship hit
                    gridOccupied[row, col + i] = true;  // Mark the cell as occupied
                }
            }
            else
            {
                for (int i = 0; i < shipSize; i++)
                {
                    labels[row + i, col].BackColor = shipColor;  // Change the label's background color to the ship's color
                    shipLabels.Add(labels[row + i, col]);  // Add the label to the ship's list
                    labels[row + i, col].Click += shiphit;  // Add the click event handler for ship hit
                    gridOccupied[row + i, col] = true;  // Mark the cell as occupied
                }
            }

            // Add the ship's labels to the ships list
            ships.Add(shipLabels);
        }
        private int totalshipssunken = 0;
        private int totalHits = 0; // Track the total number of hits

        private void shiphit(object sender, EventArgs e)
        {
            Label clickedLabel = sender as Label;

            if (clickedLabel != null)
            {
                // Iterate over each ship and check if the clicked label is part of that ship
                foreach (var ship in ships)
                {
                    if (ship.Contains(clickedLabel)) // If the clicked label is part of this ship
                    {


                        // If the label is not already marked as hit, mark it as hit
                        if (clickedLabel.BackColor != Color.Blue)
                        {
                            clickedLabel.BackColor = Color.Blue;
                            totalHits++; // Increment the global hit counter
                            hitcount.Text = "Hits: " + totalHits;


                            bool allHit = true;
                            foreach (var part in ship)
                            {
                                if (part.BackColor != Color.Blue)
                                {
                                    allHit = false;
                                    break;
                                }

                            }

                            if (allHit)
                            {
                                totalshipssunken += 1;
                                shipsunken.Text = "Ships Sunken: " + totalshipssunken;


                                foreach (var part in ship)
                                {
                                    part.BackColor = Color.Black;
                                    part.Click -= shiphit;
                                    part.BackColor = Color.Black;
                                    // Change the color of the whole ship to Black (sunk)
                                }

                                if(totalshipssunken == 5)
                                {

                                    shipsunken.Text = "Ships Sunken: 0 ";
                                    hitcount.Text = "Hit Count: 0  ";
                                    totalshipssunken = 0;
                                    totalHits = 0;

                                    ShowMessageBox();
                                    Application.Restart();
                                }
                                

                                MessageBox.Show("You sank a ship!");
                                // Notify the player that the ship is sunk
                            }
                        }
                        break; // Exit once we process the hit for this ship
                    }
                }
            }
        }

        private void BtnRestart_Click(object sender, EventArgs e)
        {
            // Clear the grid (reset the colors of the labels)
            ShowMessageBox();
            Application.Restart();

        }

        private void ResetGrid()
        {
            // Loop through each label and reset its color to the initial state (LightBlue)
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    labels[row, col].BackColor = Color.LightBlue; // Reset the label color
                }
            }
        }
    }



}


