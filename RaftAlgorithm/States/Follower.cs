using RaftAlgorithm.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftAlgorithm.States
{
    public class Follower<T> : RaftStateBase<T>
    {
        public Follower(RaftNode<T> node)
            : base(node)
        {
            
        }
        public override RaftNodeState State
        {
            get { return RaftNodeState.Follower; }
        }
        public override string ToString()
        {
            return "Follower";            
        }
        static Random rnd = new Random();
        public override RaftEventResult EnterState()
        {
            // Start new election(translate to candidate) of no sign of leader for random time specified here
            return RaftEventResult.Empty.SetTimer(Node.RaftSettings.FollowerTimeoutFrom, Node.RaftSettings.FollowerTimeoutTo);
        }

        public override RaftEventResult OnTimeout()
        {
            // translate to candidate and start new election
            return Node.TranslateToState(RaftNodeState.Candidate);
        }

        public override RaftEventResult ReceiveRequestVote(RequestVote requestVote)
        {
            // we have more recent term so we dont vote
            if(requestVote.CandidateTerm < CurrentTerm)
            {
                return RaftEventResult.ReplyMessage(DenyVote);
            }
 
            if(requestVote.CandidateTerm > CurrentTerm)
            {
                CurrentTerm = requestVote.CandidateTerm;
            }
            // if haven't voted before
            if (Node.PersistedState.VotedFor == null || Node.PersistedState.VotedFor == requestVote.CandidateId)
            {
                Node.PersistedState.VotedFor = requestVote.CandidateId;
                return RaftEventResult.ReplyMessage(GrantVote).SetTimer(Node.RaftSettings.FollowerTimeoutFrom * 2, Node.RaftSettings.FollowerTimeoutTo * 2);
            }
            return RaftEventResult.Empty;
        }

        public override RaftEventResult ReceiveRequestVoteResponse(RequestVoteResponse requestVoteResponse)
        {
            return RaftEventResult.Empty;
        }

        public override RaftEventResult ReceiveAppendEntries(AppendEntriesRPC<T> appendEntries)
        {
            //Reply false if term from append entires < currentTerm (§5.1)
            if (appendEntries.LeaderTerm < CurrentTerm)
            {
                var falseResponse = new AppendEntriesResponse(CurrentTerm, Node.Id, false);
                return RaftEventResult.ReplyMessage(falseResponse).SetTimer(Node.RaftSettings.FollowerTimeoutFrom, Node.RaftSettings.FollowerTimeoutTo);
            }
           
            CurrentTerm = appendEntries.LeaderTerm;

            //if (appendEntries.LogEntries != null)
            //    for (int i = 0; i < appendEntries.LogEntries.Count; i++)
            //    {
            //        if (appendEntries.LogEntries[i].CommitIndex > Node.Log.Count)
            //        {
            //            Node.Log.Add(appendEntries.LogEntries[i]);
            //            Node.CurrentIndex = appendEntries.LogEntries[i].CommitIndex;
            //        }
            //    }

            var aeResponse = new AppendEntriesResponse(CurrentTerm, Node.Id, true );
            return RaftEventResult.ReplyMessage(aeResponse).SetTimer(Node.RaftSettings.FollowerTimeoutFrom, Node.RaftSettings.FollowerTimeoutTo);
        }

        public override RaftEventResult ReceiveAppendEntriesResponse(AppendEntriesResponse appendEntriesResponse)
        {
            return RaftEventResult.Empty;
        }
    }
}
