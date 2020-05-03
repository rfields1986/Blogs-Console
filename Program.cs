using NLog;
using BlogsConsole.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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
                Console.WriteLine($"\n\nBlog Name: {item.Name,-15}\nBlog ID: {item.BlogId,-15}");
            }
            Console.WriteLine("\n\nPress Any Key To Return To The Main Menu");
            Console.ReadKey();
        }

        public static void ABlogsPostOrAllBlogsPost()
        {
            //Display a Post from a blog or all blogs in the database
            Console.Clear();
            var db = new BloggingContext();
            var query2 = db.Blogs.OrderBy(b => b.Name);
            Console.WriteLine("Blog List:");
            foreach (var item in query2)
            {
                Console.WriteLine($"\n\nBlog Name: {item.Name,-15}\nBlog ID: {item.BlogId,-15}");
            }

            Console.Write("\n\nWhich Blog's Posts Would You Like To See?\nPlease Enter The Blog ID Or Hit Enter To See All Blog's Posts-->  ");

            if (Int32.TryParse(Console.ReadLine(), out var input) == false)
            {
                Console.Clear();
                var Posts = db.Posts.OrderBy(p => p.Title);
                foreach (var item in Posts)
                {
                    Console.WriteLine($"\n\n{"Blog:",-10}{item.Blog.Name}\n{"PostID:",-10}{item.PostId}\n{"Title:",-10}{item.Title}\n{"Post:",-10}{item.Content}\n\n");
                }

                Console.WriteLine("Press Any Key To Return To The Main Menu");
                Console.ReadKey();
            }
            else
            {
                Console.Clear();
                var blogChoice = db.Blogs.FirstOrDefault(b => b.BlogId == input);
                if (db.Blogs.Any(b => b.BlogId == input) == false)
                {
                    logger.Error($"\n\nBlog ID: {input} does not exist");
                    Console.ReadKey();
                }
                else
                {
                    logger.Info($"\n\nThe Blog Chosen Is {blogChoice.Name}\n\n");
                    var Posts = db.Posts.Where(p => p.BlogId == blogChoice.BlogId);
                    Console.WriteLine($"{Posts.Count()} posts returned\n");
                    if (Posts.Any() == false)
                    {
                        logger.Info("\nThis Blog Has No Posts\n");
                    }
                    else
                    {


                        foreach (var item in Posts)
                        {
                            Console.WriteLine($"{"\n\nPostID:",-10}{item.PostId}\n{"Title:",-8}{item.Title}\n{"Post:",-8}{item.Content}\n\n");
                        }
                    }
                    Console.WriteLine("Press Any Key To Return To The Main Menu");
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
                logger.Error("\n\nThis Name Already Exists\n\n");
                Console.ReadKey();
            }
            else
            {
                var blog = new Blog { Name = name };
                db.AddBlog(blog);
                logger.Info("\n\nBlog added - {name}\n\n", name);
                Console.ReadKey();
            }
        }

        public static void NewPost()
        {
            // Create New Post
            var db = new BloggingContext();
            Console.Clear();
            var query = db.Blogs.OrderBy(b => b.Name);
            Console.WriteLine("All blogs in the database:");

            foreach (var item in query)
            {
                Console.WriteLine($"\n\nBlog Name: {item.Name,-15}\nBlog ID: {item.BlogId,-15}");
            }

            Console.Write("\n\nWhich Blog Would You Like To Post In?\nPlease Enter Blog ID-->  ");
            Int32.TryParse(Console.ReadLine(), out var input);
            var blogChoice = db.Blogs.FirstOrDefault(b => b.BlogId == input);
            if (db.Blogs.Count(b => b.BlogId == blogChoice.BlogId) == 0)
            {
                logger.Error($"\n\nBlog ID: {input} does not exist\n\n");
            }
            else
            {

                Console.WriteLine($"\n\nThe Blog Chosen Is {blogChoice.Name} ");
                Console.Write("\n\nPlease Enter Post Title--> ");
                Post newPost = new Post();
                newPost.Title = Console.ReadLine();
                Console.Write("\nPlease Enter The Post Content--> ");
                newPost.Content = Console.ReadLine();
                newPost.BlogId = blogChoice.BlogId;
                db.AddPost(newPost);
                Console.WriteLine();
                logger.Info($"{newPost} added {DateTime.Now}\n\n");
                db.SaveChanges();
            }
            Console.WriteLine("Press Any Key To Return To The Main Menu");
            Console.ReadKey();
        }

        public static void EditBlog()
        {
            Console.Clear();
            var db = new BloggingContext();
            var query = db.Blogs.OrderBy(b => b.Name);
            Console.WriteLine("All blogs in the database:");

            foreach (var item in query)
            {
                Console.WriteLine($"\n\nBlog Name: {item.Name,-15}\nBlog ID: {item.BlogId,-15}");
            }

            Console.Write("\n\nPlease Enter The Blog ID You Would Like To Edit--> ");
            Int32.TryParse(Console.ReadLine(), out var input);
            var blogChoice = db.Blogs.FirstOrDefault(b => b.BlogId == input);
            if (db.Blogs.Any(b => b.BlogId == input) == false)
            {
                Console.WriteLine();
                logger.Error($"Blog ID: {input} does not exist");
                Console.WriteLine("Press Any key");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine();
                logger.Info($"The Blog Chosen Is {blogChoice.Name}");
                var editedBlog = db.Blogs.Find(blogChoice.BlogId);
                Console.Write("\n\nPlease Enter A New Blog Name--> ");
                var blogName = Console.ReadLine();
                if (db.Blogs.Any(b => b.Name == blogName))
                {
                    logger.Error("\n\nThis Name Already Exists\n\n");
                    Console.WriteLine("Press Any Key To Continue");
                    Console.ReadKey();
                }
                else
                {
                    editedBlog.Name = blogName;
                    db.SaveChanges();
                    Console.WriteLine();
                    logger.Info($"The Blog {blogChoice.Name} was edited on {DateTime.Now}\n");
                }
            }
            Console.WriteLine("Press Any Key To Return To The Main Menu");
            Console.ReadKey();
        }

        public static void EditPost()
        {
            //Display a Post from a blog or all blogs in the database
            Console.Clear();
            var db = new BloggingContext();
            var query = db.Blogs.OrderBy(b => b.Name);
            Console.WriteLine("All blogs in the database:");

            foreach (var item in query)
            {
                Console.WriteLine($"\n\nBlog Name: {item.Name,-15}\nBlog ID: {item.BlogId,-15}");
            }

            Console.Write("\n\nWhich Blog's Posts Would You Like To Edit?\nPlease Enter The Blog ID-->  ");
            Int32.TryParse(Console.ReadLine(), out var input);
            var blogChoice = db.Blogs.FirstOrDefault(b => b.BlogId == input);
            if (db.Blogs.Any(b => b.BlogId == input) == false)
            {
                Console.WriteLine();
                logger.Error($"Blog ID: {input} does not exist");
                Console.WriteLine("\n\nPress Any Key");
                Console.ReadKey();
            }
            else
            {
                Console.Clear();
                logger.Info($"The Blog Chosen Is {blogChoice.Name}\n\n");
                var Posts = db.Posts.Where(p => p.BlogId == blogChoice.BlogId);
                Console.WriteLine($"{Posts.Count()} posts returned\n\n");

                if (Posts.Any() == false)
                { 
                    logger.Info("\nThis Blog Has No Posts\n");
                }
                else
                {

                    foreach (var item in Posts)
                    {
                        Console.WriteLine($"{"\n\nPostID:",-10}{item.PostId}\n{"Title:",-8}{item.Title}\n{"Post:",-8}{item.Content}\n\n");
                    }
                }

            }

            var postToBeEdited = new Post();
            var exit = 1;
            do
            {
                exit = 1;
                Console.Write("\nPlease Enter The ID Of The Post Would You Like To Edit--> ");
                Int32.TryParse(Console.ReadLine(), out var input1);
                postToBeEdited.PostId = input1;
                var checkPost = db.Posts.Find(postToBeEdited.PostId);
                if (checkPost.BlogId != blogChoice.BlogId)
                {
                    Console.WriteLine("\n\nNo Post With That ID In This Blog");
                    Console.WriteLine();
                    logger.Error("User Entered Incorrect Post ID");
                    exit = 0;
                }

            } while (exit != 1);


            Console.Write("\n\nPlease Enter A New Title--> ");
            postToBeEdited.Title = Console.ReadLine();
            Console.Write("\n\nPlease Enter The New Content --> ");
            postToBeEdited.Content = Console.ReadLine();
            var editedPost = db.Posts.Find(postToBeEdited.PostId);
            editedPost.Title = postToBeEdited.Title;
            editedPost.Content = postToBeEdited.Content;
            db.SaveChanges();
            Console.WriteLine();
            logger.Info($"The Post {editedPost.Title} was edited on {DateTime.Now}\n\n");
            Console.WriteLine("Press Any Key To Return To The Main Menu");
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
                Console.WriteLine($"\n\nBlog Name: {item.Name,-15}\nBlog ID: {item.BlogId,-15}");
            }

            Console.Write("\n\nPlease Enter The Blog ID You Would Like To Delete--> ");
            Int32.TryParse(Console.ReadLine(), out var input);
            var blogChoice = db.Blogs.FirstOrDefault(b => b.BlogId == input);
            if (db.Blogs.Any(b => b.BlogId == input) == false)
            {
                Console.WriteLine();
                logger.Error($"Blog ID: {input} does not exist");
                Console.ReadKey();
            }
            else
            {
                Console.Clear();
                logger.Info($"\nThe Blog Chosen Is {blogChoice.Name}\n");
                Console.WriteLine("Are You Sure You Want To Delete This Blog?\nAll Posts In This Blog Will Be Deleted As Well.\nType (Y) To Proceed Or (N) To Cancel.");
                char.TryParse(Console.ReadLine().ToUpper(), out var deleteChoice);
                if (deleteChoice == 'Y')
                {
                    Console.Clear();
                    var Posts = db.Posts.Where(p => p.BlogId == blogChoice.BlogId);
                    foreach (var p in Posts)
                    {
                        db.Posts.Remove(p);
                    }

                    db.Blogs.Remove(blogChoice);
                    db.SaveChanges();
                    logger.Info($"\nThe Blog {blogChoice.Name} And Its Posts were deleted on {DateTime.Now}\n");
                }
                else
                {
                    logger.Info("Delete Operation Cancelled");
                }

            }
            Console.WriteLine("Press Any Key To Return To The Main Menu");
            Console.ReadKey();
        }

        public static void DeletePost()
        {
            {
                //Display a Post from a blog or all blogs in the database
                Console.Clear();
                var db = new BloggingContext();
                var query = db.Blogs.OrderBy(b => b.Name);
                Console.WriteLine("All blogs in the database:");

                foreach (var item in query)
                {
                    Console.WriteLine($"\n\nBlog Name: {item.Name,-15}\nBlog ID: {item.BlogId,-15}");
                }

                Console.Write("\n\nWhich Blog's Posts Would You Like To Edit?\nPlease Enter The Blog ID-->  ");
                Int32.TryParse(Console.ReadLine(), out var input);
                var blogChoice = db.Blogs.FirstOrDefault(b => b.BlogId == input);
                if (db.Blogs.Any(b => b.BlogId == input) == false)
                {
                    Console.WriteLine();
                    logger.Error($"Blog ID: {input} does not exist");
                    Console.WriteLine("Press Any Key");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine();
                    Console.Clear();
                    logger.Info($"\nThe Blog Chosen Is {blogChoice.Name}\n");
                    var Posts = db.Posts.Where(p => p.BlogId == blogChoice.BlogId);
                    Console.WriteLine($"{Posts.Count()} posts returned");

                    if (Posts.Any() == false)
                    {
                        logger.Info("\nThis Blog Has No Posts\n");
                    }
                    else
                    {

                        foreach (var item in Posts)
                        {
                            Console.WriteLine($"{"\n\nPostID:",-10}{item.PostId}\n{"Title:",-8}{item.Title}\n{"Post:",-8}{item.Content}\n\n");
                        }
                    }


                }

                var postToBeDeleted = new Post();
                var exit = 1;
                var input1 = 0;
                do
                {
                    exit = 1;
                    Console.Write("\nPlease Enter The ID Of The Post Would You Like To Delete--> ");
                    Int32.TryParse(Console.ReadLine(), out  input1);
                    postToBeDeleted.PostId = input1;
                    var checkPost = db.Posts.Find(postToBeDeleted.PostId);
                    if (checkPost == null)
                    {
                        Console.WriteLine("\n\nNo Post With That ID In This Blog");
                        Console.WriteLine();
                        logger.Error("User Entered Incorrect Post ID");
                        exit = 0;
                    }
                    else if (checkPost.BlogId != blogChoice.BlogId)
                    {
                        Console.WriteLine("\n\nNo Post With That ID In This Blog");
                        Console.WriteLine();
                        logger.Error("User Entered Incorrect Post ID");
                        exit = 0;
                    }

                } while (exit != 1);

                postToBeDeleted.PostId = input1;
                var deletedPost = db.Posts.Find(postToBeDeleted.PostId);
                Console.WriteLine("Are You Sure You Want To Delete This Post?\nType (Y) To Proceed Or (N) To Cancel.");
                char.TryParse(Console.ReadLine().ToUpper(), out var deleteChoice);
                if (deleteChoice == 'Y')
                {
                    Console.Clear();
                    db.Posts.Remove(deletedPost);
                    db.SaveChanges();
                    Console.WriteLine("\n\n");
                    logger.Info($"The Post {deletedPost.Title} was deleted on {DateTime.Now}\n\n");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("\n\n");
                    logger.Info("\nDelete Operation Cancelled");
                }

                Console.WriteLine("Press Any Key To Return To The Main Menu");
                Console.ReadKey();
            }
        }
    }
}
