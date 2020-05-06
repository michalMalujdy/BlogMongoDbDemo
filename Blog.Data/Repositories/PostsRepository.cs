using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Api.Configurations;
using Blog.Core.Domain.Models;
using Blog.Core.Repositories.Posts;
using Blog.Core.Resources;
using Blog.Data.DbModels;
using MongoDB.Driver;

namespace Blog.Data.Repositories
{
    public class PostsRepository : IPostsRepository
    {
        private readonly IMongoCollection<Post> _postsCollection;
        private readonly IMongoCollection<Author> _authorsCollection;
        private readonly IMapper _mapper;

        public PostsRepository(MongoDbSettings dbSettings, IMapper mapper)
        {
            var db = new MongoClient(dbSettings.ConnectionString)
                .GetDatabase(dbSettings.DbName);

            _postsCollection = db.GetCollection<Post>("Posts");
            _authorsCollection = db.GetCollection<Author>("Authors");

            _mapper = mapper;
        }

        public async Task<Guid> Create(Post post)
        {
            await _postsCollection.InsertOneAsync(post);

            return post.Id;
        }

        public async Task<Post> Get(Guid postId)
        {
            var postsCursor = await _postsCollection.FindAsync(post => post.Id == postId);

            return await postsCursor.FirstAsync();
        }

        public async Task<PostWithAuthorResource> GetPostWithAuthor(Guid postId)
        {
            var postWithAuthorDbModel = await _postsCollection

                .Aggregate()
                .Lookup<Post, Author, PostWithAuthorsDbModel>(
                    _authorsCollection,
                    post => post.AuthorId,
                    author => author.Id,
                    model => model.Authors
                )
                .Match(p => p.Id == postId)
                .FirstOrDefaultAsync();

            if (postWithAuthorDbModel == null)
                return null;

            return _mapper.Map<PostWithAuthorResource>(postWithAuthorDbModel);
        }

        public async Task<List<PostWithAuthorResource>> GetAll()
        {
            var posts = await _postsCollection
                .Aggregate()
                .Lookup<Post, Author, PostWithAuthorsDbModel>(
                    _authorsCollection,
                    post => post.AuthorId,
                    author => author.Id,
                    model => model.Authors
                )
                .ToListAsync();

            return _mapper.Map<List<PostWithAuthorResource>>(posts);
        }

        public Task Update(Post post)
            => _postsCollection.ReplaceOneAsync(
                p => p.Id == post.Id,
                post);

        public Task Delete(Guid postId)
            => _postsCollection.DeleteOneAsync(p => p.Id == postId);
    }
}