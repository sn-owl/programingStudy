using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 완성본
{
    public partial class Form3 : Form
    {
        public Image image {  get; set; }
        public Image conimage { get; set; }


        public Form3()
        {
            InitializeComponent();
        }



        private void Form3_Load(object sender, EventArgs e)
        {
            conimage = image;
            hScrollBar1.Value=hScrollBar1.Maximum/2;
        }           
        private void Form3_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(conimage, 0, 0, panel1.Width, panel1.Height);
            //그림의 크기도 인자로 넘김. 그림크기에 맞추어 윈도우 열기 
        }     
        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

            int value = hScrollBar1.Value;
            int Rvalue = hScrollBar2.Value;
            int Gvalue = hScrollBar3.Value;
            int Bvalue = hScrollBar4.Value;


            Bitmap B = new Bitmap(image);
            for (int y = 0; y < B.Height; y++)
                for (int x = 0; x < B.Width; x++)
                {
                    Color color = B.GetPixel(x, y);
                    int r = color.R;
                    int g = color.G;
                    int b = color.B;
                    // Saturation

                        value = Math.Max(0,Math.Min(256,value));
                   
                        

                    r = Math.Max(0, Math.Min(255, r+value-128));
                    g = Math.Max(0, Math.Min(255, g+value -128));
                    b = Math.Max(0, Math.Min(255, b+value -128));

                    r = Math.Max(0, Math.Min(255, r + (Rvalue - 128)*2));
                    g = Math.Max(0, Math.Min(255, g + (Gvalue - 128)*2));
                    b = Math.Max(0, Math.Min(255, b + (Bvalue - 128)*2));

                    B.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            conimage = B;
            panel1.Invalidate();
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            hScrollBar1.Enabled = true; hScrollBar2.Enabled=true;
            hScrollBar3.Enabled=true;hScrollBar4.Enabled=true; 
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            hScrollBar1.Enabled = false; hScrollBar2.Enabled = false;
            hScrollBar3.Enabled = false; hScrollBar4.Enabled = false;

            Bitmap gBitmap = new Bitmap(image);
            //https://docs.microsoft.com/ko-kr/dotnet/api/system.drawing.imaging.pixelformat?view=netframework-4.8
            if (gBitmap.PixelFormat.ToString() != "Format8bppIndexed")
            {
                for (int i = 0; i < gBitmap.Height; i++)
                {
                    for (int j = 0; j < gBitmap.Width; j++)
                    {
                        int color = gBitmap.GetPixel(j, i).R + gBitmap.GetPixel(j, i).G + gBitmap.GetPixel(j, i).B;
                        color /= 3;
                        Color c = Color.FromArgb(color, color, color);
                        gBitmap.SetPixel(j, i, c);
                    }
                }

            }
            Bitmap Edge = new Bitmap(gBitmap);
            int[,] m = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int sum;
            for (int x = 1; x < gBitmap.Width - 1; x++)
            {
                for (int y = 1; y < gBitmap.Height - 1; y++)
                {
                    sum = 0;
                    for (int r = -1; r < 2; r++)
                    {
                        for (int c = -1; c < 2; c++)
                        {

                            sum += m[r + 1, c + 1] * gBitmap.GetPixel(x + r, y + c).R;

                        }
                    }
                    sum = Math.Abs(sum);
                    if (sum > 255) sum = 255;
                    //if (sum < 0) sum = 0;
                    Edge.SetPixel(x, y, Color.FromArgb(sum, sum, sum));
                }

            }
            conimage = Edge;
            panel1.Invalidate();
            
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            hScrollBar1.Enabled = false; hScrollBar2.Enabled = false;
            hScrollBar3.Enabled = false; hScrollBar4.Enabled = false;

            Bitmap gBitmap = new Bitmap(image);
            //https://docs.microsoft.com/ko-kr/dotnet/api/system.drawing.imaging.pixelformat?view=netframework-4.8
            if (gBitmap.PixelFormat.ToString() != "Format8bppIndexed")
            {
                for (int i = 0; i < gBitmap.Height; i++)
                {
                    for (int j = 0; j < gBitmap.Width; j++)
                    {
                        int color = gBitmap.GetPixel(j, i).R + gBitmap.GetPixel(j, i).G + gBitmap.GetPixel(j, i).B;
                        color /= 3;
                        Color c = Color.FromArgb(color, color, color);
                        gBitmap.SetPixel(j, i, c);
                    }
                }

            }
            Bitmap Edge = new Bitmap(gBitmap);
            int[,] m = { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };


            int sum;
            for (int x = 1; x < gBitmap.Width - 1; x++)
            {
                for (int y = 1; y < gBitmap.Height - 1; y++)
                {
                    sum = 0;
                    for (int r = -1; r < 2; r++)
                    {
                        for (int c = -1; c < 2; c++)
                        {

                            sum += m[r + 1, c + 1] * gBitmap.GetPixel(x + r, y + c).R;

                        }
                    }
                    sum = Math.Abs(sum);
                    if (sum > 255) sum = 255;
                    //if (sum < 0) sum = 0;
                    Edge.SetPixel(x, y, Color.FromArgb(sum, sum, sum));
                }

            }
            conimage = Edge;
            panel1.Invalidate();

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            hScrollBar1.Enabled = false; hScrollBar2.Enabled = false;
            hScrollBar3.Enabled = false; hScrollBar4.Enabled = false;

            Bitmap gBitmap = new Bitmap(image);
            //https://docs.microsoft.com/ko-kr/dotnet/api/system.drawing.imaging.pixelformat?view=netframework-4.8
            if (gBitmap.PixelFormat.ToString() != "Format8bppIndexed")
            {
                for (int i = 0; i < gBitmap.Height; i++)
                {
                    for (int j = 0; j < gBitmap.Width; j++)
                    {
                        int color = gBitmap.GetPixel(j, i).R + gBitmap.GetPixel(j, i).G + gBitmap.GetPixel(j, i).B;
                        color /= 3;
                        Color c = Color.FromArgb(color, color, color);
                        gBitmap.SetPixel(j, i, c);
                    }
                }

            }
            Bitmap Edge = new Bitmap(gBitmap);
            int[,] m = { { 1, 1, 1 }, { 1, -8, 1 }, { 1, 1, 1 } };


            int sum;
            for (int x = 1; x < gBitmap.Width - 1; x++)
            {
                for (int y = 1; y < gBitmap.Height - 1; y++)
                {
                    sum = 0;
                    for (int r = -1; r < 2; r++)
                    {
                        for (int c = -1; c < 2; c++)
                        {

                            sum += m[r + 1, c + 1] * gBitmap.GetPixel(x + r, y + c).R;

                        }
                    }
                    sum = Math.Abs(sum);
                    if (sum > 255) sum = 255;
                    //if (sum < 0) sum = 0;
                    Edge.SetPixel(x, y, Color.FromArgb(sum, sum, sum));
                }

            }
            conimage = Edge;
            panel1.Invalidate();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
