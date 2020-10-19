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
                bmp.Dispose();
            }


        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_SaveFile_Click(object sender, RoutedEventArgs e)
        {
            SecondBuffer.Save(FileName);
            ImageBuffer = (System.Drawing.Image)SecondBuffer.Clone();
            //ResetControls();
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
            //ResetControls();
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
            //ResetControls();
        }
    }
}
