using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ISHIHARA
{
    public partial class Form1 : Form
    {
        private static readonly string ImagesDirectoryPath = AppDomain.CurrentDomain.BaseDirectory + "\\obrazki\\";
        private static readonly string DotsDirectoryPath = ImagesDirectoryPath + "\\kropki\\";

        private static readonly string[] AcceptableGraphicFileExtensions = { "*.jpg", "*.jpeg", "*.png", "*.bmp" };

        public Form1()
        {
            InitializeComponent();

            try
            {
                // Wczytanie dostepnych obrazow z tekstem i dodanie ich do wyboru w ComboBoxie
                var imagesDir = new DirectoryInfo(ImagesDirectoryPath);
                // Utworzenie katalogu z obrazami jeżeli nie istneje
                if(!imagesDir.Exists)
                {
                    imagesDir.Create();
                }

                var files = new List<FileInfo>();
                foreach(var extension in AcceptableGraphicFileExtensions)
                {
                    files.AddRange(imagesDir.GetFiles(extension).ToList());
                }

                if(files.Count == 0)
                {
                    MessageBox.Show(string.Format("Brak obrazów z tekstem w lokalizacji {0}", imagesDir.FullName));
                }

                foreach(var fileInfo in files)
                {
                    SelectTextComboBox.Items.Add(fileInfo.Name);
                }

                // Wczytanie dosteonych obrazow z kropkami i dodanie ich do wyboru w ComboBoxie
                var dotsDir = new DirectoryInfo(DotsDirectoryPath);
                // Utworzenie katalogu dla obrazów z wykropkowanym tłem jeżeli nie istnieje
                if (!dotsDir.Exists)
                {
                    dotsDir.Create();
                }

                var dotFiles = new List<FileInfo>();
                foreach (var extension in AcceptableGraphicFileExtensions)
                {
                    dotFiles.AddRange(dotsDir.GetFiles(extension).ToList());
                }

                if (dotFiles.Count == 0)
                {
                    MessageBox.Show(string.Format("Brak obrazów z tłem w lokalizacji {0}", dotsDir.FullName));
                }

                foreach (var fileInfo in dotFiles)
                {
                    SelectDotsPatternComboBox.Items.Add(fileInfo.Name);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Nałożene tekstu na kropki i utworzenie na tej podstawie testu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdCombine_Click(object sender, EventArgs e)
        {
            Bitmap textBitmap = textPictureBox.Image as Bitmap;
            Bitmap dotsBitmap = dotsPictureBox.Image as Bitmap;

            if(textBitmap == null)
            {
                MessageBox.Show("Wybierz obraz z tekstem");
                return;
            }

            if (dotsBitmap == null)
            {
                MessageBox.Show("Wybierz tło z kropkami");
                return;
            }

            var imageManager = new ImageManager((Bitmap)dotsBitmap.Clone());
            imageManager.ColorTextInDotImage(textBitmap, GetTextColor(), GetBackgroundColor());

            FinalImagePictureBox.Image = imageManager.Bitmap;
        }

        /// <summary>
        /// Wczytanie obrazu na podstawie wybranego z comboboxa pliku (tekst)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectTextComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            textPictureBox.Image = new Bitmap(ImagesDirectoryPath + comboBox.SelectedItem);
        }

        /// <summary>
        /// Wczytanie obrazu na podstawie wybranego z comboboxa pliku (kropki)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectDotsPatternComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            dotsPictureBox.Image = new Bitmap(DotsDirectoryPath + comboBox.SelectedItem);
        }

        /// <summary>
        /// Pobranie koloru napisu na podstawie zaznaczonego radobuttona
        /// </summary>
        /// <returns></returns>
        private Color GetTextColor()
        {
            // pobranie zaznaczonego radiobuttona z grupy kontrolek związanych z napisem
            var checkedRadio = TextGroupBox.Controls.OfType<RadioButton>().ToList().First(rb => rb.Checked == true);

            if (Equals(checkedRadio, TextRedRadio))
                return Color.Red;

            if (Equals(checkedRadio, TextYellowRadio))
                return Color.Yellow;

            if (Equals(checkedRadio, TextGreenRadio))
                return Color.Green;

            if (Equals(checkedRadio, TextBlueRadio))
                return Color.Blue;

            return Color.Black;
        }

        /// <summary>
        /// Pobranie koloru tła na podstawie zaznaczonego radiobuttona
        /// </summary>
        /// <returns></returns>
        private Color GetBackgroundColor()
        {
            // pobranie zaznaczonego radiobuttona z grupy kontrolek związanych z tłem
            var checkedRadio = BackgroundGroupBox.Controls.OfType<RadioButton>().ToList().First(rb => rb.Checked == true);

            if (Equals(checkedRadio, BackgroundRedRadio))
                return Color.Red;

            if (Equals(checkedRadio, BackgroundYellowRadio))
                return Color.Yellow;

            if (Equals(checkedRadio, BackgroundGreenRadio))
                return Color.Green;

            if (Equals(checkedRadio, BackgroundBlueRadio))
                return Color.Blue;

            return Color.Black;
        }

        /// <summary>
        /// Zapis utworzonego testu do pliku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveTestBtn_Click(object sender, EventArgs e)
        {
            var fileDialog = new SaveFileDialog();
            fileDialog.Filter = "Bitmap Image (.bmp) | *.bmp | JPEG Image(.jpeg) | *.jpeg | JPG Image(.jpg) | *.jpg | Png Image(.png) | *.png";
            var dialogResult = fileDialog.ShowDialog();

            if(dialogResult.Equals(DialogResult.Cancel))
            {
                return;
            }

            var filePath = fileDialog.FileName;

            var bitmapToSave = new Bitmap(FinalImagePictureBox.Image);
            bitmapToSave.Save(filePath);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
