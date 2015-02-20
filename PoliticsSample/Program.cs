using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using LinearAlgebraReminder;

namespace PoliticsSample
{
    class Program
    {
        private const string VotingsFileName = "votingSearchResults.xml";
        private const string VotingsInfoFileName = "votingsInfo.xml";

        static void Main()
        {
            
        }

        private static Dictionary<string, IVector> GetVotingDictionary(IEnumerable<VotingResults> votings)
        {
            if(votings == null)
                throw new ArgumentNullException("votings");

            var result = new Dictionary<string, IVector>();
            var enumeratedVotings = votings as IList<VotingResults> ?? votings.ToList();
            if (!enumeratedVotings.Any())
                return result;

            var deputatsVotingsLists = enumeratedVotings.First()
                .ResultsByDeputy.ToDictionary(voting => voting.Code, voting => new List<int>(enumeratedVotings.Count));
            var votingDictionaries = enumeratedVotings.Select(BuildVotingDictionary);
            foreach (var votingResult in votingDictionaries)
            {
                foreach (var key in deputatsVotingsLists.Keys)
                {
                    int deputatsVote = 0;
                    if (votingResult.ContainsKey(key))
                        deputatsVote = votingResult[key];
                    deputatsVotingsLists[key].Add(deputatsVote);
                }
            }
            return result.ToDictionary(pair => pair.Key, pair => (IVector) new Vector(pair.Value));
        }

        private static Dictionary<string, int> BuildVotingDictionary(VotingResults voting)
        {
            var result = voting.ResultsByDeputy.ToDictionary(v => v.Code, v =>
            {
                int decision = 0;
                if (v.Result == "for")
                    decision = 1;
                else if (v.Result == "against")
                    decision = -1;
                return decision;
            });
            return result;
        }

        private static void UpdateAll(int period, int votingsLimit)
        {
            UpdateVotingData(period);
            VotingSearch votingSearchResult;

            XmlSerializer serializer = new XmlSerializer(typeof(VotingSearch));

            using (StreamReader reader = new StreamReader(VotingsFileName))
            {
                votingSearchResult = (VotingSearch)serializer.Deserialize(reader);
            }

            var mostControversal = SearchControversalVotings(votingSearchResult, votingsLimit);
            UpdateVotingInfoData(mostControversal);
        }

        private static List<string> SearchControversalVotings(VotingSearch votingSearchResults, int count)
        {
            var result =
                votingSearchResults.Votes.OrderByDescending(vote => Math.Min(vote.ForCount, vote.AgainstCount))
                    .Take(count)
                    .Select(vote => vote.Id)
                    .ToList();
            return result;
        }

        private static void UpdateVotingInfoData(IEnumerable<string> ids)
        {
            var votingInfo = DataLoader.GetVotingsInfo(ids).Result;
            var xsSubmit = new XmlSerializer(typeof(List<VotingResults>));
            using (var sww = new StreamWriter(VotingsInfoFileName))
            {
                var writer = XmlWriter.Create(sww);
                xsSubmit.Serialize(writer, votingInfo);
            }
        }

        private static void UpdateVotingData(int period)
        {
            var votings = DataLoader.UpdateVotingSearchData(period).Result;
            var xsSubmit = new XmlSerializer(typeof(VotingSearch));
            using (var sww = new StreamWriter(VotingsFileName))
            {
                var writer = XmlWriter.Create(sww);
                xsSubmit.Serialize(writer, votings);
            }
        }
    }
}
