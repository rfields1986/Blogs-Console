using NLog;
using BlogsConsole.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;

namespace BlogsConsole
{
    class MainClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            logger.Info("Program started\n");
            try
            {

               var menuInput1 = 0;

                do
                {
                    menuInput1 = DisplayMenu1();

                    switch (menuInput1)
                    {

                        case 1:

                            var menuInput2 = DisplayMenu2();

                            switch (menuInput2)
                            {
                                case 1:

                                    AllBlogDisplay();

                                    break;

                                case 2:

                                    ABlogsPostOrAllBlogsPost();

                                    break;

                            }

                            break;

                        case 2:

                            NewBlog();

                            break;

                        case 3:

                            NewPost();

                            break;

                        case 4:

                            EditBlog();

                            break;

                        case 5:

                            EditPost();

                            break;

                        case 6:

                            DeleteBlog();

                            break;

                        case 7:

                            DeletePost();

                            break;


                    }
                } while (menuInput1 != 8);

                Console.WriteLine("Goodbye");


            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }

        public static int DisplayMenu1()
        {
            Console.Clear();
            var input = 0;
            Console.Write("1. Display Blogs Or Posts\n2. Create New Blog\n3. Create New Post\n4. Edit Blog\n5. Edit Post\n6. Delete Blog\n7. Delete Post\n8. Exit" +
                          "\n\nPlease Enter The Number Of Your Choice-->  ");
            Int32.TryParse(Console.ReadLine(), out input);
            return input;
        }

        public static int DisplayMenu2()
        {
            Console.Clear();
            var input = 0;
            Console.Write("1.Display All Blogs By Name And ID\n2.Display a Post From A Blog Or All Blogs In The Database\n3.Exit\n\nPlease Enter The Number Of Your Choice-->  ");
            Int32.TryParse(Console.ReadLine(), out input);
            return input;
        }

        public static void AllBlogDisplay()
        {
            // Display all Blogs from the database
            Console.Clear();
            var db = new BloggingContext();
            var query = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine("All blogs in the database:");
            foreach (var item in query)
            {
                Console.WriteLine($"Blog Name: {item.Name} | Blog ID: {item.BlogId}");
            }

            Console.ReadKey();
        }

        public static void ABlogsPostOrAllBlogsPost()
        {
            //Display a Post from a blog or all blogs in the database
            Console.Clear();
            var db = new BloggingContext();
            var query2 = db.Blogs.OrderBy(b => b.Name);
            Console.WriteLine("All blogs in the database:");
            foreach (var item in query2)
            {
                Console.WriteLine($"Blog Name: {item.Name} | Blog ID: {item.BlogId}");
            }

            Console.Write("Which Blog's Posts Would You Like To See?\nPlease Enter The Blog ID Or Hit Enter To See All Blog's Posts-->  ");

            if (Int32.TryParse(Console.ReadLine(), out var input1) == false)
            {
                var Posts = db.Posts.OrderBy(p => p.Title);
                foreach (var item in Posts)
                {
                    Console.WriteLine($"Blog: {item.Blog.Name}\nTitle: {item.Title}\nPost: {item.Content}\n\n");
                }

                Console.ReadKey();
            }
            else
            {
                var blogChoice1 = db.Blogs.FirstOrDefault(b => b.BlogId == input1);
                if (db.Blogs.Any(b => b.BlogId == input1) == false)
                {
                    logger.Error($"Blog ID: {input1} does not exist");
                    Console.ReadKey();
                }
                else
                {
                    logger.Info($"\nThe Blog Chosen Is {blogChoice1.Name}\n");
                    var Posts = db.Posts.Where(p => p.BlogId == blogChoice1.BlogId);
                    Console.WriteLine($"{Posts.Count()} posts returned");
                    if (Posts.Any() == false)
                    {
                        logger.Info("\nThis Blog Has No Posts\n");
                    }
                    else
                    {


                        foreach (var item in Posts)
                        {
                            Console.WriteLine(
                                $"Blog: {item.Blog.Name}\nTitle: {item.Title}\nPost: {item.Content}\n\n");
                        }
                    }

                    Console.ReadKey();
                }
            }
        }

