using System.Collections.Generic;
using Earthwatchers.Models;

namespace Earthwatchers.Data
{
    public interface ICommentRepository
    {
        void DeleteComment(int commentId);
        List<Comment> GetCommentsByLand(int id);
        List<Comment> GetCommentsByUserId(int id);
        Comment PostComment(Comment comment);
    }
}
