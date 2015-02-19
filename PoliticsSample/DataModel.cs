using System.Collections.Generic;

namespace PoliticsSample
{
    public sealed class ResultsByFaction
    {
        public string Code { get; set; }
        public int Total { get; set; }
        public int For { get; set; }
        public int Against { get; set; }
        public int Abstain { get; set; }
        public int Absent { get; set; }
        public string Name { get; set; }
        public string Abbr { get; set; }
    }

    public sealed class ResultsByDeputy
    {
        public string Code { get; set; }
        public string Result { get; set; }
        public string FactionCode { get; set; }
        public string Family { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
    }

    public sealed class VotingResults
    {
        public string Date { get; set; }
        public string LawNumber { get; set; }
        public string Subject { get; set; }
        public bool Resolution { get; set; }
        public int For { get; set; }
        public int Against { get; set; }
        public int Abstain { get; set; }
        public int Absent { get; set; }
        public string TranscriptLink { get; set; }
        public List<ResultsByFaction> ResultsByFaction { get; set; }
        public List<ResultsByDeputy> ResultsByDeputy { get; set; }
    }

    public sealed class Votes
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string VoteDate { get; set; }
        public int VoteCount { get; set; }
        public int ForCount { get; set; }
        public int AgainstCount { get; set; }
        public int AbstainCount { get; set; }
        public int AbsentCount { get; set; }
        public string ResultType { get; set; }
        public bool Result { get; set; }
    }

    public sealed class VotingSearch
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Wording { get; set; }
        public List<Votes> Votes { get; set; }
    }

}
