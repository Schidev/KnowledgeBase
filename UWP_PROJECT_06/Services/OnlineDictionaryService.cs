using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary.OnlineDictionary;

namespace UWP_PROJECT_06.Services
{
    public static class OnlineDictionaryService
    {
        static HtmlWeb _html_web = new HtmlWeb();

        enum PartsOfSpeech
        {
            noun_masculine = 1,
            noun_feminine = 2,
            noun_neuter = 3,
            noun_plural_only = 4,
            noun_plural_for = 5,
            verb = 6,
            adjective = 7,
            adverb = 8,
            preposition = 9,
            numeral = 10,
            pronoun = 11,
            conjunction = 12,
            particle = 13,
            interjection = 14,
            possessive_pronoun = 15,
            determiner = 16,
            prefix = 17
        }

        static Dictionary<string, string> pathes = new Dictionary<string, string>()
        {
            { "body_xpath","/html[1]/body[1]/div[2]/div[1]/div[1]/div[2]/article[1]/div[2]/div[1]"},
            { "body", ".//article[@class='hfl-s lt2b lmt-10 lmb-25 lp-s_r-20 x han tc-bd lmt-20 german-english']//div[@class='entry-body']//div[@class='pr dictionary']"},

            { "word_xpath","/html[1]/body[1]/div[2]/div[1]/div[1]/div[2]/article[1]/div[2]/div[1]/span[1]/div[1]/div[3]/h2[1]"},
            { "word", ".//article[@class='hfl-s lt2b lmt-10 lmb-25 lp-s_r-20 x han tc-bd lmt-20 german-english']//div[@class='entry-body']//div[@class='pr dictionary']//div[@class='dpos-h di-head normal-entry']//h2[@class='tw-bw dhw dpos-h_hw di-title ']"},

            { "gender_xpath","/html[1]/body[1]/div[2]/div[1]/div[1]/div[2]/article[1]/div[2]/div[1]/span[1]/div[1]/div[3]/span[1]/span[3]/a[1]/span[1]/span[1]"},
            { "gender", ".//div[@class='dpos-h di-head normal-entry']//span[@class='gc dgc']"},

            { "gender_condition_path", ".//div[@class='dpos-h di-head normal-entry']//span[@class='di-info']/span[@class='gram dgram']"},

            { "word_ending_xpath","/html[1]/body[1]/div[2]/div[1]/div[1]/div[2]/article[1]/div[2]/div[1]/span[1]/div[1]/div[3]/span[1]/div[2]/span"},
            { "word_ending", ".//div[@class='irreg-infls hdib dinfls ']//span[@class='inf-group dinfg ']"},

            { "part_of_speech_xpath","/html[1]/body[1]/div[2]/div[1]/div[1]/div[2]/article[1]/div[2]/div[1]/span[1]/div[1]/div[3]/span[1]/div[1]"},
            { "part_of_speech", ".//div[@class='dpos-g hdib']"},

            { "translations_xpath","/html[1]/body[1]/div[2]/div[1]/div[1]/div[2]/article[1]/div[2]/div[1]/span[1]/div[1]/div[4]/div[1]/div[1]/div[2]/div"},
            { "translations", ".//div[@class='di-body normal-entry-body']//div[@class='sense-block pr dsense dsense-noh']//div[@class='sense-body dsense_b']//div[@class='def-block ddef_block ']"},

            { "examples_xpath","/html[1]/body[1]/div[2]/div[1]/div[1]/div[2]/article[1]/div[2]/div[1]/span[1]/div[1]/div[4]/div[1]/div[1]/div[2]/div[1]/div[3]/div"},
            { "examples", ".//div[@class='di-body normal-entry-body']//div[@class='sense-block pr dsense dsense-noh']//div[@class='sense-body dsense_b']//div[@class='def-body ddef_b ddef_b-t']//div[@class='examp dexamp']"},

            { "examples_xpath_draft","/html[1]/body[1]/div[2]/div[1]/div[1]/div[2]/article[1]/div[2]/div[1]/span[1]/div[1]/div[4]/div[1]/div[1]/div[2]/div[1]/div"},
            { "examples_draft", ".//div[@class='di-body normal-entry-body']//div[@class='sense-block pr dsense dsense-noh']//div[@class='sense-body dsense_b']//div[@class='def-body ddef_b ddef_b-t']"}
        };
        static Dictionary<string, string> genders = new Dictionary<string, string>()
        {
            { "masculine", "m"},
            { "neuter", "n"},
            { "feminine", "f"},
            { "masculine-feminine", "m/f"},
            { "masculine-neuter", "m/n"}
        };
        static Dictionary<string, string> parts_of_speech_dictionary = new Dictionary<string, string>()
            {
                    { "adjective", "adj"},
                    { "adverb", "adv"},
                    { "preposition", "prep"},
                    { "numeral", "num"},
                    { "conjunction", "conj"},
                    { "pronoun", "pron"},
                    { "particle", "part"},
                    { "interjection", "interj"},
                    { "possessive_pronoun", "pos_pron"},
                    { "determiner", "det"},
                    { "prefix", "pref"}
                };
        static Dictionary<string, int> part_of_speech_ToInt32_dictionary = new Dictionary<string, int>()
        {
            { "noun_masculine", 1},
            { "noun_masculine-feminine", 1},
            { "noun_masculine-neuter", 1},
            { "noun_neuter", 2},
            { "noun_feminine", 3},
            { "noun_plural_only", 4},
            { "noun_plural_for", 5},
            { "verb", 6},
            
            { "adjective", 7},
            { "adverb", 8},
            { "preposition", 9},
            { "numeral", 10},
            { "pronoun", 11},
            { "conjunction", 12},
            { "particle", 13},
            { "interjection", 14},
            { "possessive_pronoun", 15},
            { "determiner", 16},
            { "prefix", 17}
        };

