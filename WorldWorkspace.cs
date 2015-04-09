using System;
using System.Drawing;
using System.Windows.Forms;

namespace WorldEditor
{
    public partial class WorldWorkspace : Control
    {
#pragma warning disable 0414
        // Constants
        private int levelMargin = 24;

        // Variables
        private int pWidth, pHeight, pArrSize, pWSWidth, pWSHeight;
        private ushort[] pLevelGrid = null;
        private Tile[] pLevelPalette = new Tile[512];
        private string openedFile = null;
        private bool pShowGrid = false;
        private ushort curSelectedIndex = 0;

        // Drawing Lines, Rectangles variable
        private int pDBeginX, pDBeginY, pDEndX, pDEndY;
        private bool pDPreview; // Action not commited/ended yet

        // For Selection Rectangle Tool
        private int pSelectBeginX, pSelectBeginY, pSelectWidth, pSelectHeight;
//        private bool pDSelect

        // Optimized Drawing
        private int pCurTileWidth, pCurTileHeight;

        public WorldWorkspace()
        {
            InitializeComponent();

            this.Controls.Add(hScrollBar1);
            this.Controls.Add(vScrollBar1);
            this.Controls.Add(panel1);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            hScrollBar1.Enabled = false;
            vScrollBar1.Enabled = false;
        }

        // Get stuff
        public int LGridWidth
        {
            get { return pWidth; }
        }

        public int LGridWSWidth
        {
            get { return pWSWidth; }
        }

        public int LGridHeight
        {
            get { return pHeight; }
        }

        public int LGridWSHeight
        {
            get { return pWSHeight; }
        }

        public int LGridXOffset
        {
            get
            {
                if (hScrollBar1.Enabled)
                    return hScrollBar1.Value;
                else
                    return 0;
            }
        }

        public int LGridYOffset
        {
            get
            {
                if (vScrollBar1.Enabled)
                    return vScrollBar1.Value;
                else
                    return 0;
            }
        }

        public bool ShowGridLines
        {
            get { return pShowGrid; }
            set { pShowGrid = value; this.Invalidate(); }
        }

        // Functions
        public void CreateNewLevel(int width, int height)
        {
            pWidth = width;
            pHeight = height;
            pArrSize = pWidth * pHeight;
            pWSWidth = (pWidth * 32) + (levelMargin * 2);
            pWSHeight = (pHeight * 32) + (levelMargin * 2);
            pLevelGrid = new ushort[pArrSize];

            int tempSBX = pWSWidth - (this.Bounds.Width - vScrollBar1.Width);
            int tempSBY = pWSHeight - (this.Bounds.Height - hScrollBar1.Height);
            if (tempSBX > 0)
            {
                hScrollBar1.Minimum = 0;
                hScrollBar1.Maximum = tempSBX;
                if (tempSBX < 128)
                    hScrollBar1.LargeChange = tempSBX;
                else
                    hScrollBar1.LargeChange = 128;
                if (tempSBX < 32)
                    hScrollBar1.SmallChange = tempSBX;
                else
                    hScrollBar1.SmallChange = 32;
                hScrollBar1.Maximum += hScrollBar1.LargeChange;
                hScrollBar1.Enabled = true;
            }
            else
            {
                hScrollBar1.Enabled = false;
            }
            if (tempSBY > 0)
            {
                vScrollBar1.Minimum = 0;
                vScrollBar1.Maximum = tempSBY;
                if (tempSBY < 128)
                    vScrollBar1.LargeChange = tempSBY;
                else
                    vScrollBar1.LargeChange = 128;
                if (tempSBY < 32)
                    vScrollBar1.SmallChange = tempSBY;
                else
                    vScrollBar1.SmallChange = 32;
                vScrollBar1.Maximum += vScrollBar1.LargeChange;
                vScrollBar1.Enabled = true;
            }
            else
            {
                vScrollBar1.Enabled = false;
            }
            hScrollBar1.Value = 0;
            vScrollBar1.Value = 0;

            pCurTileWidth = (this.Bounds.Width / 32) + 1;
            pCurTileHeight = (this.Bounds.Height / 32) + 1;

            this.Invalidate();
        }

        public bool UpdateSelectedTile(Tile updateTile)
        {
            bool found = false;
            for (ushort i = 1; i < pLevelPalette.Length; i++)
            {
                if (pLevelPalette[i] != null && pLevelPalette[i].TileName == updateTile.TileName)
                {
                    found = true;
                    curSelectedIndex = i;
                    break;
                }
            }
            if (!found)
            {
                // Find empty slot
                ushort emptyIndex = 0;
                for (ushort i = 1; i < pLevelPalette.Length; i++)
                {
                    if (pLevelPalette[i] == null)
                    {
                        emptyIndex = i;
                        break;
                    }
                }
                if (emptyIndex != 0)
                {
                    pLevelPalette[emptyIndex] = updateTile;
                    curSelectedIndex = emptyIndex;
                    return true;
                }
                else
                {
                    return false; // The palette is full
                }
            }
            else
            {
                return true;
            }
        }

