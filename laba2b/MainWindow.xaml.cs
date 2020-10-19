using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Win32;
//using System.Windows.Forms;

namespace laba2b
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Drawing.Image ImageBuffer;
        System.Drawing.Image SecondBuffer;
        String FileName;
        System.Drawing.Color CurrentColor = System.Drawing.Color.Black;
        List<System.Drawing.Point> DrawingNodes = new List<System.Drawing.Point>();
        bool IsDrawing = false;
        public MainWindow()
        {
            InitializeComponent();
        }
        public void Display(System.Drawing.Image bmp)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    ms.Position = 0;

                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = ms;
                    bi.EndInit();
                    StackPanel_Picture.Height = bmp.Height;
                    StackPanel_Picture.Width = bmp.Width;
                    Image_Picture.Height = bmp.Height;
                    Image_Picture.Width = bmp.Width;
                    Image_Picture.Source = bi;

                    //bmp.Dispose();
                }
            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }


        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_SaveFile_Click(object sender, RoutedEventArgs e)
        {
            SecondBuffer.Save(FileName);
            ImageBuffer = (System.Drawing.Image)SecondBuffer.Clone();
            ResetControls();
        }

        private void MenuItem_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SaveFileDialog_Save = new SaveFileDialog();
            if (SaveFileDialog_Save.ShowDialog() == true && SaveFileDialog_Save.FileName.Length > 0)
            {
                SecondBuffer.Save(SaveFileDialog_Save.FileName);
            }
            FileName = SaveFileDialog_Save.FileName;
            ImageBuffer = (System.Drawing.Image)SecondBuffer.Clone();
            Window_Main.Title = SaveFileDialog_Save.FileName + " - Image Editor";
            ResetControls();
        }

        private void MenuItem_OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenFileDialog_Open = new OpenFileDialog();
            if (OpenFileDialog_Open.ShowDialog() == true && OpenFileDialog_Open.FileName.Length > 0)
            {
                using (FileStream fs = new FileStream(OpenFileDialog_Open.FileName, FileMode.Open, FileAccess.Read))
                {
                    ImageBuffer = System.Drawing.Image.FromStream(fs);
                    SecondBuffer = System.Drawing.Image.FromStream(fs);
                    Display(System.Drawing.Image.FromStream(fs));
                }
                Window_Main.Title = OpenFileDialog_Open.FileName + " - Image Editor";
                FileName = OpenFileDialog_Open.FileName;
                Border_Tools.IsEnabled = true;
            }
        }

        private void Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            ImageBuffer = (System.Drawing.Image)SecondBuffer.Clone();
            ResetControls();
        }
        void ResetControls()
        {
            Slider_Rotation.Value = 0;
            Slider_Height.Value = 100;
            Slider_Width.Value = 100;
            Slider_Contrast.Value = 100;
            Slider_Brightness.Value = 0;
            Slider_Red.Value = 100;
            Slider_Green.Value = 100;
            Slider_Blue.Value = 100;
            TextBlock_Red.Text = "100%";
            TextBlock_Green.Text = "100%";
            TextBlock_Blue.Text = "100%";
            TextBlock_Brightness.Text = "100%";
            TextBlock_Contrast.Text = "100%";
            TextBlock_Width.Text = "100%";
            TextBlock_Height.Text = "100%";
            TextBlock_Rotation.Text = "0°";
            IsDrawing = false;
        }
        Bitmap RotateImageN(Bitmap b, float Angle)
        {
            float wOver2 = b.Width / 2.0f;
            float hOver2 = b.Height / 2.0f;
            float radians = -(float)(Angle / 180.0 * Math.PI);
            PointF[] corners = new PointF[]{
                new PointF(-wOver2, -hOver2),
                new PointF(+wOver2, -hOver2),
                new PointF(+wOver2, +hOver2),
                new PointF(-wOver2, +hOver2)
            };
            for (int i = 0; i < 4; i++)
            {
                PointF p = corners[i];
                PointF newP = new PointF((float)(p.X * Math.Cos(radians) - p.Y * Math.Sin(radians)), (float)(p.X * Math.Sin(radians) + p.Y * Math.Cos(radians)));
                corners[i] = newP;
            }
            float minX = corners[0].X;
            float maxX = minX;
            float minY = corners[0].Y;
            float maxY = minY;
            for (int i = 1; i < 4; i++)
            {
                PointF p = corners[i];
                minX = Math.Min(minX, p.X);
                maxX = Math.Max(maxX, p.X);
                minY = Math.Min(minY, p.Y);
                maxY = Math.Max(maxY, p.Y);
            }
            SizeF newSize = new SizeF(maxX - minX, maxY - minY);
            Bitmap returnBitmap = new Bitmap((int)Math.Ceiling(newSize.Width), (int)Math.Ceiling(newSize.Height));
            using (Graphics g = Graphics.FromImage(returnBitmap))
            {
                g.TranslateTransform(newSize.Width / 2.0f, newSize.Height / 2.0f);
                g.RotateTransform(Angle);
                g.TranslateTransform(-b.Width / 2.0f, -b.Height / 2.0f);

                g.DrawImage(b, 0, 0);
            }

            return returnBitmap;
        }
        private void Slider_Rotation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Bitmap tmp = new Bitmap(ImageBuffer);
            if (Slider_Rotation.Value == 360)
                Slider_Rotation.Value = 0;
            if (Slider_Rotation.Value == -1)
                Slider_Rotation.Value = 359;
            Bitmap btm = RotateImageN(tmp, (float)Slider_Rotation.Value);
            SecondBuffer.Dispose();
            SecondBuffer = btm;
            Display(SecondBuffer);
            tmp.Dispose();
            TextBlock_Rotation.Text = Slider_Rotation.Value.ToString() + "°";
        }
        Bitmap ResizeImage(Bitmap source, System.Drawing.Size NewSize)
        {
            Bitmap bm_source = new Bitmap(source);
            Bitmap bm_dest = new Bitmap(NewSize.Width, NewSize.Height,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics gr_dest = Graphics.FromImage(bm_dest))
            {
                gr_dest.CompositingQuality = CompositingQuality.HighQuality;
                gr_dest.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr_dest.SmoothingMode = SmoothingMode.HighQuality;
                gr_dest.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr_dest.DrawImage(bm_source, 0, 0, bm_dest.Width, bm_dest.Height);
            }
            bm_source.Dispose();
            return bm_dest;
        }
        private void Slider_Size_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ImageBuffer != null)
            {
                Bitmap tmp = new Bitmap(ImageBuffer);
                SecondBuffer.Dispose();
                SecondBuffer = ResizeImage(tmp, new System.Drawing.Size(Convert.ToInt32(ImageBuffer.Width * Slider_Width.Value / 100), Convert.ToInt32(ImageBuffer.Height * Slider_Height.Value / 100)));
                tmp.Dispose();
                Display(SecondBuffer);
                TextBlock_Width.Text = Slider_Width.Value.ToString() + "%";
                TextBlock_Height.Text = Slider_Height.Value.ToString() + "%";
            }
        }
        Bitmap TransformColor(Bitmap source, ColorMatrix matrix)
        {
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            Graphics g = Graphics.FromImage(source);
            g.DrawImage(source, new System.Drawing.Rectangle(0, 0, source.Width, source.Height), 0, 0,
                source.Width, source.Height, GraphicsUnit.Pixel, imageAttributes);
            g.Dispose();
            imageAttributes.Dispose();
            return source;
        }
        private void Slider_Contrast_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ImageBuffer != null)
            {
                ColorMatrix matrix = new ColorMatrix();
                float c = (float)Slider_Contrast.Value / 100f;
                matrix.Matrix00 = matrix.Matrix11 = matrix.Matrix22 = c;
                matrix.Matrix40 = matrix.Matrix41 = matrix.Matrix42 = (1 - c) / 2;

                Bitmap tmp = new Bitmap(ImageBuffer);
                SecondBuffer.Dispose();
                SecondBuffer = TransformColor(tmp, matrix);
                Display(SecondBuffer);
                TextBlock_Contrast.Text = Slider_Contrast.Value.ToString() + "%";
            }
        }

        private void Slider_Brightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ImageBuffer != null)
            {
                ColorMatrix matrix = new ColorMatrix();
                float b = (float)Slider_Brightness.Value / 100f;
                matrix.Matrix40 = matrix.Matrix41 = matrix.Matrix42 = b;

                Bitmap tmp = new Bitmap(ImageBuffer);
                SecondBuffer.Dispose();
                SecondBuffer = TransformColor(tmp, matrix);
                Display(SecondBuffer);
                TextBlock_Brightness.Text = (Slider_Brightness.Value + 100).ToString() + "%";
            }
        }
        private void Slider_Saturation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ImageBuffer != null)
            {
                float rwgt = 0.3086f;
                float gwgt = 0.6094f;
                float bwgt = 0.0820f;
                float rs = (float)Slider_Red.Value / 100f;
                float gs = (float)Slider_Green.Value / 100f;
                float bs = (float)Slider_Blue.Value / 100f;
                float s = rs + gs + bs;
                float olds = s;
                if (s > 1.0)
                {
                    rs /= s;
                    gs /= s;
                    bs /= s;
                    s = 1;
                }
                else olds = 1;
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix00 = ((float)(1.0 - s) * rwgt + rs) * olds;
                matrix.Matrix01 = ((float)(1.0 - s) * rwgt) * olds;
                matrix.Matrix02 = ((float)(1.0 - s) * rwgt) * olds;
                matrix.Matrix10 = ((float)(1.0 - s) * gwgt) * olds;
                matrix.Matrix11 = ((float)(1.0 - s) * gwgt + gs) * olds;
                matrix.Matrix12 = ((float)(1.0 - s) * gwgt) * olds;
                matrix.Matrix20 = ((float)(1.0 - s) * bwgt) * olds;
                matrix.Matrix21 = ((float)(1.0 - s) * bwgt) * olds;
                matrix.Matrix22 = ((float)(1.0 - s) * bwgt + bs) * olds;

                Bitmap tmp = new Bitmap(ImageBuffer);
                SecondBuffer.Dispose();
                SecondBuffer = TransformColor(tmp, matrix);
                Display(SecondBuffer);
                TextBlock_Red.Text = Slider_Red.Value.ToString() + "%";
                TextBlock_Green.Text = Slider_Green.Value.ToString() + "%";
                TextBlock_Blue.Text = Slider_Blue.Value.ToString() + "%";
            }
        }

        private void Button_Palette_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog ColorDialog_BrushColor = new System.Windows.Forms.ColorDialog();
            if (Convert.ToInt32(ColorDialog_BrushColor.ShowDialog()) == 1)
            {
                CurrentColor = ColorDialog_BrushColor.Color;
                Button_SelectedColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(CurrentColor.A, CurrentColor.R, CurrentColor.G, CurrentColor.B));

            }
        }

        private void Button_Draw_Click(object sender, RoutedEventArgs e)
        {
            IsDrawing = !IsDrawing;
        }

        private void StackPanel_Picture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsDrawing && (e.ChangedButton & MouseButton.Left) == MouseButton.Left)
            {
                DrawingNodes.Add(new System.Drawing.Point(Convert.ToInt32(e.GetPosition(StackPanel_Picture).X), Convert.ToInt32(e.GetPosition(StackPanel_Picture).Y)));
            }
        }

        private void StackPanel_Picture_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsDrawing)
            {
                DrawingNodes.Clear();
            }
        }
        private void StackPanel_Picture_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDrawing)
            {
                
                if (e.LeftButton==MouseButtonState.Pressed)
                {
                    DrawingNodes.Add(new System.Drawing.Point(Convert.ToInt32(e.GetPosition(StackPanel_Picture).X), Convert.ToInt32(e.GetPosition(StackPanel_Picture).Y)));

                    using (Graphics grp = Graphics.FromImage(SecondBuffer))
                    {
                        if (DrawingNodes.Count > 1)
                            grp.DrawLines(new System.Drawing.Pen(CurrentColor, (float)Slider_PenWidth.Value), DrawingNodes.ToArray());
                    }
                    Display(SecondBuffer);
                    //PictureBox_Picture.Refresh();
                    //PictureBox_Picture.Invalidate();
                }
            }
        }

        private void Slider_PenWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ImageBuffer != null)
            {
                TextBlock_PenWidth.Text = Slider_PenWidth.Value.ToString();
            }
        }
    }
}
