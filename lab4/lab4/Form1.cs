using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace lab4
{
    public partial class Form1 : Form
    {
        private Thread ballThread, rectangleThread, sineWaveThread, triangleThread;

        private ManualResetEvent ballPauseEvent = new ManualResetEvent(true);

        private ManualResetEvent rectanglePauseEvent = new ManualResetEvent(true);

        private ManualResetEvent sineWavePauseEvent = new ManualResetEvent(true);
        private PictureBox sineWavePictureBox = new PictureBox();

        private ManualResetEvent spinningTrianglePauseEvent = new ManualResetEvent(true);
        private PictureBox spinningTrianglePictureBox = new PictureBox();
        private float rotationAngle = 0.0f;
        private float rotationSpeed = 2.0f;


        public Form1()
        {
            InitializeComponent();
            this.Load += new System.EventHandler(this.Form1_Load);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ballThread = new Thread(new ThreadStart(BallThreadFunction));
            ballThread.Start();

            rectangleThread = new Thread(new ThreadStart(RectangleThreadFunction));
            rectangleThread.Start();

            sineWaveThread = new Thread(new ThreadStart(SineWaveThreadFunction));
            sineWaveThread.Start();

            triangleThread = new Thread(new ThreadStart(SpinningTriangleThreadFunction));
            triangleThread.Start();
        }

        private void BallThreadFunction()
        {
            
            PictureBox ballPictureBox = new PictureBox();
            ballPictureBox.BackColor = Color.Red;
            ballPictureBox.Width = 20;
            ballPictureBox.Height = 20;
            ballPictureBox.Location = new Point(225, panel1.Height / 2 - ballPictureBox.Height / 2);
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, ballPictureBox.Width, ballPictureBox.Height);
            ballPictureBox.Region = new Region(path);

            
            panel1.Invoke((MethodInvoker)delegate
            {
                panel1.Controls.Add(ballPictureBox);
            });

            int direction = 1;
            int speed = 5;

            while (true)
            {
                if (ballPauseEvent.WaitOne(0))
                {
                    if (ballPictureBox.Location.X + ballPictureBox.Width + speed >= panel1.Width)
                    {
                        direction = -1;
                    }
                    else if (ballPictureBox.Location.X - speed <= 0)
                    {
                        direction = 1;
                    }

                    panel1.Invoke((MethodInvoker)delegate
                    {
                        if (!panel1.IsDisposed && !ballPictureBox.IsDisposed)
                        {
                            ballPictureBox.Location = new Point(ballPictureBox.Location.X + direction * speed, ballPictureBox.Location.Y);
                        }
                    });
                }

                
                Thread.Sleep(50);
            }
        }



        private void RectangleThreadFunction()
        {
            PictureBox rectanglePictureBox = new PictureBox();
            rectanglePictureBox.BackColor = Color.Blue;
            rectanglePictureBox.Width = 50;
            rectanglePictureBox.Height = 30;
            rectanglePictureBox.Location = new Point(50, 50);

            panel2.Invoke((MethodInvoker)delegate
            {
                if (!panel2.IsDisposed && !rectanglePictureBox.IsDisposed)
                {
                    panel2.Controls.Add(rectanglePictureBox);
                }
            });

            int widthDirection = 1;
            int heightDirection = 1;
            int widthSpeed = 2;
            int heightSpeed = 3;

            while (true)
            {
                if (rectanglePauseEvent.WaitOne(0))
                {
                    if (rectanglePictureBox.Width + widthSpeed >= 100 || rectanglePictureBox.Width - widthSpeed <= 20)
                    {
                        widthDirection = -widthDirection;
                    }

                    if (rectanglePictureBox.Height + heightSpeed >= 100 || rectanglePictureBox.Height - heightSpeed <= 20)
                    {
                        heightDirection = -heightDirection;
                    }

                    panel2.Invoke((MethodInvoker)delegate
                    {
                        if (!panel2.IsDisposed && !rectanglePictureBox.IsDisposed)
                        {
                            rectanglePictureBox.Width += widthDirection * widthSpeed;
                            rectanglePictureBox.Height += heightDirection * heightSpeed;
                        }
                    });
                }

                Thread.Sleep(50);
            }
        }

        private void SineWaveThreadFunction()
        {
            
            sineWavePictureBox.BackColor = Color.White;
            sineWavePictureBox.Width = 197;
            sineWavePictureBox.Height = 169;
            sineWavePictureBox.Location = new Point(0, 0);

            panel3.Invoke((MethodInvoker)delegate
            {
                if (!panel3.IsDisposed && !sineWavePictureBox.IsDisposed)
                {
                    panel3.Controls.Add(sineWavePictureBox);
                }
            });

            int amplitude = 50;
            int frequency = 2;
            int offsetX = 0;
            int offsetY = sineWavePictureBox.Height / 2;
            double phase = 0;
            double phaseIncrement = 0.1;

            while (true)
            {
                if (sineWavePauseEvent.WaitOne(0))
                {
                    List<Point> points = new List<Point>();
                    for (int x = 0; x <= sineWavePictureBox.Width; x += 2)
                    {
                        int y = (int)(amplitude * Math.Sin(2 * Math.PI * frequency * x / sineWavePictureBox.Width + phase)) + offsetY;
                        points.Add(new Point(x + offsetX, y));
                    }

                    Bitmap bmp = new Bitmap(sineWavePictureBox.Width, sineWavePictureBox.Height);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.White);
                        g.DrawLines(Pens.Green, points.ToArray());
                    }

                    panel3.Invoke((MethodInvoker)delegate
                    {
                        if (!panel3.IsDisposed && !sineWavePictureBox.IsDisposed)
                        {
                            sineWavePictureBox.Image?.Dispose();
                            sineWavePictureBox.Image = bmp;
                        }
                    });

                    phase += phaseIncrement;
                }

                Thread.Sleep(50);
            }
        }

        private void SpinningTriangleThreadFunction()
        {
            spinningTrianglePictureBox.BackColor = Color.Transparent;
            spinningTrianglePictureBox.Width = 197;
            spinningTrianglePictureBox.Height = 169;
            spinningTrianglePictureBox.Location = new Point(0, 0);

            panel4.Invoke((MethodInvoker)delegate
            {
                if (!panel4.IsDisposed && !spinningTrianglePictureBox.IsDisposed)
                {
                    panel4.Controls.Add(spinningTrianglePictureBox);
                }
            });

            while (true)
            {
                if (spinningTrianglePauseEvent.WaitOne(0))
                {
                    Bitmap bmp = new Bitmap(spinningTrianglePictureBox.Width, spinningTrianglePictureBox.Height);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.Transparent);

                        PointF[] points = CalculateRotatedTrianglePoints(rotationAngle, spinningTrianglePictureBox.Width, spinningTrianglePictureBox.Height);
                        g.FillPolygon(Brushes.Orange, points);
                    }

                    panel4.Invoke((MethodInvoker)delegate
                    {
                        if (!panel4.IsDisposed && !spinningTrianglePictureBox.IsDisposed)
                        {
                            spinningTrianglePictureBox.Image?.Dispose();
                            spinningTrianglePictureBox.Image = bmp;
                        }
                    });
                    rotationAngle += rotationSpeed;
                    if (rotationAngle >= 360.0f)
                    {
                        rotationAngle -= 360.0f;
                    }
                }

                Thread.Sleep(50);
            }
        }

        private PointF[] CalculateRotatedTrianglePoints(float angle, float width, float height)
        {
            PointF[] points = new PointF[3];
            float centerX = width / 2;
            float centerY = height / 2;

            for (int i = 0; i < 3; i++)
            {
                float x = (float)(centerX + Math.Cos(DegreesToRadians(angle + i * 120)) * width / 2);
                float y = (float)(centerY + Math.Sin(DegreesToRadians(angle + i * 120)) * height / 2);
                points[i] = new PointF(x, y);
            }

            return points;
        }

        private double DegreesToRadians(float degrees)
        {
            return degrees * Math.PI / 180.0;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (ballPauseEvent.WaitOne(0))
            {
                ballPauseEvent.Reset();
            }
            else
            {
                ballPauseEvent.Set();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (rectanglePauseEvent.WaitOne(0))
            {
                rectanglePauseEvent.Reset();
            }
            else
            {
                rectanglePauseEvent.Set();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (sineWavePauseEvent.WaitOne(0))
            {
                sineWavePauseEvent.Reset();
            }
            else
            {
                sineWavePauseEvent.Set();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (spinningTrianglePauseEvent.WaitOne(0))
            {
                spinningTrianglePauseEvent.Reset();
            }
            else
            {
                spinningTrianglePauseEvent.Set();
            }
        }
    }
}