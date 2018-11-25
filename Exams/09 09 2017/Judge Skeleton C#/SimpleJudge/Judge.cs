using System;
using System.Collections.Generic;
using Wintellect.PowerCollections;
using System.Linq;

public class Judge : IJudge
{
    private OrderedSet<int> users;
    private OrderedSet<int> contests;
    private OrderedDictionary<int, Submission> submissions;
    private Dictionary<SubmissionType, Dictionary<int, int>> contestsByType;
    private OrderedDictionary<int, List<Submission>> orderedByPoints;

    public Judge()
    {
        this.users = new OrderedSet<int>();
        this.contests = new OrderedSet<int>();
        this.submissions = new OrderedDictionary<int, Submission>();
        this.contestsByType =new Dictionary<SubmissionType, Dictionary<int, int>>();
        this.orderedByPoints = new OrderedDictionary<int, List<Submission>>();
    }

    public void AddContest(int contestId)
    {
        this.contests.Add(contestId);
    }

    public void AddSubmission(Submission submission)
    {
        if (!this.contests.Contains(submission.ContestId) || !this.users.Contains(submission.UserId))
        {
            throw new InvalidOperationException();
        }
        if (this.submissions.ContainsKey(submission.Id))
        {
            return;
        }
        this.submissions[submission.Id] = submission;

        if (!this.contestsByType.ContainsKey(submission.Type))
        {
            this.contestsByType[submission.Type] = new Dictionary<int, int>();
        }

        if (!this.contestsByType[submission.Type].ContainsKey(submission.ContestId))
        {
            this.contestsByType[submission.Type][submission.ContestId] = 0;
        }
        this.contestsByType[submission.Type][submission.ContestId]++;
        if (!this.orderedByPoints.ContainsKey(submission.Points))
        {
            this.orderedByPoints[submission.Points] = new List<Submission>();
        }
        this.orderedByPoints[submission.Points].Add(submission);
    }

    public void AddUser(int userId)
    {
        this.users.Add(userId);
    }

    public void DeleteSubmission(int submissionId)
    {
        if (!this.submissions.ContainsKey(submissionId))
        {
            throw new InvalidOperationException();
        }
        this.contestsByType[this.submissions[submissionId].Type][this.submissions[submissionId].ContestId]--;
        this.orderedByPoints[this.submissions[submissionId].Points].Remove(this.submissions[submissionId]);
        this.submissions.Remove(submissionId);
    }

    public IEnumerable<Submission> GetSubmissions()
    {
        return this.submissions.Values;
    }

    public IEnumerable<int> GetUsers()
    {
        return this.users;
    }

    public IEnumerable<int> GetContests()
    {
        return this.contests;
    }

    public IEnumerable<Submission> SubmissionsWithPointsInRangeBySubmissionType(int minPoints, int maxPoints, SubmissionType submissionType)
    {
        return this.orderedByPoints.Range(minPoints, true, maxPoints, true).SelectMany(x=>x.Value).Where(x=>x.Type==submissionType);
    }

    public IEnumerable<int> ContestsByUserIdOrderedByPointsDescThenBySubmissionId(int userId)
    {
        return this.submissions.Values.Where(x=>x.UserId==userId).OrderByDescending(x=>x.Points).ThenBy(x=>x.Id).Select(x=>x.ContestId).Distinct();
    }

    public IEnumerable<Submission> SubmissionsInContestIdByUserIdWithPoints(int points, int contestId, int userId)
    {
        if (!this.orderedByPoints.ContainsKey(points) || !this.contests.Contains(contestId) || !this.users.Contains(userId))
        {
            throw new InvalidOperationException();
        }
        return this.orderedByPoints[points].Where(x => x.UserId == userId && x.ContestId == contestId);
    }

    public IEnumerable<int> ContestsBySubmissionType(SubmissionType submissionType)
    {
        if (!this.contestsByType.ContainsKey(submissionType))
        {
            return new List<int>();
        }
        return this.contestsByType[submissionType].Keys;
        
    }
}
