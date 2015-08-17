using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wordpress_update_fix
{
    public class RegexTest
    {
        private string mySqlRegExp;
        private string regularExp;
        private string replaceExp;

        public string MySqlRegExp
        {
            get { return mySqlRegExp; }
        }

        public string RegularExp 
        {
            get { return regularExp; }
        }

        public string ReplaceExp
        {
            get { return replaceExp; }
        }

        public RegexTest(string mySqlRegExp, string regularExp, string replaceExp)
        {
            this.mySqlRegExp = mySqlRegExp;
            this.regularExp = regularExp;
            this.replaceExp = replaceExp;
        }
    }
}
