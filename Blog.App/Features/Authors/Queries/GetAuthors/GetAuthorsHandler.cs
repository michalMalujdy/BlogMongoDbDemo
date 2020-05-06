using System.Threading;
using System.Threading.Tasks;
using Blog.Core.Domain.Models;
using Blog.Core.Repositories;
using Blog.Core.Resources;
using MediatR;

namespace Blog.App.Features.Authors.Queries.GetAuthors
{
    public class GetAuthorsHandler : IRequestHandler<GetAuthorsQuery, PagableListResult<Author>>
    {
        private readonly IAuthorsRepository _authorsRepository;

        public GetAuthorsHandler(IAuthorsRepository authorsRepository)
            => _authorsRepository = authorsRepository;

        public Task<PagableListResult<Author>> Handle(GetAuthorsQuery query, CancellationToken ct)
            => _authorsRepository.GetMany(query.PageNr, query.PageSize, ct);
    }
}