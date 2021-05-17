using Exercise_Organizer.Json;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Path = System.IO.Path;

namespace Exercise_Organizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static string exerciseSettingsPath = "English.txt";
        public static string exerciseDataFolderPath = "English\\";

        public static readonly string BlankDefination = "##Blank##";

        private void ButtonInsertBlank_Click(object sender, RoutedEventArgs e)
        {
            int SelectionStart = TextBoxFillBlank.SelectionStart;
            TextBoxFillBlank.Text = TextBoxFillBlank.Text.Insert(TextBoxFillBlank.SelectionStart, " " + BlankDefination + " ");
            TextBoxFillBlank.SelectionStart = SelectionStart + 2 + BlankDefination.Length;
            TextBoxFillBlank.Focus();
        }

        private void ButtonSaveFillBlank_Click(object sender, RoutedEventArgs e)
        {
            int x = 0;
            while (File.Exists(exerciseDataFolderPath + x.ToString("X8") + ".json"))
                x++;
            string fileName = exerciseDataFolderPath + x.ToString("X8") + ".json";

            EnglishFillBlank englishFillBlank = new EnglishFillBlank();
            englishFillBlank.ExerciseCommonData = GetExerciseCommonData();
            englishFillBlank.Text = TextBoxFillBlank.Text;
            englishFillBlank.BlankDefination = BlankDefination;
            englishFillBlank.Answer = TextBoxFillBlankAnswer.Text;

            string json = JsonConvert.SerializeObject(englishFillBlank, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            if (!Directory.Exists(exerciseDataFolderPath))
            {
                Directory.CreateDirectory(exerciseDataFolderPath);
            }
            File.WriteAllText(fileName, json);

            if (CheckBoxAutoClear.IsChecked == true)
            {
                TextBoxFillBlank.Text = "";
                TextBoxFillBlankAnswer.Text = "";
            }
        }

        private ExerciseCommonData GetExerciseCommonData()
        {
            ExerciseCommonData exerciseCommonData = new ExerciseCommonData();
            exerciseCommonData.DateTime = DateTime.Now;
            exerciseCommonData.AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
            exerciseCommonData.SourceFrom = TextBoxSource.Text;
            return exerciseCommonData;
        }

        private void ButtonSaveAll_Click(object sender, RoutedEventArgs e)
        {
            string output = "Output.rtf";

            RichTextBox richTextBox = new RichTextBox();
            richTextBox.FontSize = 14;
            richTextBox.FontFamily = new FontFamily("Times New Roman");
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.LineHeight = 25;

            bool[] isFileExist = new bool[0x3F3F3F3F];
            int maxN = 0;
            foreach (FileInfo fileInfo in new DirectoryInfo(exerciseDataFolderPath).GetFiles())
            {
                int n = Convert.ToInt32(Path.GetFileNameWithoutExtension(fileInfo.FullName), 16);
                maxN = Math.Max(n, maxN);
                isFileExist[n] = true;
            }

            int k = 1;
            for (int i = 0; i <= maxN; i++)
            {
                EnglishFillBlank englishFillBlank = JsonConvert.DeserializeObject<EnglishFillBlank>(
                    File.ReadAllText(exerciseDataFolderPath + i.ToString("X8") + ".json"));
                if (englishFillBlank.ExerciseCommonData.OutputMark)
                {
                    string text = englishFillBlank.Text;
                    if(CheckBoxAutoFix.IsChecked == true)
                    {
                        text = TextAutoFix(text);
                    }
                    if (englishFillBlank.ExerciseCommonData.SourceFrom != "")
                    {
                        text = "[" + englishFillBlank.ExerciseCommonData.SourceFrom + "] " + text;
                    }
                    text = text.Replace(englishFillBlank.BlankDefination, "__________");
                    RichTextBoxAppend(richTextBox, 
                        string.Format("{0}\t{1}", k++ + ".", text));
                    //string.Format("{0,-5}{1}", k++ + ".", text));
                }
            }

            RichTextBoxAppend(richTextBox, "");
            RichTextBoxAppend(richTextBox, "");
            RichTextBoxAppend(richTextBox, "答案：");

            k = 1;
            for (int i = 0; i <= maxN; i++)
            {
                EnglishFillBlank englishFillBlank = JsonConvert.DeserializeObject<EnglishFillBlank>(
                    File.ReadAllText(exerciseDataFolderPath + i.ToString("X8") + ".json"));
                if (englishFillBlank.ExerciseCommonData.OutputMark)
                {
                    RichTextBoxAppend(richTextBox, string.Format("{0}\t{1}", k++ + ".", englishFillBlank.Answer));
                }
            }

            File.WriteAllText(output, RichTextBoxEx.RTF(richTextBox));
        }
        public void RichTextBoxAppend(RichTextBox richTextBox, string text)
        {
            Run run = new Run(text);
            Paragraph para = new Paragraph();
            para.Inlines.Add(run);
            richTextBox.Document.Blocks.Add(para);
        }

        private void TextBoxFillBlank_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ButtonAutoFix_Click(object sender, RoutedEventArgs e)
        {
            TextBoxFillBlank.Text = TextAutoFix(TextBoxFillBlank.Text);
        }

        public string TextAutoFix(string str)
        {
            str = str.Trim().Replace("（", "(").Replace("）", ")");
            str = RemoveSpaceAround(str, "(");
            str = RemoveSpaceAround(str, ")");
            str = RemoveSpaceAround(str, ",").Replace(" ,", ",");
            str = RemoveSpaceAround(str, ".").Replace(" .", ".");
            str = str.Replace("  ", " ").Replace("  ", " ").Replace("--", "——");

            str = str[0].ToString().ToUpper() + Strings.Mid(str, 2);

            str = str.Replace("I'ii", "I'll").Replace(" i ", " I ");

            return str;
        }

        public string RemoveSpaceAround(string str, string centerString)
        {
            str = str.Replace(centerString + " ", centerString)
                     .Replace(" " + centerString, centerString)
                     .Replace(centerString + " ", centerString)
                     .Replace(" " + centerString, centerString)
                     .Replace(centerString, " " + centerString + " ");
            return str;
        }
    }
    public static class RichTextBoxEx
    {
        public static string RTF(this RichTextBox richTextBox)
        {
            string rtf = string.Empty;
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            using (MemoryStream ms = new MemoryStream())
            {
                textRange.Save(ms, System.Windows.DataFormats.Rtf);
                ms.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(ms);
                rtf = sr.ReadToEnd();
            }

            return rtf;
        }

        public static void LoadFromRTF(this RichTextBox richTextBox, string rtf)
        {
            if (string.IsNullOrEmpty(rtf))
            {
                throw new ArgumentNullException();
            }
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write(rtf);
                    sw.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    textRange.Load(ms, DataFormats.Rtf);
                }
            }
        }

    }
}
