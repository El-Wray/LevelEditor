using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WorldEditor
{
    public partial class TileSelector : Control
    {
        private List<Tile[]> tileLists = new List<Tile[]>();
        private int curGroupSelected = 0;
        private int curTileSelected = 0;
        private Tile curTileObjSelected = null;
        private bool curTileChanged = true;

        public TileSelector()
        {
            InitializeComponent();

            this.Controls.Add(vScrollBar1);
            this.Controls.Add(panel1);
            this.Controls.Add(comboBox1);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public void LoadTilesets()
        {
            try
            {
                string[] dirList = Directory.GetDirectories(Environment.CurrentDirectory + "\\tiles");
                for (int i = 0; i < dirList.Length; i++)
                {
                    string groupName = dirList[i].Substring(dirList[i].LastIndexOf("\\") + 1);
                    comboBox1.Items.Add(groupName);
                    string[] fileList = Directory.GetFiles(dirList[i], "*.png");
                    Tile[] imgList = new Tile[fileList.Length];
                    for (int j = 0; j < fileList.Length; j++)
                    {
                        imgList[j] = new Tile();
                        imgList[j].TileName = groupName + "/" + fileList[j].Substring(fileList[j].LastIndexOf("\\") + 1);
                        imgList[j].TileImage = (Bitmap)Bitmap.FromFile(fileList[j]);
                    }
                    tileLists.Add(imgList);
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("No tiles found.\nThe editor is pretty useless without them.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            if (comboBox1.Items.Count == 0)
            {
                MessageBox.Show("No tiles found.\nThe editor is pretty useless without them.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            comboBox1.SelectedIndex = 0;
            vScrollBar1.Value = 0;
            AdjustVertScrollbar();
            this.Invalidate();
        }

        public Tile GetCurTile
        {
            get { return curTileObjSelected; }
        }

        public bool CurTileChanged
        {
            get { return curTileChanged; }
            set { curTileChanged = value; }
        }

        public void SelectTile(int mX, int mY)
        {
            mY = mY - comboBox1.Height - 2;
            if (vScrollBar1.Enabled)
                mY = mY + vScrollBar1.Value;

            int mTX = mX / 38;
            int mTY = mY / 38;
            int mTIndex = (mTY * 3) + mTX;

            if (mTIndex >= tileLists[curGroupSelected].Length)
                return;

            if (curTileObjSelected != null && (tileLists[curGroupSelected])[mTIndex].TileName == curTileObjSelected.TileName)
                return;

            curTileObjSelected = (tileLists[curGroupSelected])[mTIndex];
            curTileChanged = true;
            curTileSelected = mTIndex;

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Calling the base class OnPaint
            base.OnPaint(pe);

            if (DesignMode)
                return;

            int tmpOffsetY = vScrollBar1.Bounds.Y;
            int tmpSOffsetY = 0;
            int tmpCount = 0; // When it hits 3, reset to 0 and increase tmpOffsetY by 38

            if (vScrollBar1.Enabled)
                tmpSOffsetY = vScrollBar1.Value;

            for (int i = 0; i < tileLists[curGroupSelected].Length; i++)
            {
                if (i == curTileSelected)
                {
                    pe.Graphics.DrawRectangle(SystemPens.ControlDarkDark, tmpCount * 38, tmpOffsetY - tmpSOffsetY, 37, 37);
                    pe.Graphics.FillRectangle(SystemBrushes.ControlDark, (tmpCount * 38) + 1, tmpOffsetY + 1 - tmpSOffsetY, 36, 36);
                }
                pe.Graphics.DrawImage((tileLists[curGroupSelected])[i].TileImage, (tmpCount * 38) + 3, tmpOffsetY + 3 - tmpSOffsetY);
                tmpCount++;
                if (tmpCount == 3)
                {
                    tmpCount = 0;
                    tmpOffsetY += 38;
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Rectangle tempRect = new Rectangle(this.Bounds.Width - vScrollBar1.Width, comboBox1.Height + 2, vScrollBar1.Width, this.Bounds.Height - (comboBox1.Height + 2));
            vScrollBar1.Bounds = tempRect;
            tempRect = new Rectangle(0, comboBox1.Height, this.Bounds.Width, 2);
            panel1.Bounds = tempRect;

            AdjustVertScrollbar();
        }

        private void AdjustVertScrollbar()
        {
            if (tileLists.Count == 0)
                return;

            int tempSBY = ((((tileLists[curGroupSelected].Length - 1) / 3) + 1) * 38) - (this.Bounds.Height - comboBox1.Height - 2);
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

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            this.Invalidate();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            curGroupSelected = comboBox1.SelectedIndex;
            vScrollBar1.Value = 0;
            AdjustVertScrollbar();
            SelectTile(1, 1);
            this.Invalidate();
        }
    }
}
