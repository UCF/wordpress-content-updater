using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;

using MySql.Data.MySqlClient;

namespace wordpress_update_fix
{
    class Program
    {
        static List<PostUpdate> posts;
        static List<RegexTest> tests;

        static void Main(string[] args)
        {
            posts = new List<PostUpdate>();
            tests = SetupTests();
            List<int> blogIds = GetBlogIds();
            foreach (int blogId in blogIds)
            {
                FindBadShortcodes(blogId);
            }

            foreach (PostUpdate post in posts)
            {
                post.UpdateDatabase(CreateMySqlConnection());
            }

            Console.ReadKey();
        }

        static List<int> GetBlogIds()
        {
            MySqlConnection conn = CreateMySqlConnection();
            conn.Open();
            List<int> blogIds = new List<int>();
            MySqlCommand command = new MySqlCommand("SELECT blog_id FROM wp_blogs", conn);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (reader["blog_id"].ToString() != "1")
                {
                    blogIds.Add(int.Parse(reader["blog_id"].ToString()));
                }
            }

            reader.Close();
            conn.Close();
            return blogIds;
        }

        static void FindBadShortcodes(int blogId)
        {
            foreach (RegexTest test in tests)
            {
                MySqlConnection conn = CreateMySqlConnection();
                conn.Open();
                MySqlCommand command = new MySqlCommand(
                    "SELECT * FROM wp_" + blogId + "_posts WHERE post_content REGEXP '" + test.MySqlRegExp + "'",
                    conn
                );

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    FindMatches(reader["post_content"].ToString(), int.Parse(reader["ID"].ToString()), blogId, test);
                }

                reader.Close();
                conn.Close();
            }
        }

        static void FindMatches(string content, int postID, int blogId, RegexTest test)
        {
            MatchCollection matches = Regex.Matches(content, test.RegularExp);

            if (matches.Count > 0)
            {
                PostUpdate update = new PostUpdate(blogId, postID, content);

                foreach (Match m in matches)
                {
                    update.RegexUpdates.Add(new RegexUpdate(m.Value, m.Result(test.ReplaceExp)));
                }

                posts.Add(update);
            }
        }

        static MySqlConnection CreateMySqlConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["WordPressDev"].ConnectionString;
            return new MySqlConnection(connectionString);
        }

        static List<RegexTest> SetupTests() 
        {
            List<RegexTest> retval = new List<RegexTest>();

            retval.Add(new RegexTest(
                 @"\\[image",
                 "<img .*?src=[\"|']\\[image filename=\"(?<filename>\\w+)\"\\][\"|'](?<extras>.*?)\\/?>",
                 "<img src=\"[image filename='${filename}']\"${extras}>"
                )
            );

            return retval;
        }
    }
}
