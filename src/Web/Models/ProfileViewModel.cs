using Common.DTOs.Ban.Request;
using Common.DTOs.Comment.Response;
using Common.DTOs.Topic.Response;
using Common.DTOs.User.Response;

namespace Web.Models;

public record ProfileViewModel(
    UserResponseModel User,
    IEnumerable<CommentResponseModel> Comments,
    IEnumerable<TopicResponseModel> Topics,
    BanCreateModel BanModel,
    bool IsCurrentUser = false);