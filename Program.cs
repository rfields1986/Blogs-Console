using NLog;
using BlogsConsole.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
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
                var menuInput = 0;
                var db = new BloggingContext();

                do
                {
                    Console.Clear();
                    Console.Write("1. Display Blogs Or Posts\n2. Create New Blog\n3. Create New Post\n4. Exit\n\nPlease Enter The Number Of Your Choice-->  ");
                    Int32.TryParse(Console.ReadLine(), out menuInput);



                    switch (menuInput)
                    {
                        case 1:
                            //Display Menu Options 
                            Console.Clear();
                            var menuInput1 = 0;
                            Console.Write("1.Display All Blogs By Name And ID\n2.Display a Post From A Blog Or All Blogs In The Database\n3.Exit\n\nPlease Enter The Number Of Your Choice-->  ");
                            Int32.TryParse(Console.ReadLine(), out menuInput1);
                            switch (menuInput1)
                            {
                                case 1:
                                    // Display all Blogs from the database
                                    Console.Clear();
                                    var query = db.Blogs.OrderBy(b => b.Name);

                                    Console.WriteLine("All blogs in the database:");
                                    foreach (var item in query)
                                    {
                                        Console.WriteLine($"Blog Name: {item.Name} | Blog ID: {item.BlogId}");
                                    }

                                    Console.ReadKey();
                                    break;

                                case 2:
                                    Console.Clear();
                                    //Display a Post from a blog or all blogs in the database
                                    var query2 = db.Blogs.OrderBy(b => b.Name);
                                    IEnumerable<Post> Posts;
                                    Console.WriteLine("All blogs in the database:");
                                    foreach (var item in query2)
                                    {
                                        Console.WriteLine($"Blog Name: {item.Name} | Blog ID: {item.BlogId}");
                                    }

                                    Console.Write("Which Blog's Posts Would You Like To See?\nPlease Enter The Blog ID Or Hit Enter To See All Blog's Posts-->  ");

                                    if (Int32.TryParse(Console.ReadLine(), out var input1) == false)
                                    {
                                        Posts = db.Posts.OrderBy(p => p.Title);
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
                                            Posts = db.Posts.Where(p => p.BlogId == blogChoice1.BlogId);
                                            Console.WriteLine($"{Posts.Count()} posts returned");
                                            if (Posts.Count() == 0)
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

                                    break;

                            }

                            break;

                        case 2:
                            // Create and save a new Blog
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

                            break;

                        case 3:
                            // Create New Post
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

                            break;
                            

                    }
                } while (menuInput != 4);

                Console.WriteLine("Goodbye");


            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }
    }
}
