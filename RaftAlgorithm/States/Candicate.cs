using RaftAlgorithm.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftAlgorithm.States
{
    public class Candicate<T> : RaftStateBase<T>
    {
        public Candicate(RaftNode<T> node)
            : base(node)
        {
            
        }

        public override string ToString()
        {
            return "Candidate";
        }

        public override RaftEventResult EnterState()
        {            
           return StartNewElection();
        }

        RaftEventResult StartNewElection()
        {
            VoteTable.Clear();
            Node.RaftEventListener.OnElectionStarted();
            // increment current term
            Node.CurrentTerm++;
            // vote for self
            ProcessVote(Node.Id);
            Node.VotedFor = Node.Id;
            // send request votes to all servers
            var requestVote = new RequestVote() { CandidateId = Node.Id, CandidateTerm = Node.CurrentTerm };
            return RaftEventResult.BroadcastMessage(requestVote).SetTimer(Node.RaftSettings.FollowerTimeoutFrom, Node.RaftSettings.FollowerTimeoutTo);
        }



        public override RaftEventResult OnTimeout()
        {
            return StartNewElection();
        }

        public override RaftEventResult ReceiveRequestVote(RequestVote requestVote)
        {
            if (requestVote.CandidateId == Node.Id)
                return RaftEventResult.Empty;

        
            bool voteGranted = false;
            if (requestVote.CandidateTerm >= CurrentTerm)
            {
              //  Node.CurrentTerm = requestVote.CandidateTerm;
                return Node.TranslateToState(RaftNodeState.Follower, requestVote);
            }
            return RaftEventResult.ReplyMessage(DenyVote);
        }
        Dictionary<string, bool> VoteTable = new Dictionary<string, bool>();

        void ProcessVote(string voterId)
        {
            if (!VoteTable.ContainsKey(voterId))
                VoteTable.Add(voterId, true);
        }

        public override RaftEventResult ReceiveRequestVoteResponse(RequestVoteResponse requestVoteResponse)
        {      
            if(requestVoteResponse.CurrentTerm > CurrentTerm)
            {                
                return Node.TranslateToState(RaftNodeState.Follower, requestVoteResponse);
            }

            if (requestVoteResponse.VoteGranted)
            {
                ProcessVote(requestVoteResponse.VoterId);
            }
            
            int majority = (Node.RaftSettings.ClusterSize / 2) + 1;
            if (VoteTable.Count >= majority)
            {               
                return Node.TranslateToState(RaftNodeState.Leader);
            }
            return RaftEventResult.Empty;
        }

        public override RaftEventResult ReceiveAppendEntries(AppendEntries appendEntries)
        {
            if (appendEntries.LeaderTerm >= Node.CurrentTerm)
            {
                return Node.TranslateToState(RaftNodeState.Follower, appendEntries );
            }
            return RaftEventResult.Empty;
        }

        public override RaftEventResult ReceiveAppendEntriesResponse(AppendEntriesResponse appendEntriesResponse)
        {
            return Node.TranslateToState(RaftNodeState.Follower);
        }
    }
}
