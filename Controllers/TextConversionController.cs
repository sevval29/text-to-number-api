using Microsoft.AspNetCore.Mvc;
using NLPTexttoNumber.Requests;
using NLPTexttoNumber.Response;
using System.Globalization;
using System.Linq;

namespace NLPTexttoNumber.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TextConversionController : ControllerBase
    {
        [HttpPost]
        public IActionResult ConvertTextToNumber([FromBody] TextConversionRequest request)
        {
            try
            {
                string userText = request.UserText + " ";
                string convertedText = Converter.Convert(userText);

                var response = new TextConversionResponse { Output = convertedText };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        public static readonly long ZERO = 0;
        public static readonly Dictionary<string, string> ONES = new Dictionary<string, string>()
        {
            { "1" , "bir" },
            { "2" , "iki" },
            { "3" , "üç" },
            { "4" , "dört" },
            { "5" , "beş" },
            { "6" , "altı" },
            { "7" , "yedi" },
            { "8" , "sekiz" },
            { "9" , "dokuz" }
        };
        public static readonly Dictionary<string, string> TENS = new Dictionary<string, string>()
        {
            { "10" , "on" },
            { "20" , "yirmi" },
            { "30" , "otuz" },
            { "40" , "kırk" },
            { "50" , "elli" },
            { "60" , "altmış" },
            { "70" , "yetmiş" },
            { "80" , "seksen" },
            { "90" , "doksan" }
        };
        public static readonly Dictionary<string, string> TRIPLETS = new Dictionary<string, string> {
           {"100","yüz" },
           {"1000", "bin" },
           {"1000000", "milyon"}
        };
       

        public static readonly List<string> WORD_DICTIONARY = new List<string>()
        {
            "bir"       ,
            "iki"       ,
            "üç"        ,
            "dört"      ,
            "beş"       ,
            "altı"      ,
            "yedi"      ,
            "sekiz"     ,
            "dokuz"     ,
            "on"        ,
            "yirmi"     ,
            "otuz"      ,
            "kırk"      ,
            "elli"      ,
            "altmış"    ,
            "yetmiş"    ,
            "seksen"    ,
            "doksan"    ,
            "yüz"       ,
            "bin"       ,
            "milyon"
        };

        public static readonly List<string> WORD_DICTIONARY_ENG = new List<string>()
{
    "one"       ,
    "two"       ,
    "three"     ,
    "four"      ,
    "five"      ,
    "six"       ,
    "seven"     ,
    "eight"     ,
    "nine"      ,
    "ten"       ,
    "twenty"    ,
    "thirty"    ,
    "forty"     ,
    "fifty"     ,
    "sixty"     ,
    "seventy"   ,
    "eighty"    ,
    "ninety"    ,
    "one hundred",
    "one thousand",
    "one million"
};

        public static readonly List<Dictionary<string, string>> COMBINED_DICTIONARY = new List<Dictionary<string, string>>() { TRIPLETS, TENS, ONES };


       


        public static class Converter
        {

            private static string[] PreProcess(string input, int startIndex = 0, int length = 2)
            {
                ConvertDigitToText(ref input);

                var words = input.Split();

                foreach (var word in words)
                {
                    foreach (var eng_word in WORD_DICTIONARY_ENG)
                    {

                        if (word.Contains(eng_word))
                        {
                            var result = englishtonumber.ConverterEng.Convert(word);
                            
                            ConvertDigitToText(ref result);
                            input = input.Replace(word, result);
                            break;
                          
                        }
                    }
                }
                var wordss = input.Split();



                foreach (var word in wordss)
                {

                    var splittedMultiToken = TrySplitMultiTokens(word);
                    if (splittedMultiToken != "")
                    {
                        input = input.Replace(splittedMultiToken.Replace(" ", ""), splittedMultiToken);
                    }
                }

                return input.Split();
            }

           
            //Hepsi aynı formatta olsun diye inputtaki sayıları metinsel ifadelere dönüştürür.
            private static void ConvertDigitToText(ref string input)
            {
                foreach (var dict in COMBINED_DICTIONARY)
                {
                    foreach (var pair in dict)
                    {
                        var num = pair.Key;
                        if (input.Contains(num))
                            input = input.Replace(num, pair.Value);
                    }
                }
            }



            // Bu metot, belirli bir kelimenin içinde geçen çoklu kelimeleri ayırmak için kullanılır
            private static string TrySplitMultiTokens(string word)
            {
                int start = 0;
                int length = 1;

                var str = "";

                while (start + length <= word.Length)
                {
                    var subword = word.Substring(start, length);
                    foreach (var w in WORD_DICTIONARY)
                    {
                        if (w.ToLower(new CultureInfo("TR-tr")) == subword.ToLower(new CultureInfo("TR-tr")))
                        {
                            str += subword + " ";
                            start += length;
                            length = 1;
                           break;
                        }

                    }
                    length++;
                }

                return str.TrimEnd();

            }

            public static string Convert(string input)
            {
                long sum = 0;
                var tokens = PreProcess(input);

                List<long> results = new List<long>();
                string resultString = "";
                for (int i = 0; i < tokens.Length; i++)
                {

                    var token = tokens[i];


                    var result = SearchAndConvertToNumber(token);
                    if (result != ZERO)
                    {
                        results.Add(result);
                        

                    }

                    else if (result == ZERO && results.Count > 0)
                    {
                        var converted = GenerateNumberAsString(results);
                        resultString += converted  + " " + token + " ";
                        

                        results = new List<long>();
                    }
                    else
                    {
                        resultString += token + " ";
                    }
                }

                return resultString.TrimEnd();
            }



            //Bu metodun amacı, bir liste içindeki sayıları birleştirmek

            private static string GenerateNumberAsString(List<long> numbers)
            {

                long total = 0;
                long prior = -1;

                foreach (var number in numbers)
                {
                    if (prior == -1)
                        prior = number;
                    else if (prior > number)
                        prior += number;
                    else
                        prior *= number;


                    if (number >= 1000)
                    {
                        total += prior;
                        prior = -1;
                    }

                }

                return (prior == -1 ? total : prior + total).ToString();

            }


            //Bu metod bir kelimenin sayıya dönüştürülmesini gerçekleştirir.

            private static long SearchAndConvertToNumber(string token)
            {
                token = token.ToLower(new CultureInfo("tr-TR"));
              

                for (int i = 0; i < COMBINED_DICTIONARY.Count; i++)
                {
                    foreach (var pair in COMBINED_DICTIONARY[i])
                    {
                        if (pair.Value == token)
                            return long.Parse(pair.Key);
                    }
                }

                return ZERO;

            }

        
            }
           
        }

    
}