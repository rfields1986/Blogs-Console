using NLog;
using BlogsConsole.Models;
using System;
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
                    Console.Write("1. Display all Blogs\n2. Create New Blog\n3. Create New Post\n4. Exit\n\nPlease Enter Your Choice-->  ");
                    Int32.TryParse(Console.ReadLine(), out  menuInput);

                    

                    switch (menuInput)
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
                            // Create and save a new Blog
                            Console.Write("Enter a name for a new Blog: ");
                            var name = Console.ReadLine();
                            var blog = new Blog { Name = name };
                            db.AddBlog(blog);
                            logger.Info("Blog added - {name}", name);
                            break;

                        case 3:
                            Console.WriteLine("Which Blog Would You Like To Post In?\nPlease Enter Blog ID-->  ");
                            Int32.TryParse(Console.ReadLine(), out var input1);
                            var blogChoice = db.Blogs.FirstOrDefault(b => b.BlogId == input1);
                            Console.WriteLine($"The Blog Chosen Is {blogChoice.Name} ");
                            Console.WriteLine("Please Enter Post Title--> ");
                            Post newPost = new Post();
                            newPost.Title = Console.ReadLine();
                            Console.WriteLine("Please Enter The Post Content Below");
                            newPost.Content = Console.ReadLine();
                            newPost.BlogId = blogChoice.BlogId;
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