        public void PaintTile(int tX, int tY)
        {
            // Pencil Tool
            if (pLevelGrid[(tY * pWidth) + tX] != curSelectedIndex)
            {
                pLevelGrid[(tY * pWidth) + tX] = curSelectedIndex;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Calling the base class OnPaint
            base.OnPaint(pe);

            pe.Graphics.FillRectangle(Brushes.DarkGray, 0, 0, this.Bounds.Width, this.Bounds.Height);
            if (pLevelGrid != null)
            {
                int tmpOffsetX = 0;
                int tmpOffsetY = 0;
                int tmpXWidth = 0;
                int tmpYHeight = 0;
                int gOffsetX = 0;
                int gOffsetY = 0;

                // This code is for if user maximizes and scrollbars are disabled
                if (hScrollBar1.Enabled)
                    tmpOffsetX = hScrollBar1.Value;
                if (vScrollBar1.Enabled)
                    tmpOffsetY = vScrollBar1.Value;
                // Draw Outline Border
                pe.Graphics.DrawRectangle(Pens.Black, levelMargin - 1 - tmpOffsetX, levelMargin - 1 - tmpOffsetY, (pWidth * 32) + 1, (pHeight * 32) + 1);

                if (tmpOffsetX > 56)
                    gOffsetX = (tmpOffsetX - 24) / 32;
                if (tmpOffsetY > 56)
                    gOffsetY = (tmpOffsetY - 24) / 32;

                tmpXWidth = gOffsetX + pCurTileWidth;
                tmpYHeight = gOffsetY + pCurTileHeight;

                if (tmpXWidth > pWidth)
                    tmpXWidth = pWidth;
                if (tmpYHeight > pHeight)
                    tmpYHeight = pHeight;

                for (int y = gOffsetY; y < tmpYHeight; y++)
                {
                    for (int x = gOffsetX; x < tmpXWidth; x++)
                    {
                        int tmpGridIndex = (y * pWidth) + x;
                        if (pLevelGrid[tmpGridIndex] == 0)
                        {
                            pe.Graphics.FillRectangle(Brushes.White, (x << 5) + levelMargin - tmpOffsetX, (y << 5) + levelMargin - tmpOffsetY, 32, 32);
                            pe.Graphics.FillRectangle(Brushes.LightGray, (x << 5) + levelMargin - tmpOffsetX, (y << 5) + levelMargin - tmpOffsetY, 16, 16);
                            pe.Graphics.FillRectangle(Brushes.LightGray, (x << 5) + levelMargin - tmpOffsetX + 16, (y << 5) + levelMargin - tmpOffsetY + 16, 16, 16);
                        }
                        else
                        {
                            pe.Graphics.DrawImageUnscaled(pLevelPalette[pLevelGrid[tmpGridIndex]].TileImage, (x << 5) + levelMargin - tmpOffsetX, (y << 5) + levelMargin - tmpOffsetY);
                        }

                        // Draw Grid?
                        if (pShowGrid)
                        {
                            pe.Graphics.DrawRectangle(Pens.DarkGray, (x << 5) + levelMargin - tmpOffsetX, (y << 5) + levelMargin - tmpOffsetY, 31, 31);
                        }
                    }
                }

                //if (curSelectedIndex != 0)
                //{
                //    Font tmpFont = new Font(FontFamily.GenericSerif, 8, FontStyle.Bold);
                //    pe.Graphics.DrawString("Current Tile: " + pLevelPalette[curSelectedIndex].TileName, tmpFont, Brushes.Black, 2.0f, 2.0f);
                //    pe.Graphics.DrawString("Current Palette Index: " + curSelectedIndex, tmpFont, Brushes.Black, 2.0f, 20.0f);
                //    tmpFont.Dispose();
                //}
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Rectangle tempRect = new Rectangle(this.Bounds.Width - vScrollBar1.Width, 0, vScrollBar1.Width, this.Bounds.Height - hScrollBar1.Height);
            vScrollBar1.Bounds = tempRect;
            tempRect = new Rectangle(0, this.Bounds.Height - hScrollBar1.Height, this.Bounds.Width - vScrollBar1.Width, hScrollBar1.Height);
            hScrollBar1.Bounds = tempRect;
            tempRect = new Rectangle(vScrollBar1.Bounds.X, hScrollBar1.Bounds.Y, vScrollBar1.Width, hScrollBar1.Height);
            panel1.Bounds = tempRect;

            if (pLevelGrid != null)
            {
                int tempSBX = pWSWidth - (this.Bounds.Width - vScrollBar1.Width);
                int tempSBY = pWSHeight - (this.Bounds.Height - hScrollBar1.Height);
                if (tempSBX > 0)
                {
                    hScrollBar1.Minimum = 0;
                    hScrollBar1.Maximum = tempSBX;
                    if (tempSBX < 128)
                        hScrollBar1.LargeChange = tempSBX;
                    else
                        hScrollBar1.LargeChange = 128;
                    if (tempSBX < 32)
                        hScrollBar1.SmallChange = tempSBX;
                    else
                        hScrollBar1.SmallChange = 32;
                    hScrollBar1.Maximum += hScrollBar1.LargeChange;
                    hScrollBar1.Enabled = true;
                }
                else
                {
                    hScrollBar1.Enabled = false;
                }
                if (tempSBY > 0)
                {
                    vScrollBar1.Minimum = 0;
                    vScrollBar1.Maximum = tempSBY;
                    if (tempSBY < 128)
                        vScrollBar1.LargeChange = tempSBY;
                    else
                        vScrollBar1.LargeChange = 128;
                    if (tempSBY < 32)
                        vScrollBar1.SmallChange = tempSBY;
                    else
                        vScrollBar1.SmallChange = 32;
                    vScrollBar1.Maximum += vScrollBar1.LargeChange;
                    vScrollBar1.Enabled = true;
                }
                else
                {
                    vScrollBar1.Enabled = false;
                }
            }

            pCurTileWidth = (this.Bounds.Width / 32) + 1;
            pCurTileHeight = (this.Bounds.Height / 32) + 1;

            this.Invalidate();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            this.Invalidate();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            this.Invalidate();
        }
    }
}