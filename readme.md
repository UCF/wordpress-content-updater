# WordPress Content Update
This is a quick utility to be used for updating content within a WordPress Multisite database.

It provides a simple framework for creating regex search and replace "tests", then traverses all of the sites within a multisite updating post content as needed.

Each test uses (1) a MySqlRegExp to reduce the number of records that need to be traversed, (2) the RegularExp, which determines what needs to be replaced and (3) the ReplaceExp, which replaces the matched content.

## TO DO
[] Update this readme with more complete information regarding usage.

[] Update RegexTest with an overridable "Update" method that allows for non-regex replacements to be made on records.