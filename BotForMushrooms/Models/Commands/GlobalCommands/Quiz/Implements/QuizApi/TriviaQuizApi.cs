using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Web;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements.QuizApi
{
    public class TriviaQuizApi
    {
        public const string BaseApiUrl = "https://opentdb.com";

        public HashSet<QuizAnswerTypeEnum> AnswerTypes { get; set; }

        public HashSet<QuizDifficultyEnum> Difficulty { get; set; }

        public HashSet<QuizThemeEnum> Theme { get; set; } 

        private enum TriviaApiThemesEnum
        {
            Any = 0,
            GeneralKnowlage = 9,
            EntertainmentBooks,
            EntertainmentFilm,
            EntertainmentMusic,
            EntertainmentMusicalAndTheatres,
            EntertainmentTV,
            EntertainmentVideoGames,
            EntertainmentBoardGames,
            ScienceAndNature,
            ScienceComputers,
            ScienceMath,
            Mythology,
            Sport,
            Geography,
            History,
            Politics,
            Art,
            Celebrities,
            Animals,
            Vehicles,
            EntertainmentComics,
            ScienceGadgets,
            EntertainmentJapansAnimeAndManga,
            EntertainmentCartoonAndAnimations
        }

        private List<TriviaApiThemesEnum> ScienceThemes { get; }

        private List<TriviaApiThemesEnum> EntertainmentThemes { get; }

        private List<TriviaApiThemesEnum> GeneralKnowlageThemes { get; }


        public TriviaQuizApi() 
        {
            AnswerTypes = [
                QuizAnswerTypeEnum.Multiple,
                QuizAnswerTypeEnum.AbsoluteAnswer,
                QuizAnswerTypeEnum.YesOrNot,
                QuizAnswerTypeEnum.All
                ];

            Difficulty = [
                QuizDifficultyEnum.Easy,
                QuizDifficultyEnum.Medium,
                QuizDifficultyEnum.Hard,
                QuizDifficultyEnum.All,
                ];

            Theme = [
                QuizThemeEnum.AllTheme,
                QuizThemeEnum.Science,
                QuizThemeEnum.Entertaiment,
                QuizThemeEnum.GeneralKnowlage,
                QuizThemeEnum.VideoGames,
                ];

            ScienceThemes = [
                TriviaApiThemesEnum.ScienceComputers,
                TriviaApiThemesEnum.ScienceAndNature,
                TriviaApiThemesEnum.ScienceGadgets,
                TriviaApiThemesEnum.ScienceMath,
                TriviaApiThemesEnum.History,
                TriviaApiThemesEnum.Geography,
                TriviaApiThemesEnum.Mythology,
                TriviaApiThemesEnum.Animals,
                TriviaApiThemesEnum.Art
                ];

            EntertainmentThemes = [
                TriviaApiThemesEnum.EntertainmentBoardGames,
                TriviaApiThemesEnum.EntertainmentFilm,
                TriviaApiThemesEnum.EntertainmentVideoGames,
                TriviaApiThemesEnum.EntertainmentCartoonAndAnimations,
                TriviaApiThemesEnum.EntertainmentMusic,
                TriviaApiThemesEnum.EntertainmentBooks
                ];

            GeneralKnowlageThemes = [
                TriviaApiThemesEnum.Vehicles,
                TriviaApiThemesEnum.Celebrities,
                TriviaApiThemesEnum.GeneralKnowlage,
                TriviaApiThemesEnum.Politics
                ];
        }

        public async Task<QuizQuestion?> GetQuestion(QuizSettings quizSettings)
        {
            List<TriviaApiThemesEnum> currentThemes = quizSettings.Theme switch
            {
                QuizThemeEnum.Science => ScienceThemes,
                QuizThemeEnum.GeneralKnowlage => GeneralKnowlageThemes,
                QuizThemeEnum.Entertaiment => EntertainmentThemes,
                QuizThemeEnum.VideoGames => [TriviaApiThemesEnum.EntertainmentVideoGames],
                _ => ScienceThemes.Concat(GeneralKnowlageThemes).Concat(EntertainmentThemes).ToList()
            };
            
            Random rand = new Random();
            TriviaApiThemesEnum currentThem = currentThemes[rand.Next(currentThemes.Count)];

            using HttpClient client = new HttpClient();

            var uriBuilder = new UriBuilder(BaseApiUrl + "/api.php");

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["amount"] = "1";
            query["category"] = ((int)currentThem).ToString();

            if(quizSettings.Difficulty != QuizDifficultyEnum.All)
            {
                query["difficulty"] = quizSettings.Difficulty.ToString().ToLower();
            }

            if(quizSettings.AnswerType != QuizAnswerTypeEnum.All)
            {
                query["type"] = "multiple";
            }

            uriBuilder.Query = query.ToString();

            string url = uriBuilder.ToString();

            int responseAmount = 10;
            HttpResponseMessage? response = null;
            while (responseAmount != 0)
            {
                try
                {
                    response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    break;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"TriviaApi Request Error: {ex.Message}");
                    await Task.Delay(5000);
                    responseAmount--;
                }
            }

            if(responseAmount == 0 || response == null)
            {
                return null;
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            JObject jsonObject = JObject.Parse(responseBody);

            JArray? results = (JArray?)jsonObject["results"];
            if(results == null)
            {
                return null;
            }

            JObject firstResult = (JObject)results[0];

            string? type = (string?)firstResult["type"];
            string? difficulty = (string?)firstResult["difficulty"];
            string? category = (string?)firstResult["category"];
            string? question = (string?)firstResult["question"];
            string? correctAnswer = (string?)firstResult["correct_answer"];
            JArray? incorrectAnswers = (JArray?)firstResult["incorrect_answers"];

            if(type == null || difficulty == null || category == null
                || question == null || correctAnswer == null || incorrectAnswers == null)
            {
                return null;
            }

            List<string>? incorrectAnswersList = incorrectAnswers.ToObject<List<string>>();
            if(incorrectAnswersList == null)
            {
                return null;
            }

            question = ParseQuestion(question);
            correctAnswer = ParseQuestion(correctAnswer);
            for (int i = 0; i < incorrectAnswersList.Count; i++)
            {
                incorrectAnswersList[i] = ParseQuestion(incorrectAnswersList[i]);
            }

            return new QuizQuestion(question, quizSettings.AnswerType.Value, correctAnswer, incorrectAnswersList) { Theme = currentThem.ToString() };
        }

        private string ParseQuestion(string question)
        {
            question = question.Replace("&quot;", "\"");
            question = question.Replace("&#039;", "\'");
            question = question.Replace("&eacute;", "é");
            return question;
        }
    }
}
