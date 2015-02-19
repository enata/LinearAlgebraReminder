using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace PoliticsSample
{
    class Program
    {
        private const string VotingsFileName = "votingSearchResults.xml";
        private const string VotingsInfoFileName = "votingsInfo.xml";

        static void Main()
        {
            
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
            var votings = DataLoader.UpdateVotingSearchData(3).Result;
            var xsSubmit = new XmlSerializer(typeof(VotingSearch));
            using (var sww = new StreamWriter(VotingsFileName))
            {
                var writer = XmlWriter.Create(sww);
                xsSubmit.Serialize(writer, votings);
            }
        }
    }
}
