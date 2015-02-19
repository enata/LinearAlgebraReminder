using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PoliticsSample
{
    static class DataLoader
    {
        private const string QueryBase = "http://api.duma.gov.ru/api/{0}/";
        private const string Token = "cecbd492194a52758ddf0bc04845347282e5ed99";
        private const string AppToken = "app94c5ae80b831fb1f476105e8266377383f1c2050";
        private const string DateFormat = "yyyy-MM-dd";
        private const int EntriesPerPage = 100;
        private const string VoteSearchQuery = "voteSearch.json?app_token={2}&from={0}&to={1}&limit=100&page={3}";
        private const string VoteInfoQuery = "vote/{0}.json?app_token={1}";

        public static async Task<List<VotingResults>> GetVotingsInfo(IEnumerable<string> ids)
        {
            var result = new List<VotingResults>();
            using (var client = CreateClient())
            {
                foreach (var id in ids)
                {
                    var query = String.Format(VoteInfoQuery, id, AppToken);
                    HttpResponseMessage response = await client.GetAsync(query);
                    if (response.IsSuccessStatusCode)
                    {
                        VotingResults votingInfo = await response.Content.ReadAsAsync<VotingResults>();
                        result.Add(votingInfo);
                    }
                }
            }
            return result;
        }

        private static HttpClient CreateClient()
        {
            var client = new HttpClient {BaseAddress = new Uri(String.Format(QueryBase, Token))};
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public static async Task<VotingSearch> UpdateVotingSearchData(int historyLength)
        {
            var searchResults = await DownloadVoteSearchDataAsync(historyLength);
            return CombineVotingSearches(searchResults);
        }

        private static VotingSearch CombineVotingSearches(IEnumerable<VotingSearch> votingSearches)
        {
            var result = new VotingSearch();
            var enumeratedSearches = votingSearches as IList<VotingSearch> ?? votingSearches.ToList();
            if (!enumeratedSearches.Any())
                return result;
            result.TotalCount = enumeratedSearches.First().TotalCount;
            result.Wording = enumeratedSearches.First().Wording;
            result.Votes = enumeratedSearches.Select(search => search.Votes).SelectMany(vote => vote).ToList();
            return result;
        }

        private static async Task<List<VotingSearch>> DownloadVoteSearchDataAsync(int historyLength)
        {
            var votesSearchResults = new List<VotingSearch>();
            using (var client = CreateClient())
            {

                DateTime toDate = DateTime.Now.Date;
                DateTime fromDate = toDate.AddMonths(-historyLength);
                int page = 0;
                int totalCount = 0;                
                do
                {
                    page += 1;
                    var votings = await GetVotingPage(fromDate, toDate, client, page);
                    if (votings != null)
                    {
                        totalCount = votings.TotalCount;
                        votesSearchResults.Add(votings);
                    }
                } while (totalCount - page*EntriesPerPage > 0);
            }
            return votesSearchResults;
        }

        private static async Task<VotingSearch> GetVotingPage(DateTime fromDate, DateTime toDate, HttpClient client, int page)
        {
            string query = String.Format(VoteSearchQuery, fromDate.ToString(DateFormat),
                toDate.ToString(DateFormat), AppToken, page);
            HttpResponseMessage response = await client.GetAsync(query);
            if (response.IsSuccessStatusCode)
            {
                VotingSearch votings = await response.Content.ReadAsAsync<VotingSearch>();
                return votings;
            }
            return null;
        }
    }
}