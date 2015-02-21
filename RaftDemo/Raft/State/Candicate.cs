using RaftDemo.Raft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.Raft.State
{
    public class Candicate : RaftStateBase
    {
        public Candicate(RaftNode node)
            : base(node)
        {
            
        }

        int NodeCount = 4;

        public override string ToString()
        {
            return "Candidate";
        }

        public override void EnterState()
        {            
            StartNewElection();
        }
        int VoteCount = 1;
        void StartNewElection()
        {
            VoteTable.Clear();
            Node.RaftWorld.OnElectionStarted();
            // increment current term
            Node.CurrentTerm++;
            // vote for self
            VoteCount = 1;
            Node.VotedFor = Node.Id;
            // start random election timer
            Node.RaftTimer.SetRandomTimeout(2000, 6000);
            // send request votes to all servers
            Node.BroadcastMessage(new RequestVote() { CandidateId = Node.Id, CandidateTerm = Node.CurrentTerm });
        }

        

        public override void OnTimeout()
        {
            StartNewElection();
        }
        public override void ExitState()
        {

        }

        public override void ReceiveRequestVote(RequestVote requestVote, INodeChannel sourceChannel)
        {
            if (requestVote.CandidateId == Node.Id)
                return;

        
            bool voteGranted = false;
            if (requestVote.CandidateTerm >= CurrentTerm)
            {
              //  Node.CurrentTerm = requestVote.CandidateTerm;
                Node.TranslateToState(RaftNodeState.Follower);
                Node.RaisePacketReceived(requestVote, sourceChannel);
                return;
            }
            Node.SendMessage(sourceChannel, DenyVote);
        }
        Dictionary<string, bool> VoteTable = new Dictionary<string, bool>();
        public override void ReceiveRequestVoteResponse(RequestVoteResponse requestVoteResponse, INodeChannel sourceChannel)
        {      
            if(requestVoteResponse.CurrentTerm > CurrentTerm)
            {
                
                Node.TranslateToState(RaftNodeState.Follower);
                Node.RaisePacketReceived(requestVoteResponse, sourceChannel);
                return;
            }



            if (!VoteTable.ContainsKey(requestVoteResponse.VoterId))
            {
                if (requestVoteResponse.VoteGranted)                
                    VoteCount++;

                VoteTable.Add(requestVoteResponse.VoterId, requestVoteResponse.VoteGranted);
            }
            

            int majority = (NodeCount / 2) + 1;
            if (VoteCount >= majority)
            {               
                Node.TranslateToState(RaftNodeState.Leader);
            }
        }

        public override void ReceiveAppendEntries(AppendEntries appendEntries, INodeChannel sourceChannel)
        {
            if (appendEntries.LeaderTerm >= Node.CurrentTerm)
            {
                Node.TranslateToState(RaftNodeState.Follower);
                Node.RaisePacketReceived(appendEntries, sourceChannel);
            }

        }

        public override void ReceiveAppendEntriesResponse(AppendEntriesResponse appendEntriesResponse, INodeChannel sourceChannel)
        {
            Node.TranslateToState(RaftNodeState.Follower);
        }
    }
}
