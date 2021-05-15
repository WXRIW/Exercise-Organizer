using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exercise_Organizer.Json
{
    public class ExerciseCommonData
    {
        public DateTime DateTime { get; set; } = new DateTime();
        public Version AppVersion { get; set; } = new Version();
        public string SourceFrom { get; set; }
        public bool OutputMark { get; set; } = true;
    }

    public class EnglishFillBlank
    {
        public ExerciseCommonData ExerciseCommonData { get; set; } = new ExerciseCommonData();
        public string Text { get; set; }
        public string BlankDefination { get; set; }
        public string Answer { get; set; }
    }

    public class EnglishChoice
    {

    }
}
