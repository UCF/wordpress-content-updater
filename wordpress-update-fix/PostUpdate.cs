using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;

namespace wordpress_update_fix
{
    public class RegexUpdate
    {
        private string match;
        private string replace;

        public string Match
        {
            get { return Regex.Escape(this.match); }
        }

        public string Replace
        {
            get { return Regex.Escape(this.replace); }
        }

        public RegexUpdate(string match, string replace)
        {
            this.match = match;
            this.replace = replace;
        }
    }

    public class PostUpdate
    {
        private int blogID;
        private int postID;
        private string originalContent;
        private string replacedContent;
        List<RegexUpdate> regexUpdates;
        private bool localUpdated;
        private bool dbUpdated;

        public int BlogID
        {
            get { return blogID; }
        }

        public int PostID
        {
            get { return postID; }
        }

        public string OriginalContent
        {
            get { return originalContent; }
        }

        public string ReplaceContent
        {
            get { return replacedContent; }
            set { replacedContent = value; }
        }

        public List<RegexUpdate> RegexUpdates
        {
            get { return regexUpdates; }
            set { regexUpdates = value; }
        }

        public bool LocalUpdated
        {
            get { return localUpdated; }
        }

        public bool DBUpdated
        {
            get { return dbUpdated; }
        }

        public PostUpdate(int blogID, int postID, string originalContent)
        {
            this.blogID = blogID;
            this.postID = postID;
            this.originalContent = originalContent;
            this.regexUpdates = new List<RegexUpdate>();
            localUpdated = false;
            dbUpdated = false;
        }

        public void UpdateLocal()
        {
            if (string.IsNullOrWhiteSpace(this.replacedContent))
            {
                this.replacedContent = this.originalContent;
            }

            foreach (RegexUpdate update in regexUpdates)
            {
                this.replacedContent = Regex.Replace(this.replacedContent, update.Match, update.Replace);
            }

            this.localUpdated = true;
        }

        public void UpdateDatabase(MySqlConnection conn)
        {
            if (this.localUpdated == false)
            {
                UpdateLocal();
            }

            conn.Open();

            MySqlCommand command = new MySqlCommand(
                "UPDATE wp_" + blogID + "_posts SET post_content = '" + Regex.Unescape(replacedContent) + "' where ID = " + postID + ";",
                conn
            );

            Console.WriteLine(command.CommandText);

            //command.ExecuteNonQuery();
            //this.dbUpdated = true;

            conn.Close();
        }

        public void Rollback(MySqlConnection conn)
        {
            conn.Open();

            MySqlCommand command = new MySqlCommand(
                "UPDATE wp_" + blogID + "_posts SET post_content = '" + originalContent + "' where ID = "  + postID + ";",
                conn
            );

            command.ExecuteNonQuery();
            this.dbUpdated = false;

            conn.Close();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("PostID:\t" + this.postID);
            //sb.AppendLine("Content:\t" + this.originalContent);
            sb.AppendLine("Replacements:");
            foreach (RegexUpdate update in regexUpdates)
            {
                sb.AppendLine("\tOriginal: " + update.Match + " | Replacement: " + update.Replace);
            }

            return sb.ToString();
        }

    }
}
