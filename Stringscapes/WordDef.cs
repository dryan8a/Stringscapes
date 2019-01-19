using Newtonsoft.Json;

namespace Stringscapes
{

    public class WordDef
    {
        [JsonProperty("Metadata")]
        public Metadata MetaData { get; set; }

        [JsonProperty("results")]
        public Result[] Results { get; set; }

    }

    public class Metadata
    {
        public string provider { get; set; }
    }

    public class Result
    {
        public string id { get; set; }
        public string language { get; set; }
        public Lexicalentry[] lexicalEntries { get; set; }
        public string type { get; set; }
        public string word { get; set; }
    }

    public class Lexicalentry
    {
        public Entry[] entries { get; set; }
        public string language { get; set; }
        public string lexicalCategory { get; set; }
        public Pronunciation[] pronunciations { get; set; }
        public string text { get; set; }
    }

    public class Entry
    {
        public string[] etymologies { get; set; }
        public Grammaticalfeature[] grammaticalFeatures { get; set; }
        public string homographNumber { get; set; }
        public Sens[] senses { get; set; }
    }

    public class Grammaticalfeature
    {
        public string text { get; set; }
        public string type { get; set; }
    }

    public class Sens
    {
        public string[] definitions { get; set; }
        public Example[] examples { get; set; }
        public string id { get; set; }
        public string[] short_definitions { get; set; }
        public Subsens[] subsenses { get; set; }
        public Thesauruslink1[] thesaurusLinks { get; set; }
        public string[] registers { get; set; }
    }

    public class Example
    {
        public string text { get; set; }
    }

    public class Subsens
    {
        public string[] definitions { get; set; }
        public Example1[] examples { get; set; }
        public string id { get; set; }
        public string[] short_definitions { get; set; }
        public Thesauruslink[] thesaurusLinks { get; set; }
        public string[] registers { get; set; }
    }

    public class Example1
    {
        public string text { get; set; }
    }

    public class Thesauruslink
    {
        public string entry_id { get; set; }
        public string sense_id { get; set; }
    }

    public class Thesauruslink1
    {
        public string entry_id { get; set; }
        public string sense_id { get; set; }
    }

    public class Pronunciation
    {
        public string audioFile { get; set; }
        public string[] dialects { get; set; }
        public string phoneticNotation { get; set; }
        public string phoneticSpelling { get; set; }
    }

}
