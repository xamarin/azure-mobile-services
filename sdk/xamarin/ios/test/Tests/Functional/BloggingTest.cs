// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.WindowsAzure.MobileServices;

namespace Microsoft.Azure.Zumo.Win8.CSharp.Test
{
    [DataTable(Name = "blog_posts")]
    public class BlogPost
    {
        public int Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "commentCount")]
        public int CommentCount { get; set; }
    }

    [DataTable(Name = "blog_comments")]
    public class BlogComment
    {
        public int Id { get; set; }

        [DataMember(Name = "postid")]
        public int BlogPostId { get; set; }

        [DataMember(Name = "name")]
        public string UserName { get; set; }

        [DataMember(Name = "commentText")]
        public string Text { get; set; }
    }

    public class BloggingTest : FunctionalTestBase
    {
        [Test]
        public void PostComments()
        {
            MobileServiceClient client = GetClient();
            IMobileServiceTable<BlogPost> postTable = client.GetTable<BlogPost>();
            IMobileServiceTable<BlogComment> commentTable = client.GetTable<BlogComment>();

            // Add a few posts and a comment
            //Log("Adding posts");
            BlogPost post = new BlogPost { Title = "Windows 8" };
            postTable.InsertAsync(post).WaitOrFail (Timeout);
            BlogPost highlight = new BlogPost { Title = "ZUMO" };
            postTable.InsertAsync(highlight).WaitOrFail (Timeout);
            commentTable.InsertAsync(new BlogComment {
                BlogPostId = post.Id,
                UserName = "Anonymous",
                Text = "Beta runs great" }).Wait (Timeout);
            commentTable.InsertAsync(new BlogComment {
                BlogPostId = highlight.Id,
                UserName = "Anonymous",
                Text = "Whooooo" }).WaitOrFail (Timeout);
            Assert.AreEqual(2, postTable.Where(p => p.Id >= post.Id).ToListAsync().WaitOrFail (Timeout).Count);

            // Navigate to the first post and add a comment
            //Log("Adding comment to first post");
            BlogPost first = postTable.LookupAsync (post.Id).WaitOrFail (Timeout);
            Assert.AreEqual("Windows 8", first.Title);
            BlogComment opinion = new BlogComment { BlogPostId = first.Id, Text = "Can't wait" };
            commentTable.InsertAsync(opinion).WaitOrFail (Timeout);
            Assert.AreNotEqual(0, opinion.Id);
        }
    }
}
