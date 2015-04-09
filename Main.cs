using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WorldEditor
{
    public partial class Main : Form
    {
        private enum LGridTools { SelectTool, RectSelectTool, PencilTool, LineTool, DrawRectTool, FillRectTool, FillTool, TilePickerTool, EraseTool };
        private LGridTools curTool = LGridTools.SelectTool;
        private bool wsMouseDown = false;

        public Main()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            tileSelector1.LoadTilesets();
            worldWorkspace1.CreateNewLevel(75, 75);
            tileSelector1.SelectTile(1, 1);
        }

        private void tileSelector1_MouseDown(object sender, MouseEventArgs e)
        {
            // Check X Bounds
            if (e.X > (tileSelector1.Width - tileSelector1.Controls["vScrollBar1"].Width))
            {
                return;
            }
            tileSelector1.SelectTile(e.X, e.Y);
        }

        private void worldWorkspace1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                wsMouseDown = true;
                // Translate screen coords into grid coords
                int tmpCanvasX = e.X + worldWorkspace1.LGridXOffset - 24;
                int tmpCanvasY = e.Y + worldWorkspace1.LGridYOffset - 24;
                if (tmpCanvasX < 0 || (tmpCanvasX / 32) >= worldWorkspace1.LGridWidth || tmpCanvasY < 0 || (tmpCanvasY / 32) >= worldWorkspace1.LGridHeight)
                {
                    // Release mouse down and don't do anything else, we hit the margins...
                    wsMouseDown = false;
                    return;
                }
                int tmpGridX = tmpCanvasX / 32;
                int tmpGridY = tmpCanvasY / 32;

                switch (curTool)
                {
                    case LGridTools.SelectTool:
                        wsMouseDown = false; // Release immediately, no need to drag
                        break;
                    case LGridTools.RectSelectTool:
                        break;
                    case LGridTools.PencilTool:
                        if (tileSelector1.CurTileChanged)
                        {
                            if (!worldWorkspace1.UpdateSelectedTile(tileSelector1.GetCurTile))
                            {
                                MessageBox.Show("The level palette is full.");
                                wsMouseDown = false;
                                break;
                            }
                            tileSelector1.CurTileChanged = false;
                        }
                        worldWorkspace1.PaintTile(tmpGridX, tmpGridY);
                        break;
                    case LGridTools.LineTool:
                        if (tileSelector1.CurTileChanged)
                        {
                            if (!worldWorkspace1.UpdateSelectedTile(tileSelector1.GetCurTile))
                            {
                                MessageBox.Show("The level palette is full.");
                                wsMouseDown = false;
                                break;
                            }
                            tileSelector1.CurTileChanged = false;
                        }
                        break;
                    case LGridTools.DrawRectTool:
                        if (tileSelector1.CurTileChanged)
                        {
                            if (!worldWorkspace1.UpdateSelectedTile(tileSelector1.GetCurTile))
                            {
                                MessageBox.Show("The level palette is full.");
                                wsMouseDown = false;
                                break;
                            }
                            tileSelector1.CurTileChanged = false;
                        }
                        break;
                    case LGridTools.FillRectTool:
                        if (tileSelector1.CurTileChanged)
                        {
                            if (!worldWorkspace1.UpdateSelectedTile(tileSelector1.GetCurTile))
                            {
                                MessageBox.Show("The level palette is full.");
                                wsMouseDown = false;
                                break;
                            }
                            tileSelector1.CurTileChanged = false;
                        }
                        break;
                    case LGridTools.FillTool:
                        if (tileSelector1.CurTileChanged)
                        {
                            if (!worldWorkspace1.UpdateSelectedTile(tileSelector1.GetCurTile))
                            {
                                MessageBox.Show("The level palette is full.");
                                wsMouseDown = false;
                                break;
                            }
                            tileSelector1.CurTileChanged = false;
                        }
                        wsMouseDown = false; // Release immediately
                        break;
                    case LGridTools.TilePickerTool:
                        wsMouseDown = false; // Release immediately
                        break;
                    case LGridTools.EraseTool:
                        break;
                }
            }
        }

        private void worldWorkspace1_MouseMove(object sender, MouseEventArgs e)
        {
            // Translate screen coords into grid coords
            int tmpCanvasX = e.X + worldWorkspace1.LGridXOffset - 24;
            int tmpCanvasY = e.Y + worldWorkspace1.LGridYOffset - 24;
            int tmpGridX = tmpCanvasX / 32;
            int tmpGridY = tmpCanvasY / 32;

            // Update X and Y Status Labels only if we're in Canvas Bounds
            if (tmpCanvasX > 0 && tmpGridX < worldWorkspace1.LGridWidth && tmpCanvasY > 0 && tmpGridY < worldWorkspace1.LGridHeight)
            {
                xStatusLabel.Text = "X: " + tmpGridX;
                yStatusLabel.Text = "Y: " + tmpGridY;
            }
            else
            {
                xStatusLabel.Text = "";
                yStatusLabel.Text = "";
            }

            // Bounds Checking
            if (tmpGridX > worldWorkspace1.LGridWidth)
                tmpGridX = worldWorkspace1.LGridWidth - 1;
            if (tmpGridY > worldWorkspace1.LGridHeight)
                tmpGridY = worldWorkspace1.LGridHeight - 1;

            if (wsMouseDown)
            {
                switch (curTool)
                {
                    case LGridTools.SelectTool:
                        // Do nothing
                        break;
                    case LGridTools.RectSelectTool:
                        break;
                    case LGridTools.PencilTool:
                        worldWorkspace1.PaintTile(tmpGridX, tmpGridY);
                        break;
                    case LGridTools.LineTool:
                        break;
                    case LGridTools.DrawRectTool:
                        break;
                    case LGridTools.FillRectTool:
                        break;
                    case LGridTools.FillTool:
                        // Do nothing
                        break;
                    case LGridTools.TilePickerTool:
                        // Do nothing
                        break;
                    case LGridTools.EraseTool:
                        break;
                }
            }
        }

        private void worldWorkspace1_MouseUp(object sender, MouseEventArgs e)
        {
            wsMouseDown = false;
            switch (curTool)
            {
                case LGridTools.SelectTool:
                    // Do nothing
                    break;
                case LGridTools.RectSelectTool:
                    break;
                case LGridTools.PencilTool:
                    // Don't have to do anything here
                    break;
                case LGridTools.LineTool:
                    break;
                case LGridTools.DrawRectTool:
                    break;
                case LGridTools.FillRectTool:
                    break;
                case LGridTools.FillTool:
                    // Do nothing
                    break;
                case LGridTools.TilePickerTool:
                    // Do nothing
                    break;
                case LGridTools.EraseTool:
                    break;
            }
        }

        private void worldWorkspace1_MouseLeave(object sender, EventArgs e)
        {
            xStatusLabel.Text = "";
            yStatusLabel.Text = "";
        }

        private void uncheckAllToolButtons()
        {
            selectToolButton.Checked = false;
            rectSelectToolButton.Checked = false;
            pencilToolButton.Checked = false;
            lineToolButton.Checked = false;
            drawRectToolButton.Checked = false;
            fillRectToolButton.Checked = false;
            fillToolButton.Checked = false;
            tilePickerToolButton.Checked = false;
            eraseToolButton.Checked = false;
        }

        private void selectToolButton_Click(object sender, EventArgs e)
        {
            uncheckAllToolButtons();
            selectToolButton.Checked = true;
            curTool = LGridTools.SelectTool;
        }

        private void rectSelectToolButton_Click(object sender, EventArgs e)
        {
            uncheckAllToolButtons();
            rectSelectToolButton.Checked = true;
            curTool = LGridTools.RectSelectTool;
        }

        private void pencilToolButton_Click(object sender, EventArgs e)
        {
            uncheckAllToolButtons();
            pencilToolButton.Checked = true;
            curTool = LGridTools.PencilTool;
        }

        private void lineToolButton_Click(object sender, EventArgs e)
        {
            uncheckAllToolButtons();
            lineToolButton.Checked = true;
            curTool = LGridTools.LineTool;
        }

        private void drawRectToolButton_Click(object sender, EventArgs e)
        {
            uncheckAllToolButtons();
            drawRectToolButton.Checked = true;
            curTool = LGridTools.DrawRectTool;
        }

        private void fillRectToolButton_Click(object sender, EventArgs e)
        {
            uncheckAllToolButtons();
            fillRectToolButton.Checked = true;
            curTool = LGridTools.FillRectTool;
        }

        private void fillToolButton_Click(object sender, EventArgs e)
        {
            uncheckAllToolButtons();
            fillToolButton.Checked = true;
            curTool = LGridTools.FillTool;
        }

        private void tilePickerToolButton_Click(object sender, EventArgs e)
        {
            uncheckAllToolButtons();
            tilePickerToolButton.Checked = true;
            curTool = LGridTools.TilePickerTool;
        }

        private void eraseToolButton_Click(object sender, EventArgs e)
        {
            uncheckAllToolButtons();
            eraseToolButton.Checked = true;
            curTool = LGridTools.EraseTool;
        }

        private void showGridLinesButton_Click(object sender, EventArgs e)
        {
            if (showGridLinesButton.Checked)
            {
                showGridLinesButton.Checked = false;
                worldWorkspace1.ShowGridLines = false;
            }
            else
            {
                showGridLinesButton.Checked = true;
                worldWorkspace1.ShowGridLines = true;
            }
        }
    }
}