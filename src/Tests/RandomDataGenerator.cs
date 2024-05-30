using Bogus;
using Common.Enums;
using Domain.Entities;

namespace Tests;

public class RandomDataGenerator
{
    public IEnumerable<User> GeneratedUserData { get; private set; } = new List<User>();
    public IEnumerable<Comment> GeneratedCommentData { get; private set; } = new List<Comment>();
    public IEnumerable<Topic> GeneratedTopicData { get; private set; } = new List<Topic>();
    public IEnumerable<Ban> GeneratedBanData { get; private set; } = new List<Ban>();
    
    public void GenerateAllStaticBogusData()
    {
        GenerateUserStaticBogusData();
        GenerateTopicStaticBogusData();
        GenerateCommentStaticBogusData();
        GenerateBanStaticBogusData();
    }
    
    private static Faker<User> GetUserGenerator() =>
        new Faker<User>()
            .RuleFor(u => u.Id, f => f.Random.Guid().ToString())
            .RuleFor(u => u.RefreshToken, f => f.Random.Guid().ToString()[..10])
            .RuleFor(u => u.ProfilePicUrl, f => f.Internet.Avatar())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.Comments, (f, e) => GetBogusCommentData(e.Id, f.Random.UInt(100)))
            .RuleFor(u => u.Topics, (_, e) => GetBogusTopicData(e.Id))
            .RuleFor(u => u.IsAdmin, _ => false)
            .RuleFor(u => u.BanInfo, _ => null)
            .RuleFor(u => u.IsBanned, _ => false);

    private static Faker<Comment> GetCommentGenerator(string userId, uint topicId) =>
        new Faker<Comment>()
            .RuleFor(c => c.Id, f => (uint)f.IndexGlobal)
            .RuleFor(c => c.CreationDate, f => f.Date.Past())
            .RuleFor(c => c.Content, f => f.Rant.Review())
            .RuleFor(c => c.AuthorId, _ => userId)
            .RuleFor(c => c.TopicId, _ => topicId);
    
    private static Faker<Topic> GetTopicGenerator(string userId) =>
        new Faker<Topic>()
            .RuleFor(t => t.Id, f => (uint)f.IndexGlobal)
            .RuleFor(t => t.CreationDate, f => f.Date.Past())
            .RuleFor(t => t.Title, f => f.Rant.Review().Split(' ').Take(5).Aggregate((a, b) => $"{a} {b}") + "...")
            .RuleFor(t => t.Content, f => f.Rant.Review(f.Vehicle.Manufacturer()))
            .RuleFor(t => t.BackgroundImageUrl, f => f.Image.PicsumUrl())
            .RuleFor(t => t.AuthorId, _ => userId)
            .RuleFor(t => t.CommentCount, f => (uint)f.Random.Int(0,8))
            .RuleFor(t => t.State, _ => State.Show)
            .RuleFor(t => t.Status, _ => Status.Active);
    
    private static Faker<Ban> GetBanGenerator(string userId) =>
        new Faker<Ban>()
            .RuleFor(b => b.Id, f => (uint)f.IndexGlobal)
            .RuleFor(b => b.BanEndDate, f => f.Date.Future())
            .RuleFor(b => b.Reason, f => f.Lorem.Sentence())
            .RuleFor(b => b.UserId, _ => userId);

    private static IEnumerable<Topic> GetBogusTopicData(string userId, int count = 1) =>
        GetTopicGenerator(userId).Generate(count);
    
    private static IEnumerable<Comment> GetBogusCommentData(string userId, uint topicId, int count = 2) =>
        GetCommentGenerator(userId, topicId).Generate(count);
    
    private static Ban GetBogusBanData(string userId) =>
        GetBanGenerator(userId).Generate();

    private static IEnumerable<User> GetBogusUserData(int count = 10) =>
        GetUserGenerator().Generate(count);

    private void GenerateUserStaticBogusData()
    {
        if(GeneratedUserData.Any()) return;
        GeneratedUserData = GetBogusUserData().ToList();
    }

    private void GenerateTopicStaticBogusData()
    {
        if(GeneratedTopicData.Any()) return;
        GeneratedTopicData = GeneratedUserData.SelectMany(u => GetBogusTopicData(u.Id)).ToList();
        foreach (var user in GeneratedUserData) user.Topics = GeneratedTopicData.Where(t => t.AuthorId == user.Id).ToList();
    }

    private void GenerateCommentStaticBogusData()
    {
        if(GeneratedCommentData.Any()) return;
        var comments = new List<Comment>();
        foreach (var topic in GeneratedTopicData)
        {
            var commentCount = topic.CommentCount;
            foreach (var user in GeneratedUserData)
            {
                if (topic.AuthorId == user.Id) continue;
                if (commentCount-- <= 0) break;
                comments.AddRange(GetBogusCommentData(user.Id, topic.Id));
            }
        }
        GeneratedCommentData = comments;
        foreach (var user in GeneratedUserData) user.Comments = GeneratedCommentData.Where(c => c.AuthorId == user.Id).ToList();
        foreach (var topic in GeneratedTopicData) topic.Comments = GeneratedCommentData.Where(c => c.TopicId == topic.Id).ToList();
        foreach (var comment in GeneratedCommentData) comment.Topic = GeneratedTopicData.First(t => t.Id == comment.TopicId);
    }

    private void GenerateBanStaticBogusData()
    {
        if(GeneratedBanData.Any()) return;
        var bans = new List<Ban>();
        var count = 3;
        foreach (var user in GeneratedUserData)
        {
            if (count-- <= 0) break;
            
            var ban = GetBogusBanData(user.Id);
            
            bans.Add(ban);
            
            user.IsBanned = true;
            ban.BannedUser = user;
            user.BanInfo = ban;
        }
        GeneratedBanData = bans;
    }
}