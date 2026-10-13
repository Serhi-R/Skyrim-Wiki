using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkyrimWIKI.Models;
using SkyrimWIKI.Models.Api;
using SkyrimWIKI.Repositories;

namespace SkyrimWIKI.Controllers;

// JSON endpoints behind the star rating and comment widget:
// GET    /api/comments?questId=1  - list and average
// POST   /api/comments            - add one (signed in users)
// DELETE /api/comments/{id}       - remove own comment
[ApiController]
[Route("api/comments")]
public class CommentsController : ControllerBase
{
    private readonly IRepository<Comment> _comments;
    private readonly IRepository<Quest> _quests;
    private readonly UserManager<ApplicationUser> _users;

    public CommentsController(
        IRepository<Comment> comments,
        IRepository<Quest> quests,
        UserManager<ApplicationUser> users)
    {
        _comments = comments;
        _quests = quests;
        _users = users;
    }

    [HttpGet]
    public IActionResult GetByQuest(int questId)
    {
        if (_quests.GetById(questId) is null)
        {
            return NotFound(new { message = "Quest not found." });
        }

        // Sorted newest first; the average is computed from the same list.
        var comments = _comments.Find(c => c.QuestId == questId)
            .OrderByDescending(c => c.CreatedAt)
            .ToList();

        var currentUserId = _users.GetUserId(User);

        var response = new CommentsResponse
        {
            QuestId = questId,
            Count = comments.Count,
            AverageRating = comments.Count == 0
                ? 0
                : Math.Round(comments.Average(c => c.Rating), 1),
            Items = comments.Select(c => ToDto(c, currentUserId)).ToList()
        };

        return Ok(response);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CommentInput input)
    {
        if (_quests.GetById(input.QuestId) is null)
        {
            return NotFound(new { message = "Quest not found." });
        }

        var user = await _users.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var comment = new Comment
        {
            QuestId = input.QuestId,
            Text = input.Text.Trim(),
            Rating = input.Rating,
            AuthorName = user.UserName ?? "Adventurer",
            AuthorId = user.Id
        };

        _comments.Add(comment);
        _comments.SaveChanges();

        return CreatedAtAction(
            nameof(GetByQuest),
            new { questId = comment.QuestId },
            ToDto(comment, user.Id));
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var comment = _comments.GetById(id);
        if (comment is null)
        {
            return NotFound();
        }

        // A comment can only be removed by the account that wrote it.
        if (comment.AuthorId != _users.GetUserId(User))
        {
            return Forbid();
        }

        _comments.Remove(comment);
        _comments.SaveChanges();

        return NoContent();
    }

    private static CommentDto ToDto(Comment comment, string? currentUserId)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Author = comment.AuthorName,
            Text = comment.Text,
            Rating = comment.Rating,
            CreatedAt = comment.CreatedAt,
            CanDelete = currentUserId is not null && comment.AuthorId == currentUserId
        };
    }
}
