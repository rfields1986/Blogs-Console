﻿using NLog;
using BlogsConsole.Models;
using System;
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
            logger.Info("Program started");
            try
            {
                var menuInput = 0;
                var db = new BloggingContext();

                do
                {
                    Console.Write("1. Display Blogs Or Posts\n2. Create New Blog\n3. Create New Post\n4. Exit\n\nPlease Enter The Number Of Your Choice-->  ");
                    Int32.TryParse(Console.ReadLine(), out menuInput);



                    switch (menuInput)
                    {
                        case 1:
                            //Display Menu Options 
                            var menuInput1 = 0;
                            Console.Write("1. Display All Blogs By Name And ID\n2.Display a Post From A Blog Or All Blogs In The Database\n3. \n4. Exit\n\nPlease Enter The Number Of Your Choice-->  ");
                            Int32.TryParse(Console.ReadLine(), out menuInput1);
                            switch (menuInput1)
                            {
                                case 1:
                                    // Display all Blogs from the database
                                    var query = db.Blogs.OrderBy(b => b.Name);

                                    Console.WriteLine("All blogs in the database:");
                                    foreach (var item in query)
                                    {
                                        Console.WriteLine($"Blog Name: {item.Name} | Blog ID: {item.BlogId}");
                                    }
                                    break;

                                case 2:
                                    //Display a Post from a blog or all blogs in the database
                                    var query2 = db.Blogs.OrderBy(b => b.Name);

                                    Console.WriteLine("All blogs in the database:");
                                    foreach (var item in query2)
                                    {
                                        Console.WriteLine($"Blog Name: {item.Name} | Blog ID: {item.BlogId}");
                                    }

                                    Console.WriteLine("Which Blog's Posts Would You Like To See?\nPlease Enter The Blog ID-->  ");
                                    Int32.TryParse(Console.ReadLine(), out var input1);
                                    var blogChoice1 = db.Blogs.FirstOrDefault(b => b.BlogId == input1);
                                    logger.Info($"The Blog Chosen Is {blogChoice1.Name} ");
                                    Console.WriteLine($"{blogChoice1.Posts}");
                                    Console.ReadKey();







                                    break;

                            }




                            break;


                        case 2:
                            // Create and save a new Blog
                            Console.Write("Enter a name for a new Blog: ");
                            var name = Console.ReadLine();
                            var blog = new Blog { Name = name };
                            db.AddBlog(blog);
                            logger.Info("Blog added - {name}", name);
                            break;

                        case 3:
                            // Create New Post
                            Console.WriteLine("Which Blog Would You Like To Post In?\nPlease Enter Blog ID-->  ");
                            Int32.TryParse(Console.ReadLine(), out var input2);
                            var blogChoice2 = db.Blogs.FirstOrDefault(b => b.BlogId == input2);
                            Console.WriteLine($"The Blog Chosen Is {blogChoice2.Name} ");
                            Console.WriteLine("Please Enter Post Title--> ");
                            Post newPost = new Post();
                            newPost.Title = Console.ReadLine();
                            Console.WriteLine("Please Enter The Post Content Below");
                            newPost.Content = Console.ReadLine();
                            newPost.BlogId = blogChoice2.BlogId;
                            db.AddPost(newPost);
                            db.SaveChanges();
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