        public static void NewBlog()
        {
            // Create and save a new Blog
            var db = new BloggingContext();
            Console.Clear();
            Console.Write("Enter a name for a new Blog: ");
            var name = Console.ReadLine();
            if (db.Blogs.Any(b => b.Name == name))
            {
                logger.Error("This Name Already Exists");
                Console.ReadKey();
            }
            else
            {
                var blog = new Blog { Name = name };
                db.AddBlog(blog);
                logger.Info("\nBlog added - {name}\n", name);
            }
        }

        public static void NewPost()
        {
            // Create New Post
            var db = new BloggingContext();
            Console.Clear();
            var query1 = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine("All blogs in the database:");
            foreach (var item in query1)
            {
                Console.WriteLine($"Blog Name: {item.Name} | Blog ID: {item.BlogId}");
            }
            Console.Write("\n\nWhich Blog Would You Like To Post In?\nPlease Enter Blog ID-->  ");
            Int32.TryParse(Console.ReadLine(), out var input2);
            var blogChoice2 = db.Blogs.FirstOrDefault(b => b.BlogId == input2);
            if (db.Blogs.Count(b => b.BlogId == blogChoice2.BlogId) == 0)
            {
                logger.Error($"\nBlog ID: {input2} does not exist\n");
            }
            else
            {

                Console.WriteLine($"The Blog Chosen Is {blogChoice2.Name} ");
                Console.Write("Please Enter Post Title--> ");
                Post newPost = new Post();
                newPost.Title = Console.ReadLine();
                Console.WriteLine("Please Enter The Post Content Below");
                newPost.Content = Console.ReadLine();
                newPost.BlogId = blogChoice2.BlogId;
                db.AddPost(newPost);
                db.SaveChanges();
            }
        }

        public static void EditBlog()
        {
            Console.Clear();
            var db = new BloggingContext();
            var query = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine("All blogs in the database:");
            foreach (var item in query)
            {
                Console.WriteLine($"Blog Name: {item.Name} | Blog ID: {item.BlogId}");
            }
            var editedBlog = new Blog();
            Console.Write("Please Enter The Blog ID You Would Like To Edit--> ");
            Int32.TryParse(Console.ReadLine(), out var input1);
            var blogChoice1 = db.Blogs.FirstOrDefault(b => b.BlogId == input1);
            if (db.Blogs.Any(b => b.BlogId == input1) == false)
            {
                logger.Error($"Blog ID: {input1} does not exist");
                Console.ReadKey();
            }
            else
            {
                logger.Info($"\nThe Blog Chosen Is {blogChoice1.Name}\n");
                var uneditedBlog = db.Blogs.Find(blogChoice1.BlogId);
                Console.Write("Please Enter A New Blog Name--> ");
                uneditedBlog.Name = Console.ReadLine();
                db.SaveChanges();
                logger.Info($"\nThe Blog {blogChoice1.Name} was edited on {DateTime.Now}\n");
            }

        }

        public static void EditPost()
        {
            //Display a Post from a blog or all blogs in the database
            Console.Clear();
            var db = new BloggingContext();
            var query2 = db.Blogs.OrderBy(b => b.Name);
            Console.WriteLine("All blogs in the database:");
            foreach (var item in query2)
            {
                Console.WriteLine($"Blog Name: {item.Name} | Blog ID: {item.BlogId}");
            }

            Console.Write("Which Blog's Posts Would You Like To Edit?\nPlease Enter The Blog ID-->  ");
            Int32.TryParse(Console.ReadLine(), out var input1);
            var blogChoice1 = db.Blogs.FirstOrDefault(b => b.BlogId == input1);
            if (db.Blogs.Any(b => b.BlogId == input1) == false)
            {
                    logger.Error($"Blog ID: {input1} does not exist");
                    Console.ReadKey();
            }
            else
            {
                logger.Info($"\nThe Blog Chosen Is {blogChoice1.Name}\n");
                var Posts = db.Posts.Where(p => p.BlogId == blogChoice1.BlogId);
                Console.WriteLine($"{Posts.Count()} posts returned");

                if (Posts.Any() == false)
                { 
                    logger.Info("\nThis Blog Has No Posts\n");
                }
                else
                {

                    foreach (var item in Posts)
                    { 
                        Console.WriteLine($"Blog: {item.Blog.Name}\nPostID: {item.PostId}\nTitle: {item.Title}\nPost: {item.Content}\n\n");
                    }
                }


            }

            var editedPost = new Post();
            Console.Write("Please Enter The ID Of The Post Would You Like To Edit--> ");
            Int32.TryParse(Console.ReadLine(), out input1); 
            editedPost.PostId = input1;
            Console.Write("Please Enter A New Title--> ");
            editedPost.Title = Console.ReadLine();
            Console.WriteLine("Please Enter The New Content below");
            editedPost.Content = Console.ReadLine();
            var uneditedPost = db.Posts.Find(editedPost.PostId);
            uneditedPost.Title = editedPost.Title;
            uneditedPost.Content = editedPost.Content;
            db.SaveChanges();
            logger.Info($"\nThe Post {uneditedPost.Title} was edited on {DateTime.Now}\n");
            Console.ReadKey();

        }

