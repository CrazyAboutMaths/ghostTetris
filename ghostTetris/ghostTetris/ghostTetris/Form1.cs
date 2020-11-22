using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ghostTetris
{
    public partial class ghostTetris : Form
    {
        Grid myGrid;
        public char keyPress;
        public bool spacePress = false;
        public bool end = false;
        private int tickCounter;
        public int passedTime = 0;

        //Read Input from keyboard
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Space)
            {
                spacePress = true;
                Console.WriteLine("Space pressed");
            }
            else if (keyData == Keys.Up)
            {
                keyPress = 'U';
                myGrid.rotatePlayer();
            }
            else if (keyData == Keys.Down)
            {
                keyPress = 'D';
                myGrid.movePlayerBlock('D');
                Console.WriteLine("Down pressed");
            }
            else if (keyData == Keys.Left)
            {
                keyPress = 'L';
                myGrid.movePlayerBlock('L');
                Console.WriteLine("Left pressed");
            }
            else if (keyData == Keys.Right)
            {
                keyPress = 'R';
                myGrid.movePlayerBlock('R');
                Console.WriteLine("Right pressed");
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        //when key is no longer pressed
        public void Form1_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyValue == (char)Keys.Space)
            {
                spacePress = false;
            }
            else
            {
                keyPress = ' ';
            }
        }

        public ghostTetris()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void ghostTetris_Load(object sender, EventArgs e)
        {
            //initialize grid, 
            myGrid = new Grid();
            myGrid.initialise(this);
            GameLoop();
        }

        public async void GameLoop()
        {
            //loop the game
            do
            {
                await Task.Delay(120);
                OneTick();
            } while (end == false);
        }

        private void OneTick()
        {
            //move pieces
            tickCounter++;
            if (tickCounter % 3 == 0)
            {
                myGrid.movePlayerBlock('D');
                timer1_Tick();
            }
            myGrid.moveGhost();
        }

        private void timer1_Tick()
        {
            passedTime++;
        }
    }

    public class Grid
    {
        static Form myForm;
        static Cords[,] shapes;
        Panel bulbasaur;
        Block[,] blockArray;
        Ghost[] ghostArray;
        System.Media.SoundPlayer[] placeSounds;
        PlayerBlock curPlayer;
        int gridMaxX = 11;
        int gridMaxY = 20;
        int ghostNum = 5;

        public void initialise(Form igloo)
        {
            myForm = igloo;

            bulbasaur = new Panel();
            bulbasaur.Size = new Size(myForm.Size.Width, myForm.Size.Height - 30);
            bulbasaur.Location = new Point(0, 0);
            bulbasaur.BackgroundImage = new Bitmap("../../../images/winScreen.png");
            bulbasaur.BackgroundImageLayout = ImageLayout.Stretch;
            bulbasaur.Visible = false;
            myForm.Controls.Add(bulbasaur);

            //Setting up the player
            PlayerBlock.initialiseThis(gridMaxY, gridMaxX);
            curPlayer = new PlayerBlock();

            //Set up block class to be usable
            Block.blockInitialise(gridMaxX, gridMaxY, myForm.Size.Width, myForm.Size.Height, myForm);
            Block.changePlayerColour(curPlayer.getColour());

            //Set up the 2D array
            blockArray = new Block[gridMaxX, gridMaxY];

            for (int i = 0; i < gridMaxX; i++)
            {
                for (int counter = 0; counter < gridMaxY; counter++)
                {
                    blockArray[i, counter] = new Block(i, counter);
                }
            }

            //Setting up ghost
            Ghost.setGrid(this);
            ghostArray = new Ghost[ghostNum];
            for (int i = 0; i < ghostNum; i++)
            {
                ghostArray[i] = new Ghost(gridMaxX, gridMaxY, i);
            }

            //Setting up shapes
            shapes = new Cords[7, 4];
            //box
            shapes[0, 0] = new Cords(1, 0);
            shapes[0, 1] = new Cords(1, 1);
            shapes[0, 2] = new Cords(0, 1);
            shapes[0, 3] = new Cords(0, 0);
            //
            shapes[1, 0] = new Cords(0, 1);
            shapes[1, 1] = new Cords(0, 2);
            shapes[1, 2] = new Cords(0, 3);
            shapes[1, 3] = new Cords(0, 0);
            //
            shapes[2, 0] = new Cords(1, 0);
            shapes[2, 1] = new Cords(0, 1);
            shapes[2, 2] = new Cords(0, 2);
            shapes[2, 3] = new Cords(0, 0);
            //
            shapes[3, 0] = new Cords(1, 0);
            shapes[3, 1] = new Cords(1, 1);
            shapes[3, 2] = new Cords(1, 2);
            shapes[3, 3] = new Cords(0, 0);
            //
            shapes[4, 0] = new Cords(1, 0);
            shapes[4, 1] = new Cords(1, 1);
            shapes[4, 2] = new Cords(2, 1);
            shapes[4, 3] = new Cords(0, 0);
            //
            shapes[5, 0] = new Cords(0, 2);
            shapes[5, 1] = new Cords(1, 1);
            shapes[5, 2] = new Cords(0, 1);
            shapes[5, 3] = new Cords(0, 0);
            //
            shapes[6, 0] = new Cords(1, 1);
            shapes[6, 1] = new Cords(0, 1);
            shapes[6, 2] = new Cords(0, 2);
            shapes[6, 3] = new Cords(0, 0);

            placeSounds = new System.Media.SoundPlayer[8];
            for (int i = 0; i < 8; i++)
            {
                placeSounds[i] = new System.Media.SoundPlayer("../../../sounds/place" + (i + 1) + ".wav");
            }

            drawPlayer();
        }
        //get rotated shape cords
        public Cords[] rotator(int rotation = -1)
        {
            Cords[] returner = new Cords[4];
            if (rotation == -1) { rotation = curPlayer.getRotate(); }
            switch (rotation)
            {
                case (0):
                    returner[0] = shapes[curPlayer.getShape(), 0];
                    returner[1] = shapes[curPlayer.getShape(), 1];
                    returner[2] = shapes[curPlayer.getShape(), 2];
                    break;
                case (1):
                    for (int i = 0; i < 3; i++)
                    {
                        returner[i] = new Cords(shapes[curPlayer.getShape(), i].getY() * -1, shapes[curPlayer.getShape(), i].getX());
                    }
                    break;
                case (2):
                    for (int i = 0; i < 3; i++)
                    {
                        returner[i] = new Cords(shapes[curPlayer.getShape(), i].getX() * -1, shapes[curPlayer.getShape(), i].getY() * -1);
                    }
                    break;
                case (3):
                    for (int i = 0; i < 3; i++)
                    {
                        returner[i] = new Cords(shapes[curPlayer.getShape(), i].getY(), shapes[curPlayer.getShape(), i].getX() * -1);
                    }
                    break;
            }
            returner[3] = new Cords(0, 0);

            return (returner);
        }
        //Draws the player on the grid
        void drawPlayer()
        {
            Cords[] cordArray = rotator();
            for (int i = 0; i < 4; i++)
            {
                blockArray[curPlayer.getXCord() + cordArray[i].getX(), curPlayer.getYCord() + cordArray[i].getY()].changeState(2);
            }
        }

        //creates new player block
        public void newPlayer()
        {
            curPlayer = new PlayerBlock();
            Block.changePlayerColour(curPlayer.getColour());
            if (blockArray[curPlayer.getXCord(), curPlayer.getYCord()].getState() == 1)
            {
                devastation();
            }
        }
        public bool rotatePlayer(char direction = 'l')
        {
            int tempRotate = curPlayer.getRotate();
            if (direction == 'l')
            {
                tempRotate++;
                if (tempRotate > 3)
                {
                    tempRotate = 0;
                }
            }
            else
            {
                tempRotate--;
                if (tempRotate < 0)
                {
                    tempRotate = 3;
                }
            }

            Cords[] cordArray = rotator(tempRotate);
            for (int i = 0; i < 4; i++)
            {
                if (cordArray[i].getX() + curPlayer.getXCord() >= gridMaxX || cordArray[i].getX() + curPlayer.getXCord() < 0 || cordArray[i].getY() + curPlayer.getYCord() > gridMaxY || cordArray[i].getY() + curPlayer.getYCord() < 0) { return false; }
                if (blockArray[cordArray[i].getX() + curPlayer.getXCord(), cordArray[i].getY() + curPlayer.getYCord()].getState() == 1 || blockArray[cordArray[i].getX() + curPlayer.getXCord(), cordArray[i].getY() + curPlayer.getYCord()].getState() == 3 || blockArray[cordArray[i].getX() + curPlayer.getXCord(), cordArray[i].getY() + curPlayer.getYCord()].getState() == 4)
                {
                    return false;
                }
            }
            cordArray = rotator();
            for (int i = 0; i < 4; i++)
            {
                blockArray[cordArray[i].getX() + curPlayer.getXCord(), cordArray[i].getY() + curPlayer.getYCord()].changeState(0);
            }

            curPlayer.setRotate(tempRotate);
            drawPlayer();
            return true;
        }
        public int getState(int x, int y)
        {
            return (blockArray[x, y].getState());
        }
        public void changeState(int x, int y, int glass, int ghostNum = 0)
        {
            blockArray[x, y].changeState(glass, ghostNum);
        }
        public bool getMarked(int x, int y)
        {
            return blockArray[x, y].getMarked();
        }
        public void setMarked(int x, int y, bool teller = true)
        {
            blockArray[x, y].setMarked(teller);
        }
        public void resetMarked()
        {
            for (int i = 0; i < gridMaxX; i++)
            {
                for (int counter = 0; counter < gridMaxY; counter++)
                {
                    blockArray[i, counter].setMarked(false);
                }
            }
        }
        public bool findGhost(int x, int y, char direction)
        {
            for (int i = 0; i < ghostNum; i++)
            {
                if (ghostArray[i].getX() == x && ghostArray[i].getY() == y)
                {
                    return (ghostArray[i].forceMove(direction));
                }
            }
            return true;
        }
        public void movePlayerBlock(char direction)
        {
            Cords[] cordArray = new Cords[4];
            for (int i = 0; i < 4; i++)
            {
                cordArray[i] = new Cords(0, 0);
            }
            do
            {
                cordArray = rotator();
            } while (cordArray == null);

            //Moving if possible
            bool possible = true;
            switch (direction)
            {
                //Down
                case ('D'):
                    for (int i = 0; i < 4; i++)
                    {
                        if (curPlayer.getYCord() + cordArray[i].getY() + 1 >= gridMaxY)
                        {
                            possible = false;
                            break;
                        }
                        if (possible && blockArray[curPlayer.getXCord() + cordArray[i].getX(), curPlayer.getYCord() + cordArray[i].getY() + 1].getState() == 1)
                        {
                            possible = false;
                            break;
                        }
                    }
                    if (possible == true)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (blockArray[curPlayer.getXCord() + cordArray[i].getX(), curPlayer.getYCord() + cordArray[i].getY() + 1].getState() == 3)
                            {
                                if (!findGhost(curPlayer.getXCord() + cordArray[i].getX(), curPlayer.getYCord() + cordArray[i].getY() + 1, 'd'))
                                {
                                    possible = false;
                                    break;
                                }
                            }
                        }
                        if (possible)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                blockArray[curPlayer.getXCord() + cordArray[i].getX(), curPlayer.getYCord() + cordArray[i].getY()].changeState(0);
                            }
                            curPlayer.setYCord(curPlayer.getYCord() + 1);
                        }
                    }
                    break;
                //Left
                case ('L'):
                    for (int i = 0; i < 4; i++)
                    {
                        if (curPlayer.getXCord() + cordArray[i].getX() - 1 < 0)
                        {
                            possible = false;
                            return;
                        }
                        if (possible && blockArray[curPlayer.getXCord() + cordArray[i].getX() - 1, curPlayer.getYCord() + cordArray[i].getY()].getState() == 1)
                        {
                            possible = false;
                            return;
                        }
                    }
                    if (possible == true)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (blockArray[curPlayer.getXCord() + cordArray[i].getX() - 1, curPlayer.getYCord() + cordArray[i].getY()].getState() == 3)
                            {
                                if (!findGhost(curPlayer.getXCord() + cordArray[i].getX() - 1, curPlayer.getYCord() + cordArray[i].getY(), 'l'))
                                {
                                    possible = false;
                                    break;
                                }
                            }
                        }
                        if (possible)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                blockArray[curPlayer.getXCord() + cordArray[i].getX(), curPlayer.getYCord() + cordArray[i].getY()].changeState(0);
                            }
                            curPlayer.setXCord(curPlayer.getXCord() - 1);
                        }
                    }
                    break;
                //Right
                case ('R'):
                    for (int i = 0; i < 4; i++)
                    {
                        if (curPlayer.getXCord() + cordArray[i].getX() + 1 >= gridMaxX)
                        {
                            possible = false;
                            return;
                        }
                        if (possible && blockArray[curPlayer.getXCord() + cordArray[i].getX() + 1, curPlayer.getYCord() + cordArray[i].getY()].getState() == 1)
                        {
                            possible = false;
                            return;
                        }
                    }
                    if (possible == true)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (blockArray[curPlayer.getXCord() + cordArray[i].getX() + 1, curPlayer.getYCord() + cordArray[i].getY()].getState() == 3)
                            {
                                if (!findGhost(curPlayer.getXCord() + cordArray[i].getX() + 1, curPlayer.getYCord() + cordArray[i].getY(), 'r'))
                                {
                                    possible = false;
                                    break;
                                }
                            }
                        }
                        if (possible)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                blockArray[curPlayer.getXCord() + cordArray[i].getX(), curPlayer.getYCord() + cordArray[i].getY()].changeState(0);
                            }
                            curPlayer.setXCord(curPlayer.getXCord() + 1);
                        }
                    }
                    break;
            }
            //If possible set the position of the new player blocks
            if (possible)
            {
                drawPlayer();
            }
            //If not possible set the position of the player blocks to walls
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    blockArray[curPlayer.getXCord() + cordArray[i].getX(), curPlayer.getYCord() + cordArray[i].getY()].changeState(1);
                }
                Random rand = new Random();
                placeSounds[rand.Next(0, 7)].Play();
                checkRemoveLine();
                if (checkIfGameComplete()) { celebration(); }
                //HEEEEEEEEEEEEEEEEEEEEEEEEEEERRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRREEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
                //Big fireworks if finished
                newPlayer();
            }
        }
        public async void devastation()
        {
            bulbasaur.BackgroundImage = new Bitmap("../../../images/loseScreen.png");
            bulbasaur.Visible = true;
            System.Media.SoundPlayer mwahaha = new System.Media.SoundPlayer("../../../sounds/loseSound.wav");
            mwahaha.Play();
            await Task.Delay(3000);
            myForm.Close();
            Application.Exit();
        }
        public async void celebration()
        {
            bulbasaur.BackgroundImage = new Bitmap("../../../images/winScreen.png");
            bulbasaur.Visible = true;
            System.Media.SoundPlayer yay = new System.Media.SoundPlayer("../../../sounds/winSound.wav");
            yay.Play();
            await Task.Delay(3000);
            myForm.Close();
            Application.Exit();
        }
        //Checks if all the ghosts have been captured
        public void moveGhost()
        {
            for (int i = 0; i < ghostNum; i++)
            {
                ghostArray[i].move();

            }
        }
        public bool checkIfGameComplete()
        {
            for (int i = 0; i < ghostNum; i++)
            {
                if (!ghostArray[i].checkIfTrapped())
                {
                    return false;
                }
            }
            return true;
        }
        public void checkRemoveLine()
        {
            //check every line in the grid
            for (int i = gridMaxY - 1; i >= 0; i--)
            {
                int counter = 0;

                for (int j = 0; j < gridMaxX; j++)
                {
                    if (blockArray[j, i].getState() == 1)
                    {
                        counter++;
                    }
                }

                //if the entire row is filled 
                if (counter == gridMaxX)
                {
                    //clear line
                    for (int j = 0; j < gridMaxX; j++)
                    {
                        blockArray[j, i].changeState(0);
                    }

                    //move everything down one y

                    for (int k = i; k >= 0; k--)
                    {

                        for (int j = 0; j <= gridMaxX - 1; j++)
                        {

                            if (k == 0)
                            {
                                blockArray[j, k].changeState(0);
                            }
                            else
                            {
                                blockArray[j, k].changeState(blockArray[j, k - 1].getState());
                                blockArray[j, k].setColour(blockArray[j, k - 1].getColour());
                            }
                        }

                    }

                }
            }

            for (int i = 0; i < ghostNum; i++)
            {
                ghostArray[i].drawMe();
            }
        }
        public int getMaxX()
        {
            return gridMaxX;
        }
        public int getMaxY()
        {
            return gridMaxY;
        }
    }

    public class Cords
    {
        int xcord;
        int ycord;

        public Cords(int x, int y)
        {
            xcord = x;
            ycord = y;
        }
        public int getX()
        {
            return xcord;
        }
        public int getY()
        {
            return ycord;
        }
        public void setCords(int x, int y)
        {
            xcord = x;
            ycord = y;
        }
    }

    public class Block
    {
        static int boxSize;
        static int startingX;
        static int startingY;
        static int spacing;
        static Form myForm;
        static Color curPlayerBlockColour;
        Panel myPanel;
        bool marked = false;
        int state = 0; //0 = blank, 1 = stationary solid block, 2 = moving block, 3 = ghost, 4 = trapped ghost

        public Block(int x, int y)
        {
            myPanel = new Panel();
            myPanel.BackgroundImageLayout = ImageLayout.Stretch;
            if (y < 4) { return; }
            y -= 4;
            myPanel.MaximumSize = new Size(boxSize, boxSize);
            myPanel.Size = new Size(boxSize, boxSize);
            myPanel.Location = new Point(startingX + x * (boxSize + spacing), startingY + y * (boxSize + spacing));
            changeState(0);
            myForm.Controls.Add(myPanel);
        }

        public static void blockInitialise(int maxX, int maxY, int winWidth, int winHeight, Form flag)
        {
            myForm = flag;
            winHeight -= 10;
            maxY -= 4;
            if (winWidth / maxX > winHeight / maxY)
            {
                boxSize = winHeight / maxY * 9 / 10;
                spacing = winHeight / maxY / 11;
            }
            else
            {
                boxSize = winWidth / maxX * 9 / 10;
                spacing = winWidth / maxX / 11;
            }

            int gridWidth = (spacing + boxSize) * maxX;
            int gridHeight = (spacing + boxSize) * maxY;
            startingX = winWidth / 2 - gridWidth / 2;
            startingY = (winHeight / 2 - gridHeight / 2) - 10;
        }


        public void changeState(int changingTo, int ghostType = -1)
        {
            state = changingTo;

            switch (changingTo)
            {
                case (0):
                    myPanel.BackColor = Color.FromName("White");
                    myPanel.BackgroundImage = null;
                    break;
                case (1):
                    myPanel.BackColor = curPlayerBlockColour;
                    myPanel.BackgroundImage = null;
                    break;
                case (2):
                    myPanel.BackColor = curPlayerBlockColour;
                    myPanel.BackgroundImage = null;
                    break;
                case (3):
                    if (ghostType == -1)
                    {
                        changeState(1);
                        return;
                    }
                    myPanel.BackColor = Color.FromName("Cyan");
                    //"../../../images/winScreen.png"
                    myPanel.BackgroundImage = new Bitmap("../../../images/ghost" + ghostType + ".png");
                    break;
                case (4):
                    if (ghostType == -1)
                    {
                        changeState(1);
                        return;
                    }
                    myPanel.BackColor = Color.FromName("Orange");
                    myPanel.BackgroundImage = new Bitmap("../../../images/ghost" + ghostType + ".png");
                    break;
            }
        }

        public Color getColour()
        {
            return myPanel.BackColor;
        }
        public void setColour(Color pussia)
        {
            myPanel.BackColor = pussia;
        }

        public int getState()
        {
            return state;
        }

        public static void changePlayerColour(Color Bill)
        {
            curPlayerBlockColour = Bill;
        }

        public bool getMarked()
        {
            return marked;
        }
        public void setMarked(bool potato)
        {
            marked = potato;
        }
    }

    public class PlayerBlock
    {
        static Random rand;
        static bool randInt = false;
        static int gridHeight;
        static int gridWidth;
        int shape = 0; //0 = 2x2; 1 = 1x4; 2 = L; 3 = reverse L; 4 = S; 5 = 2; 6 = T
        int xCord;
        int yCord;
        Color colour; //0 = pink; 1 = blue
        int rotation; //0 = 0 degrees, 1 = 90 degrees, 2 = 180 degrees, 3 = 270 degrees.

        public static void initialiseThis(int Sweden, int Penguin)
        {
            gridHeight = Sweden;
            gridWidth = Penguin;
        }
        public PlayerBlock()
        {
            //Initialise RNG
            if (!randInt)
            {
                rand = new Random();
                randInt = true;
            }

            yCord = 4;
            xCord = rand.Next(2, gridWidth - 3);
            rotation = rand.Next(0, 3);
            shape = rand.Next(0, 6);

            int colourInt = rand.Next(0, 6);
            switch (colourInt)
            {
                case 0:
                    colour = Color.FromName("Pink");
                    break;
                case 1:
                    colour = Color.FromName("Blue");
                    break;
                case 2:
                    colour = Color.FromName("Green");
                    break;
                case 3:
                    colour = Color.FromName("Yellow");
                    break;
                case 4:
                    colour = Color.FromName("Lime");
                    break;
                case 5:
                    colour = Color.FromName("Magenta");
                    break;
                case 6:
                    colour = Color.FromName("Cyan");
                    break;
            }
        }
        public int getRotate()
        {
            return rotation;
        }
        public void setRotate(int logs)
        {
            rotation = logs;
        }
        public int getXCord()
        {
            return xCord;
        }
        public int getYCord()
        {
            return yCord;
        }
        public void setXCord(int newXCord)
        {
            xCord = newXCord;
        }

        public void setYCord(int newYCord)
        {
            yCord = newYCord;
        }
        public int getShape()
        {
            return shape;
        }
        public Color getColour()
        {
            return colour;
        }
    }

    public class Ghost
    {
        static Random rand;
        static bool randInt = false;
        static Grid myGrid;
        static int ghostTrapBlockNum = 8;
        int freeSpotCounter = 0;
        int xCord;
        int yCord;
        int ghostNum;
        bool trapped = false;

        public static void setGrid(Grid potato)
        {
            myGrid = potato;
        }
        public Ghost(int x, int y, int index)
        {
            ghostNum = index;
            //Initialise RNG
            if (!randInt)
            {
                rand = new Random();
                randInt = true;
            }
            bool loop = true;
            while (loop)
            {
                xCord = rand.Next(0, x - 1);
                yCord = rand.Next(10, y);
                if (myGrid.getState(xCord, yCord) == 0)
                {
                    loop = false;
                    myGrid.changeState(xCord, yCord, 3, ghostNum);
                }
            }
        }
        public int getX()
        {
            return xCord;
        }
        public int getY()
        {
            return yCord;
        }
        public void drawMe()
        {
            if (!trapped) { myGrid.changeState(xCord, yCord, 3, ghostNum); }
            if (trapped) { myGrid.changeState(xCord, yCord, 4, ghostNum); }
        }
        public void move()
        {
            if (trapped == true) { return; }

            if (rand.Next(0, 3) == 1)
            {
                switch (rand.Next(0, 4))
                {
                    case (0):
                        if (yCord >= myGrid.getMaxY() - 1) { return; }
                        if (myGrid.getState(xCord, yCord + 1) == 0)
                        {
                            myGrid.changeState(xCord, yCord, 0);
                            yCord++;
                            myGrid.changeState(xCord, yCord, 3, ghostNum);
                        }
                        break;
                    case (1):
                        if (yCord <= 7) { return; }
                        if (myGrid.getState(xCord, yCord - 1) == 0)
                        {
                            myGrid.changeState(xCord, yCord, 0);
                            yCord--;
                            myGrid.changeState(xCord, yCord, 3, ghostNum);
                        }
                        break;
                    case (2):
                        if (xCord >= myGrid.getMaxX() - 1) { return; }
                        if (myGrid.getState(xCord + 1, yCord) == 0)
                        {
                            myGrid.changeState(xCord, yCord, 0);
                            xCord++;
                            myGrid.changeState(xCord, yCord, 3, ghostNum);
                        }
                        break;
                    case (3):
                        if (xCord <= 0) { return; }
                        if (myGrid.getState(xCord - 1, yCord) == 0)
                        {
                            myGrid.changeState(xCord, yCord, 0);
                            xCord--;
                            myGrid.changeState(xCord, yCord, 3, ghostNum);
                        }
                        break;
                }
            }
        }
        public bool forceMove(char cat)
        {
            switch (cat)
            {
                case ('d'):
                    if (yCord >= myGrid.getMaxY() - 1) { return false; }
                    if (myGrid.getState(xCord, yCord + 1) != 0) { return false; }
                    myGrid.changeState(xCord, yCord, 0);
                    yCord++;
                    myGrid.changeState(xCord, yCord, 3, ghostNum);
                    break;
                case ('l'):
                    if (xCord <= 0) { return false; }
                    if (myGrid.getState(xCord - 1, yCord) != 0) { return false; }
                    myGrid.changeState(xCord, yCord, 0);
                    xCord--;
                    myGrid.changeState(xCord, yCord, 3, ghostNum);
                    break;
                case ('r'):
                    if (xCord >= myGrid.getMaxX() - 1) { return false; }
                    if (myGrid.getState(xCord + 1, yCord) != 0) { return false; }
                    myGrid.changeState(xCord, yCord, 0);
                    xCord++;
                    myGrid.changeState(xCord, yCord, 3, ghostNum);
                    break;
            }
            return true;
        }
        public bool checkIfTrapped()
        {
            freeSpotCounter = 0;
            tileCounter(xCord, yCord);

            if (freeSpotCounter <= ghostTrapBlockNum)
            {
                myGrid.changeState(xCord, yCord, 4, ghostNum);
                trapped = true;
            }
            else
            {
                myGrid.changeState(xCord, yCord, 3, ghostNum);
                trapped = false;
            }
            myGrid.resetMarked();

            return trapped;
        }
        private int tileCounter(int x, int y)
        {
            if (x > myGrid.getMaxX() - 1 || x < 0 || y > myGrid.getMaxY() - 1 || y < 0) { return (0); }
            if (freeSpotCounter > ghostTrapBlockNum || myGrid.getMarked(x, y) || myGrid.getState(x, y) == 1 || myGrid.getState(x, y) == 2)
            {
                return 0;
            }
            if (myGrid.getState(x, y) == 0 || myGrid.getState(x, y) == 3 || myGrid.getState(x, y) == 4)
            {
                myGrid.setMarked(x, y);
                freeSpotCounter++;
                return (tileCounter(x + 1, y) + tileCounter(x, y + 1) + tileCounter(x - 1, y) + tileCounter(x, y - 1));
            }
            return 0;
        }
    }
}
