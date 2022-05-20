// AUTHORS: Scott Crowley (u1178178), David Gillespie (u0720569), & CS3500 Faculty
// VERSION: 25 November 2019

using System;
using System.Drawing;
using System.Windows.Forms;

namespace TankWars
{
    /// <summary>
    /// Represents the game view in MVC design. Handles the graphics and UI of the TankWars client.
    /// </summary>
    public partial class ClientViewer : Form
    {
        private ClientController _controller;   // Instance of the ClientController.
        private World _world;                   // Instance of the game world/model.
        private DrawingPanel _drawingPanel;     // The panel where the world is drawn.

        /// <summary>
        /// Sole constructor for ClientViewer. Called by Main.
        /// </summary>
        /// <param name="controller">The TankWars controller.</param>
        public ClientViewer(ClientController controller)
        {
            // Initialize the Form.
            InitializeComponent();
            _controller = controller;
            _world = _controller.GetWorld();
            _controller.RegisterServerUpdateHandler(OnFrame);

            // Setup the DrawingPanel.
            ClientSize = new Size(Constants.VIEW_SIZE, Constants.VIEW_SIZE+ toolStrip1.Size.Height);
            _drawingPanel = new DrawingPanel(_world, _controller);
            _drawingPanel.Location = new Point(0, toolStrip1.Size.Height);
            _drawingPanel.Size = new Size(ClientSize.Width, ClientSize.Height);
            Controls.Add(_drawingPanel);

            // DrawingPanel event listeners:
            _drawingPanel.KeyDown   += new KeyEventHandler(DrawingPanel_KeyDown);
            _drawingPanel.KeyUp     += new KeyEventHandler(DrawingPanel_KeyUp);
            _drawingPanel.MouseDown += new MouseEventHandler(DrawingPanel_MouseDown);
            _drawingPanel.MouseUp   += new MouseEventHandler(DrawingPanel_MouseUp);
            _drawingPanel.MouseMove += new MouseEventHandler(DrawingPanel_MouseMovement);
        }


        /// <summary>
        /// Handles UpdateArrived events as notified by controller.
        /// </summary>
        private void OnFrame()
        {
            // Don't try to redraw if the window doesn't exist yet.
            // This might happen if the controller sends an update
            // before the Form has started.
            if (!IsHandleCreated)
                return;

            try
            {
                // Invalidate this form and all its children.
                // This will cause the form to redraw as soon as it can
                MethodInvoker method = new MethodInvoker(() => { Invalidate(true); });
                Invoke(method);
            }
            catch (ObjectDisposedException)
            {
                // This exception can be safely ignored. It happens when the form is closed
                // just before the OnPaint method.
            }
        }


        /// <summary>
        /// Handles clicking the Connect button.
        /// </summary>
        private void ConnectToServerButton_Click(object sender, EventArgs e)
        {   // Disable form buttons and fields to prevent accidental changing.
            connectToServerButton.Enabled = false;
            userNameTextBox.Enabled = false;
            serverAddressTextBox.Enabled = false;
            // Force focus to the DrawingPanel.
            _drawingPanel.Focus();
            // Initiate connection.
            _controller.ConnectButton(serverAddressTextBox.Text, userNameTextBox.Text);
        }

        
        /// <summary>
        /// Handles button down events on keyboard.
        /// </summary>
        private void DrawingPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                _controller.FireKeyDepress();
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                Close();
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.A:
                case Keys.S:
                case Keys.D:
                case Keys.Up:
                case Keys.Left:
                case Keys.Down:
                case Keys.Right:
                    _controller.MoveKeyDepress(e.KeyCode);
                    break;
            }
        }


        /// <summary>
        /// Handles button release events on keyboard.
        /// </summary>
        private void DrawingPanel_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                _controller.FireKeyRelease();
            }

            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.A:
                case Keys.S:
                case Keys.D:
                case Keys.Up:
                case Keys.Left:
                case Keys.Down:
                case Keys.Right:
                    _controller.MoveKeyRelease(e.KeyCode);
                    break;
            }
        }


        /// <summary>
        /// Handles mouse button down events.
        /// </summary>
        private void DrawingPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _controller.LeftMouseClick();
            }
            else if (e.Button == MouseButtons.Right)
            {
                _controller.RightMouseClick();
            }
        }


        /// <summary>
        /// Handles mouse button release events.
        /// </summary>
        private void DrawingPanel_MouseUp(object sender, MouseEventArgs e)
        {
                _controller.FireKeyRelease();
        }


        /// <summary>
        /// Handles mouse movement events.
        /// </summary>
        private void DrawingPanel_MouseMovement(object sender, MouseEventArgs e)
        {
            int x = e.X - (Constants.VIEW_SIZE) / 2;
            int y = e.Y - (Constants.VIEW_SIZE) / 2;
            _controller.TurretDirection(x, y);
        }


        /// <summary>
        /// Game information found in the [Help>About] menu.
        /// </summary>
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Tank Wars:\n"+
                                                 "Artwork by Jolie Uk and Alex Smith\n"+
                                                 "Game Design by Daniel Kopta\n"+
                                                 "Implementation by Scott Crowley and David Gillespie\n"+
                                                 "CS3500 Fall 2019 University of Utah");
        }

        /// <summary>
        /// Game controls layout found in the [Help>Controls] menu.
        /// </summary>
        private void ControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("W/↑:\t\t\tMove up\n" +
                                                 "A/←:\t\t\tMove left\n" +
                                                 "S/↓:\t\t\tMove down\n" +
                                                 "D/→:\t\t\tMove right\n" +
                                                 "Mouse:\t\t\tAim\n" +
                                                 "Left Click/Spacebar :\tFire Projectile\n" +
                                                 "Right Click:\t\tFire Special\n"+
                                                 "Esc:\t\t\tQuit\n" 
                                                 );
        }
    }
}
