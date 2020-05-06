using System;
using Blog.Core.Resources;
using MediatR;

namespace Blog.App.Features.Posts.Queries.GetPost
{
    public class GetPostQuery : IRequest<PostCompleteResource>
    {
        public Guid PostId { get; set; }
    }
}