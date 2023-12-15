using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using openCV;

namespace Histogram
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        IplImage image1;
        IplImage editableImg;
        Bitmap bmp;

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "All|*.*|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.bmp|All|*.*|";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                image1 = cvlib.CvLoadImage(openFileDialog1.FileName, cvlib.CV_LOAD_IMAGE_COLOR);
                CvSize size = new CvSize(pictureBox1.Width, pictureBox1.Height);
                IplImage resized_image = cvlib.CvCreateImage(size, image1.depth, image1.nChannels);
                cvlib.CvResize(ref image1, ref resized_image, cvlib.CV_INTER_LINEAR);
                pictureBox1.Image = (Image)resized_image;
            }
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.Series["Red"].Points.Clear();
            chart1.Series["Green"].Points.Clear();
            chart1.Series["Blue"].Points.Clear();

            Bitmap bmpImg = (Bitmap)pictureBox1.Image;
            int width = bmpImg.Width;
            int hieght = bmpImg.Height;

            int[] numOfRed = new int[256];
            int[] numOfGreen = new int[256];
            int[] numOfBlue = new int[256];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < hieght; j++)
                {
                    Color pixelColor = bmpImg.GetPixel(i, j);

                    numOfRed[pixelColor.R]++;
                    numOfGreen[pixelColor.G]++;
                    numOfBlue[pixelColor.B]++;

                }
            }

            for (int i = 0; i < 256; i++)
            {
                chart1.Series["Red"].Points.AddY(numOfRed[i]);
                chart1.Series["Green"].Points.AddY(numOfGreen[i]);
                chart1.Series["Blue"].Points.AddY(numOfBlue[i]);
            }
        }

        private void equlaizedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart2.Series["Red"].Points.Clear();
            chart2.Series["Green"].Points.Clear();
            chart2.Series["Blue"].Points.Clear();

            Bitmap bmpImg = (Bitmap)pictureBox2.Image;
            int width = bmpImg.Width;
            int hieght = bmpImg.Height;

            int[] numOfRed = new int[256];
            int[] numOfGreen = new int[256];
            int[] numOfBlue = new int[256];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < hieght; j++)
                {
                    Color pixelColor = bmpImg.GetPixel(i, j);

                    numOfRed[pixelColor.R]++;
                    numOfGreen[pixelColor.G]++;
                    numOfBlue[pixelColor.B]++;

                }
            }

            for (int i = 0; i < 256; i++)
            {
                chart2.Series["Red"].Points.AddY(numOfRed[i]);
                chart2.Series["Green"].Points.AddY(numOfGreen[i]);
                chart2.Series["Blue"].Points.AddY(numOfBlue[i]);
            }
        }

        private void equalizedImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmpImg = (Bitmap)image1;
            Bitmap newImage = bmpImg;
            int width = bmpImg.Width;
            int hieght = bmpImg.Height;


            //Calculate N(i)

            int[] numOfRed = new int[256];
            int[] numOfGreen = new int[256];
            int[] numOfBlue = new int[256];


            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < hieght; j++)
                {
                    Color pixelColor = bmpImg.GetPixel(i, j);

                    numOfRed[pixelColor.R]++;
                    numOfGreen[pixelColor.G]++;
                    numOfBlue[pixelColor.B]++;
                }
            }

            //Calculate P(Ni) 
            decimal[] probNumOfRed = new decimal[256];
            decimal[] probNumOfGreen = new decimal[256];
            decimal[] probNumOfBlue = new decimal[256];

            for (int i = 0; i < 256; i++)
            {
                probNumOfRed[i] = (decimal)numOfRed[i] / (decimal)(width * hieght);
                probNumOfGreen[i] = (decimal)numOfGreen[i] / (decimal)(width * hieght);
                probNumOfBlue[i] = (decimal)numOfBlue[i] / (decimal)(width * hieght);
            }

            //Calculate CDF

            decimal[] cdfRed = new decimal[256];
            decimal[] cdfGreen = new decimal[256];
            decimal[] cdfBlue = new decimal[256];

            cdfRed[0] = probNumOfRed[0];
            cdfGreen[0] = probNumOfGreen[0];
            cdfBlue[0] = probNumOfBlue[0];

            for (int i = 1; i < 256; i++)
            {
                cdfRed[i] = probNumOfRed[i] + cdfRed[i - 1];
                cdfGreen[i] = probNumOfGreen[i] + cdfGreen[i - 1];
                cdfBlue[i] = probNumOfBlue[i] + cdfBlue[i - 1];
            }


            //Calculate CDF(L-1)


            int red, green, blue;
            int constant = 255;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < hieght; j++)
                {
                    Color pixelColor = bmpImg.GetPixel(i, j);

                    red = (int)Math.Round(cdfRed[pixelColor.R] * constant);
                    green = (int)Math.Round(cdfRed[pixelColor.G] * constant);
                    blue = (int)Math.Round(cdfRed[pixelColor.B] * constant);

                    Color newColor = Color.FromArgb(red, green, blue);
                    newImage.SetPixel(i, j, newColor);

                }
            }

            pictureBox2.Image = (Image)newImage;
        }

        private void convertToGreyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmpImg = (Bitmap)image1;
            int width = bmpImg.Width;
            int height = bmpImg.Height;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color pixelColor = bmpImg.GetPixel(i, j);
                    int a = pixelColor.A;
                    int r = pixelColor.R;
                    int g = pixelColor.G;
                    int b = pixelColor.B;
                    int average = (r + g + b) / 3;
                    Color greyColor = Color.FromArgb(a, average, average, average);
                    bmpImg.SetPixel(i, j, greyColor);
                    pictureBox3.BackgroundImage = (Image)bmpImg;
                }
            }
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editableImg = cvlib.CvCreateImage(new CvSize(image1.width, image1.height), image1.depth, image1.nChannels);
            int srcAddress = image1.imageData.ToInt32();
            int destAddress = editableImg.imageData.ToInt32();
            unsafe
            {
                int srcIndex, destIndex;

                for (int r = 0; r < editableImg.height; r++)
                    for (int c = 0; c < editableImg.width; c++)
                    {
                        srcIndex = destIndex = (editableImg.width * r * editableImg.nChannels) + (c * editableImg.nChannels);
                        *(byte*)(destAddress + destIndex + 0) = 0; //blueValue
                        *(byte*)(destAddress + destIndex + 1) = 0; //greenValue
                        *(byte*)(destAddress + destIndex + 2) = *(byte*)(srcAddress + srcIndex + 2); //redValue
                    }
            }
            CvSize size = new CvSize(pictureBox2.Width, pictureBox2.Height);
            IplImage resizedImage = cvlib.CvCreateImage(size, editableImg.depth, editableImg.nChannels);
            cvlib.CvResize(ref editableImg, ref resizedImage, cvlib.CV_INTER_LINEAR);
            pictureBox3.BackgroundImage = (Image)resizedImage;
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editableImg = cvlib.CvCreateImage(new CvSize(image1.width, image1.height), image1.depth, image1.nChannels);
            int srcAddress = image1.imageData.ToInt32();
            int destAddress = editableImg.imageData.ToInt32();
            unsafe
            {
                int srcIndex, destIndex;

                for (int r = 0; r < editableImg.height; r++)
                    for (int c = 0; c < editableImg.width; c++)
                    {
                        srcIndex = destIndex = (editableImg.width * r * editableImg.nChannels) + (c * editableImg.nChannels);
                        *(byte*)(destAddress + destIndex + 0) = 0; 
                        *(byte*)(destAddress + destIndex + 1) = *(byte*)(srcAddress + srcIndex + 1); 
                        *(byte*)(destAddress + destIndex + 2) = 0; 
                    }
            }
            CvSize size = new CvSize(pictureBox2.Width, pictureBox2.Height);
            IplImage resizedImage = cvlib.CvCreateImage(size, editableImg.depth, editableImg.nChannels);
            cvlib.CvResize(ref editableImg, ref resizedImage, cvlib.CV_INTER_LINEAR);
            pictureBox3.BackgroundImage = (Image)resizedImage;
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editableImg = cvlib.CvCreateImage(new CvSize(image1.width, image1.height), image1.depth, image1.nChannels);
            int srcAddress = image1.imageData.ToInt32();
            int destAddress = editableImg.imageData.ToInt32();

            unsafe
            {
                int srcIndex, destIndex;

                for (int b = 0; b < editableImg.height; b++)
                    for (int c = 0; c < editableImg.width; c++)
                    {
                        srcIndex = destIndex = (editableImg.width * b * editableImg.nChannels) + (c * editableImg.nChannels);
                        *(byte*)(destAddress + destIndex + 0) = *(byte*)(srcAddress + srcIndex + 0);
                        *(byte*)(destAddress + destIndex + 1) = 0;
                        *(byte*)(destAddress + destIndex + 2) = 0;
                    }
                CvSize size = new CvSize(pictureBox2.Width, pictureBox2.Height);
                IplImage resizedImg = cvlib.CvCreateImage(size, editableImg.depth, editableImg.nChannels);
                cvlib.CvResize(ref editableImg, ref resizedImg, cvlib.CV_INTER_LINEAR);
                pictureBox3.BackgroundImage = (Image)resizedImg;
            }
        }

        private void minFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmp = (Bitmap)image1;
            Bitmap result = new Bitmap(bmp.Width, bmp.Height);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color minColor = Color.White;

                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            int posX = i + x;
                            int posY = j + y;

                            if (posX >= 0 && posX < bmp.Width && posY >= 0 && posY < bmp.Height)
                            {
                                Color currentColor = bmp.GetPixel(posX, posY);
                                minColor = Color.FromArgb(Math.Min(minColor.R, currentColor.R),
                                                          Math.Min(minColor.G, currentColor.G),
                                                          Math.Min(minColor.B, currentColor.B));
                            }
                        }
                    }

                    result.SetPixel(i, j, minColor);
                }
            }

            pictureBox4.Image = result;

        }

        private void maxFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmp = (Bitmap)image1;
            Bitmap result = new Bitmap(bmp.Width, bmp.Height);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color maxColor = Color.Black;

                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            int posX = i + x;
                            int posY = j + y;

                            if (posX >= 0 && posX < bmp.Width && posY >= 0 && posY < bmp.Height)
                            {
                                Color currentColor = bmp.GetPixel(posX, posY);
                                maxColor = Color.FromArgb(Math.Max(maxColor.R, currentColor.R),
                                                          Math.Max(maxColor.G, currentColor.G),
                                                          Math.Max(maxColor.B, currentColor.B));
                            }
                        }
                    }

                    result.SetPixel(i, j, maxColor);
                }
            }
            pictureBox4.Image = result;

        }

        private void meanFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
             bmp = (Bitmap)image1;
            Bitmap result = new Bitmap(bmp.Width, bmp.Height);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    int totalR = 0;
                    int totalG = 0;
                    int totalB = 0;
                    int count = 0;

                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            int posX = i + x;
                            int posY = j + y;

                            if (posX >= 0 && posX < bmp.Width && posY >= 0 && posY < bmp.Height)
                            {
                                Color currentColor = bmp.GetPixel(posX, posY);
                                totalR += currentColor.R;
                                totalG += currentColor.G;
                                totalB += currentColor.B;
                                count++;
                            }
                        }
                    }

                    int avgR = totalR / count;
                    int avgG = totalG / count;
                    int avgB = totalB / count;
                    result.SetPixel(i, j, Color.FromArgb(avgR, avgG, avgB));
                }
            }
            pictureBox4.Image = result;
        }

        private void medianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
             bmp = (Bitmap)image1;
             Bitmap result = new Bitmap(bmp.Width, bmp.Height);

        for (int i = 0; i < bmp.Width; i++)
        {
            for (int j = 0; j < bmp.Height; j++)
            {
                int[] redValues = new int[9];
                int[] greenValues = new int[9];
                int[] blueValues = new int[9];
                int count = 0;

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        int posX = i + x;
                        int posY = j + y;

                        if (posX >= 0 && posX < bmp.Width && posY >= 0 && posY < bmp.Height)
                        {
                            Color currentColor = bmp.GetPixel(posX, posY);
                            redValues[count] = currentColor.R;
                            greenValues[count] = currentColor.G;
                            blueValues[count] = currentColor.B;
                            count++;
                        }
                    }
                }

                Array.Sort(redValues);
                Array.Sort(greenValues);
                Array.Sort(blueValues);

                int medianR = redValues[count / 2];
                int medianG = greenValues[count / 2];
                int medianB = blueValues[count / 2];

                result.SetPixel(i, j, Color.FromArgb(medianR, medianG, medianB));
            }
        }
        pictureBox4.Image = result;

        }

        private void gaussianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmp = (Bitmap)image1;
            int[,] matrix = new int[,]
        {
            { 1, 2, 1 },
            { 2, 4, 2 },
            { 1, 2, 1 }
        };

            Bitmap result = new Bitmap(bmp.Width, bmp.Height);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    int totalR = 0;
                    int totalG = 0;
                    int totalB = 0;
                    int matrixSum = 0;

                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            int posX = i + x;
                            int posY = j + y;

                            if (posX >= 0 && posX < bmp.Width && posY >= 0 && posY < bmp.Height)
                            {
                                Color currentColor = bmp.GetPixel(posX, posY);
                                int weight = matrix[x + 1, y + 1];
                                totalR += currentColor.R * weight;
                                totalG += currentColor.G * weight;
                                totalB += currentColor.B * weight;
                                matrixSum += weight;
                            }
                        }
                    }
                    int avgR = totalR / matrixSum;
                    int avgG = totalG / matrixSum;
                    int avgB = totalB / matrixSum;

                    result.SetPixel(i, j, Color.FromArgb(avgR, avgG, avgB));
                }
            }
            pictureBox4.Image = result;
            }

           }
         }

