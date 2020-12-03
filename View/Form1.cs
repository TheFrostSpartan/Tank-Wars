using DrawPan;
using GameController;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace View
{
    public partial class Form1 : Form
    {
        // Globals to be used throughout the methods in form1
        Controller mainCtrl;
        private World theWorld;
        DrawingPanel drawingPanel;
        private bool leftPressed = false;
        private bool rightPressed = false;
        private bool upPressed = false;
        private bool downPressed = false;

        /// <summary>
        /// The constructor of Form1 that also watches for Mouse Moved Events
        /// </summary>
        /// <param name="control"></param>
        public Form1(Controller control)
        {
            InitializeComponent();
            mainCtrl = control;
            theWorld = mainCtrl.getWorld();
            mainCtrl.registerUpdateHandler(OnFrame);

            ClientSize = new Size(800, 800);
            drawingPanel = new DrawingPanel(theWorld);
            drawingPanel.Location = new Point(0, 0);
            drawingPanel.Size = new Size(this.ClientSize.Width, this.ClientSize.Height);
            this.Controls.Add(drawingPanel);
            this.drawingPanel.MouseMove += new MouseEventHandler(this.drawingPanel_MouseMove);
            this.drawingPanel.MouseClick += new MouseEventHandler(this.drawingPanel_MouseClick);
        }

        /// <summary>
        /// When the ConnectButton is clicked, use the connectToServer method to connect the client to the form
        /// and disable the button and text boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (serverTextBox.TextLength != 0 && nameTextBox.TextLength != 0)
            {
                mainCtrl.connectToServer(serverTextBox.Text, nameTextBox.Text);
                this.ConnectButton.Enabled = false;
                this.serverTextBox.Enabled = false;
                this.nameTextBox.Enabled = false;
            }
            else
            {
                MessageBox.Show("Please fill out all forms");
            }
        }

        /// <summary>
        /// Invalidates the entire form and prepares it for a new draw
        /// </summary>
        private void OnFrame()
        {
            if (!IsHandleCreated)
            {
                return;
            }
            MethodInvoker invoker = new MethodInvoker(() => this.Invalidate(true));
            this.Invoke(invoker);
        }

        /// <summary>
        /// When a key is pressed, detect which key it was and then send to the controller the proper move command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            if (e.KeyCode == Keys.W)
            {
                mainCtrl.moveTank("up");
                upPressed = true;
            }
            else if (e.KeyCode == Keys.S)
            {
                mainCtrl.moveTank("down");
                downPressed = true;
            }
            else if (e.KeyCode == Keys.A)
            {
                mainCtrl.moveTank("left");
                leftPressed = true;
            }
            else if (e.KeyCode == Keys.D)
            {
                mainCtrl.moveTank("right");
                rightPressed = true;
            }
        }

        /// <summary>
        /// An additional move helper method that checks what keys are pressed
        /// anytime a key is lifted. This allows for multiple keys to be pressed
        /// at the same time.
        /// </summary>
        private void secondaryMove()
        {
            if (upPressed)
            {
                mainCtrl.moveTank("up");
            }
            else if (downPressed)
            {
                mainCtrl.moveTank("down");
            }
            else if (leftPressed)
            {
                mainCtrl.moveTank("left");
            }
            else if (rightPressed)
            {
                mainCtrl.moveTank("right");
            }
        }

        /// <summary>
        /// On key up, detect which key was lifted and then stop the movement of that key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                upPressed = false;
                secondaryMove();
            }
            else if (e.KeyCode == Keys.S)
            {
                downPressed = false;
                secondaryMove();
            }
            else if (e.KeyCode == Keys.A)
            {
                leftPressed = false;
                secondaryMove();
            }
            else if (e.KeyCode == Keys.D)
            {
                rightPressed = false;
                secondaryMove();
            }

            if(!downPressed && !upPressed && !leftPressed && !rightPressed)
            {
                mainCtrl.moveTank("none");
            }
        }

        /// <summary>
        /// On mouse click detect which button was pressed and then send the appropriate fire command
        /// to the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drawingPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                mainCtrl.fireTank("main");
            }
            else if(e.Button == MouseButtons.Right)
            {
                mainCtrl.fireTank("alt");
            }
        }

        /// <summary>
        /// On mouse move create a point that follows it and then turn the turret that direction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drawingPanel_MouseMove(object sender, MouseEventArgs e)
        {
            Point temp = PointToClient(Control.MousePosition);
            mainCtrl.turretDir(temp);
        }

        /// <summary>
        /// When the help button is clicked, show the help button dropdown menu and remove
        /// the focus from the button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpButton_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(helpButton, new Point(0, helpButton.Height));
            helpButton.Enabled = false;
            helpButton.Enabled = true;
        }

        /// <summary>
        /// When the contols button is clicked, show the controls help message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Controls:\nW: - Move Up\nS: - Move Down\nA: - Move Left\nD: - Move Right\nMouse: - Aim\nLeft Click: - Fire Normal\nRight Click: - Fire Beam");
        }

        /// <summary>
        /// When the about button is clicked, show the about message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created for University of Utah CS3500\nCompleted November 22nd 2019\nArtwork by Jolie Uk and Alex Smith\nGame design by Daniel Kopta\nImplementation by Ethan Duncan and Matt Mader");
        }
    }
}