        public static void DeleteBlog()
        {
            Console.Clear();
            var db = new BloggingContext();
            var query = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine("All blogs in the database:");
            foreach (var item in query)
            {
                Console.WriteLine($"Blog Name: {item.Name} | Blog ID: {item.BlogId}");
            }
            Console.Write("Please Enter The Blog ID You Would Like To Delete--> ");
            Int32.TryParse(Console.ReadLine(), out var input1);
            var blogChoice1 = db.Blogs.FirstOrDefault(b => b.BlogId == input1);
            if (db.Blogs.Any(b => b.BlogId == input1) == false)
            {
                logger.Error($"Blog ID: {input1} does not exist");
                Console.ReadKey();
            }
            else
            {
                logger.Info($"\nThe Blog Chosen Is {blogChoice1.Name}\n");
                var Posts = db.Posts.Where(p => p.BlogId == blogChoice1.BlogId);
                foreach (var p in Posts)
                {
                    db.Posts.Remove(p);
                }

                db.Blogs.Remove(blogChoice1);
                db.SaveChanges();
                logger.Info($"\nThe Blog {blogChoice1.Name} And Its Posts were deleted on {DateTime.Now}\n");
            }

            Console.ReadKey();
        }

        public static void DeletePost()
        {
            {
                //Display a Post from a blog or all blogs in the database
                Console.Clear();
                var db = new BloggingContext();
                var query2 = db.Blogs.OrderBy(b => b.Name);
                Console.WriteLine("All blogs in the database:");
                foreach (var item in query2)
                {
                    Console.WriteLine($"Blog Name: {item.Name} | Blog ID: {item.BlogId}");
                }

                Console.Write("Which Blog's Posts Would You Like To Edit?\nPlease Enter The Blog ID-->  ");
                Int32.TryParse(Console.ReadLine(), out var input1);
                var blogChoice1 = db.Blogs.FirstOrDefault(b => b.BlogId == input1);
                if (db.Blogs.Any(b => b.BlogId == input1) == false)
                {
                    logger.Error($"Blog ID: {input1} does not exist");
                    Console.ReadKey();
                }
                else
                {
                    logger.Info($"\nThe Blog Chosen Is {blogChoice1.Name}\n");
                    var Posts = db.Posts.Where(p => p.BlogId == blogChoice1.BlogId);
                    Console.WriteLine($"{Posts.Count()} posts returned");

                    if (Posts.Any() == false)
                    {
                        logger.Info("\nThis Blog Has No Posts\n");
                    }
                    else
                    {

                        foreach (var item in Posts)
                        {
                            Console.WriteLine($"Blog: {item.Blog.Name}\nPostID: {item.PostId}\nTitle: {item.Title}\nPost: {item.Content}\n\n");
                        }
                    }


                }

                var deletedPost = new Post();
                Console.Write("Please Enter The ID Of The Post Would You Like To Delete--> ");
                Int32.TryParse(Console.ReadLine(), out input1);
                deletedPost.PostId = input1;
                var undeletedPost = db.Posts.Find(deletedPost.PostId);
                db.Posts.Remove(undeletedPost);
                db.SaveChanges();
                logger.Info($"\nThe Post {undeletedPost.Title} was deleted on {DateTime.Now}\n");
                Console.ReadKey();
            }
        }
    }
}