        public static Definition GetGermanDefenition(String word)
        {
            var def = new Definition();

            var full_uri = String.Format(@"https://dictionary.cambridge.org/dictionary/german-english/{0}", word);
            var parsed_html = _html_web.Load(full_uri);

            #region Check if it is possible to find this word
            var word_html = parsed_html.DocumentNode.SelectSingleNode(pathes["word_xpath"]);

            if (word_html == null)
                word_html = parsed_html.DocumentNode.SelectSingleNode(pathes["word"]);

            if (word_html == null)
                return null;

            def._word = word_html.InnerText;

            #endregion
            #region Looking for body html

            var body_html = parsed_html.DocumentNode.SelectSingleNode(pathes["body_xpath"]);

            if (body_html == null)
                body_html = parsed_html.DocumentNode.SelectSingleNode(pathes["body"]);

            if (body_html == null)
                return null;

            #endregion
            #region Looking for part of speech html

            var part_of_speech_html = body_html.SelectSingleNode(pathes["part_of_speech_xpath"]);

            if (part_of_speech_html == null)
                part_of_speech_html = body_html.SelectSingleNode(pathes["part_of_speech"]);

            string part_of_speech = part_of_speech_html.InnerText;

            if (part_of_speech.Contains("noun") && part_of_speech.Contains("plural"))
                part_of_speech = "noun_plural_only";
            else if (part_of_speech.Contains("noun") && !part_of_speech.Contains("pronoun"))
                part_of_speech = "noun";
            else if (part_of_speech.Contains("possessive") && part_of_speech.Contains("noun"))
                part_of_speech = "possessive_pronoun";
            else if (part_of_speech.Contains("noun") && part_of_speech.Contains("pronoun"))
                part_of_speech = "pronoun";
            else if (part_of_speech.Contains("adjective"))
                part_of_speech = "adjective";
            else if (part_of_speech.Contains("verb") && !part_of_speech.Contains("adverb"))
                part_of_speech = "verb";
            else if (part_of_speech.Contains("verb") && part_of_speech.Contains("adverb"))
                part_of_speech = "adverb";
            else if (part_of_speech.Contains("preposition"))
                part_of_speech = "preposition";
            else if (part_of_speech.Contains("numeral"))
                part_of_speech = "numeral";
            else if (part_of_speech.Contains("conjunction"))
                part_of_speech = "conjunction";
            else if (part_of_speech.Contains("particle") && !part_of_speech.Contains("article"))
                part_of_speech = "particle";
            else if (part_of_speech.Contains("interjection"))
                part_of_speech = "interjection";
            else if (part_of_speech.Contains("determiner"))
                part_of_speech = "determiner";
            else if (part_of_speech.Contains("prefix"))
                part_of_speech = "prefix";

            #endregion
            #region Looking for word ending html

            if (part_of_speech_html.InnerText.Contains("noun") && !part_of_speech_html.InnerText.Contains("pronoun"))
            {
                var inner_spans_html = body_html.SelectNodes(pathes["word_ending_xpath"]);
                HtmlNode word_ending_html = null;

                if (inner_spans_html == null)
                    inner_spans_html = body_html.SelectNodes(pathes["word_ending"]);

                if (inner_spans_html != null)
                {
                    foreach (var span in inner_spans_html)
                    {
                        if (span.InnerText.Contains("plural"))
                        {
                            word_ending_html = span.SelectSingleNode(".//b[@class='inf dinf']");
                            break;
                        }
                    }
                }

                def._plural = word_ending_html == null ? "" : word_ending_html.InnerHtml;
                def._plural = part_of_speech == "noun_plural_only" ? def._word : def._plural;
            }

            #endregion
            #region Looking for translations html

            var translations_spans_html = body_html.SelectNodes(pathes["translations_xpath"]).First();

            if (translations_spans_html == null)
                translations_spans_html = body_html.SelectNodes(pathes["translations"]).First();

            var meaning = new Meaning();

            // Check to what theme this word belongs to
            var translations_theme = translations_spans_html.SelectSingleNode(".//span[@class='def-info ddef_i']"); // ".//span[@class='def-info ddef_i']//span[@class][last()]"

            if (translations_theme != null)
                meaning._theme = translations_theme.InnerText;

            // Check if this word has meaning
            var translations_meaning = translations_spans_html.SelectSingleNode(".//div[@class='def ddef_d db']");

            if (translations_meaning != null)
                meaning._meaning = translations_meaning.InnerText;

            // Check if this word has translations 
            var translations_translate = translations_spans_html.SelectNodes(".//span[@class='trans dtrans']");
            if (translations_translate != null)
            {
                foreach (var translation in translations_translate)
                {
                    if (translation.InnerText != ", ")
                        meaning._translation.Add(translation.InnerText);
                }
            }

            def._definitions = meaning;

            #endregion
            #region Looking for translations and examles (second) html

            // Looking for examples
            var examples_html = body_html.SelectNodes(pathes["examples_xpath"]);

            if (examples_html == null)
                examples_html = body_html.SelectNodes(pathes["examples"]);

            // Check if examples exist
            var examples = new List<Example>();

            if (examples_html != null)
            {
                foreach (var example_html in examples_html)
                {
                    var example = new Example();

                    // First line is always exist but the second one
                    var example_first_line = example_html.SelectSingleNode(".//span[@class='eg deg']");
                    example.first_line = example_first_line.InnerText;

                    var example_second_line = example_html.SelectSingleNode(".//span[@class='trans dtrans hdb']");

                    if (example_second_line != null)
                        example.second_line = example_second_line.InnerText;

                    examples.Add(example);
                }
            }

            def._examples = examples;

            #endregion
            #region Looking for gender html

            var gender_html = body_html.SelectSingleNode(pathes["gender_xpath"]);

            if (gender_html == null)
                gender_html = body_html.SelectSingleNode(pathes["gender"]);

            var gender_check_html = body_html.SelectSingleNode(".//div[@class='dpos-h di-head normal-entry']//span[@class='di-info']/span[@class='gram dgram']//span[@class='gc dgc']");

            if (translations_spans_html != null && gender_check_html != null)
            {
                if (genders.ContainsKey(gender_check_html.InnerText))
                    def._gender = genders[gender_check_html.InnerText];
                else
                    def._gender = gender_check_html.InnerText;
            }

            #endregion
            #region Set part of speech
            if (part_of_speech == "noun")
                part_of_speech = String.Format("{0}_{1}", part_of_speech, gender_check_html.InnerText);
            
            def._part_of_speech = part_of_speech_ToInt32_dictionary[part_of_speech];
            
            #endregion

            #region Parse verb

            if (part_of_speech == "verb")
                def._verb = ParseGermanVerb(def._word);

            #endregion
            #region Make meaning string

            if (part_of_speech.Contains("noun") && !part_of_speech.Contains("pronoun"))
            {
                var end = "";

                if (def._plural.Contains(def._word) && def._plural.Length == def._word.Length)
                    end = "==";

                if (def._plural.Contains(def._word) && def._plural.Length != def._word.Length)
                    end = "-" + def._plural.Remove(0, def._word.Length);

                def._meaning_string = String.Format("{0} ({1}, {2})", def._word, def._gender, end == "" ? def._plural : end);
                def._meaning_string = part_of_speech == "noun_plural_only" ?  String.Format("{0} ({1}, {2})", def._word, "Pl.", def._word) : def._meaning_string;
            }
            else if (part_of_speech == "verb")
            {
                def._meaning_string = String.Format("[er {0}, er {1}, er {2}]", def._verb._prasens, def._verb._prateritum, def._verb._perfekt);
            }
            else
            {
                def._meaning_string = String.Format("{0} ({1})", def._word, parts_of_speech_dictionary[part_of_speech]);
            }

            #endregion
            
            return def;
        }
        static Verb ParseGermanVerb(String word)
        {
            var full_uri = String.Format(@"https://konjugator.reverso.net/konjugation-deutsch-verb-{0}.html", word);
            var parsed_html = _html_web.Load(full_uri);

            #region Check if it possible to parse this verb
            var body_html = parsed_html.DocumentNode.SelectSingleNode(".//div[@class='wrapperW']");
            if (body_html == null)
                return null;

            var verb_forms_html = body_html.SelectNodes(".//div[@class='result-block-api']/div[@class='word-wrap-row']");
            if (verb_forms_html == null)
                return null;

            #endregion

            #region Looking for verb's forms
            var verb = new Verb();

            foreach (var verb_form in verb_forms_html)
            {
                if (verb_form.SelectSingleNode(".//div[@mobile-title='Indikativ Präsens']//p") != null)
                {
                    if ((verb_form.SelectSingleNode(".//div[@mobile-title='Indikativ Präsens']//p").InnerText == "Präsens"))
                    {
                        verb._prasens = verb_form.SelectSingleNode(".//div[@mobile-title='Indikativ Präsens']//ul//li[position()=3]").InnerText.Remove(0, 10);
                        verb._prateritum = verb_form.SelectSingleNode(".//div[@mobile-title='Indikativ Präteritum']//ul//li[position()=3]").InnerText.Remove(0, 10);
                    }
                }

                if (verb_form.SelectSingleNode(".//div[@mobile-title='Indikativ Perfekt']//p") != null)
                {
                    if (verb_form.SelectSingleNode(".//div[@mobile-title='Indikativ Perfekt']//p").InnerText == "Perfekt")
                    {
                        verb._perfekt = verb_form.SelectSingleNode(".//ul//li[position()=3]").InnerText.Remove(0, 10);
                    }
                }

            }
            #endregion

            return verb;
        }
    }
}
