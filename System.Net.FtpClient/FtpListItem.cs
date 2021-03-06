using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;

namespace System.Net.FtpClient {
    /// <summary>
    /// Represents a file system object on the server
    /// </summary>
    /// <example><code source="..\Examples\CustomParser.cs" lang="cs" /></example>
    public class FtpListItem {
        FtpFileSystemObjectType m_type = 0;
        /// <summary>
        /// Gets the type of file system object. This property can be
        /// set however this functionality is intended to be done by
        /// custom parsers.
        /// </summary>
        public FtpFileSystemObjectType Type {
            get {
                return m_type;
            }
            set {
                m_type = value;
            }
        }

        string m_path = null;
        /// <summary>
        /// Gets the full path name to the object. This property can be
        /// set however this functionality is intended to be done by
        /// custom parsers.
        /// </summary>
        public string FullName {
            get {
                return m_path;
            }
            set {
                m_path = value;
            }
        }

        string m_name = null;
        /// <summary>
        /// Gets the name of the object. This property can be
        /// set however this functionality is intended to be done by
        /// custom parsers.
        /// </summary>
        public string Name {
            get {
                return m_name;
            }
            set {
                m_name = value;
            }
        }

        DateTime m_modified = DateTime.MinValue;
        /// <summary>
        /// Gets the last write time of the object. This property can be
        /// set however this functionality is intended to be done by
        /// custom parsers.
        /// </summary>
        public DateTime Modified {
            get {
                return m_modified;
            }
            set {
                m_modified = value;
            }
        }

        DateTime m_created = DateTime.MinValue;
        /// <summary>
        /// Gets the created date of the object. This property can be
        /// set however this functionality is intended to be done by
        /// custom parsers.
        /// </summary>
        public DateTime Created {
            get {
                return m_created;
            }
            set {
                m_created = value;
            }
        }

        long m_size = -1;
        /// <summary>
        /// Gets the size of the object. This property can be
        /// set however this functionality is intended to be done by
        /// custom parsers.
        /// </summary>
        public long Size {
            get {
                return m_size;
            }
            set {
                m_size = value;
            }
        }

        FtpSpecialPermissions m_specialPermissions = FtpSpecialPermissions.None;
        /// <summary>
        /// Gets special UNIX permissions such as Stiky, SUID and SGID. This property can be
        /// set however this functionality is intended to be done by
        /// custom parsers.
        /// </summary>
        public FtpSpecialPermissions SpecialPermissions {
            get { 
                return m_specialPermissions; 
            }
            set { 
                m_specialPermissions = value; 
            }
        }

        FtpPermission m_ownerPermissions = FtpPermission.None;
        /// <summary>
        /// Gets the owner permissions. This property can be
        /// set however this functionality is intended to be done by
        /// custom parsers.
        /// </summary>
        public FtpPermission OwnerPermissions {
            get { 
                return m_ownerPermissions; 
            }
            set { 
                m_ownerPermissions = value; 
            }
        }

        FtpPermission m_groupPermissions = FtpPermission.None;
        /// <summary>
        /// Gets the group permissions. This property can be
        /// set however this functionality is intended to be done by
        /// custom parsers.
        /// </summary>
        public FtpPermission GroupPermissions {
            get { 
                return m_groupPermissions; 
            }
            set { 
                m_groupPermissions = value; 
            }
        }

        FtpPermission m_otherPermissions = FtpPermission.None;
        /// <summary>
        /// Gets the others permissions. This property can be
        /// set however this functionality is intended to be done by
        /// custom parsers.
        /// </summary>
        public FtpPermission OthersPermissions {
            get { 
                return m_otherPermissions; 
            }
            set { 
                m_otherPermissions = value; 
            }
        }

        string m_input = null;
        /// <summary>
        /// Gets the input string that was parsed to generate the
        /// values in this object. This property can be
        /// set however this functionality is intended to be done by
        /// custom parsers.
        /// </summary>
        public string Input {
            get {
                return m_input;
            }
            private set {
                m_input = value;
            }
        }

        /// <summary>
        /// Returns a string representation of this object and its properties
        /// </summary>
        /// <returns>A string value</returns>
        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            foreach (System.Reflection.PropertyInfo p in GetType().GetProperties()) {
                sb.AppendLine(string.Format("{0}: {1}", p.Name, p.GetValue(this, null)));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Parses a line from a file listing using the first successful match in the Parsers collection.
        /// </summary>
        /// <param name="path">The source path of the file listing</param>
        /// <param name="buf">A line from the file listing</param>
        /// <param name="capabilities">Server capabilities</param>
        /// <returns>A FtpListItem object representing the parsed line, null if the line was
        /// unable to be parsed. If you have encountered an unsupported list type add a parser
        /// to the public static Parsers collection of FtpListItem.</returns>
        public static FtpListItem Parse(string path, string buf, FtpCapability capabilities) {
            if (buf != null && buf.Length > 0) {
                FtpListItem item;

                foreach (Parser parser in Parsers) {
                    if ((item = parser(buf, capabilities)) != null) {
                        item.FullName = path.GetFtpPath(item.Name);
                        item.Input = buf;
                        return item;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Used for synchronizing access to the Parsers collection
        /// </summary>
        static Mutex m_parserLock = new Mutex();

        static List<Parser> m_parsers = null;
        /// <summary>
        /// Collection of parsers. Each parser object contains
        /// a regex string that uses named groups, i.e., (?&lt;group_name&gt;foobar).
        /// The support group names are modify for last write time, size for the
        /// size and name for the name of the file system object. Each group name is
        /// optional, if they are present then those values are retrieved from a 
        /// successful match. In addition, each parser contains a Type property
        /// which gets set in the FtpListItem object to distinguish between different
        /// types of objects.
        /// </summary>
        static Parser[] Parsers {
            get {
                Parser[] parsers;

                try {
                    m_parserLock.WaitOne();

                    if (m_parsers == null) {
                        m_parsers = new List<Parser>();
                        m_parsers.Add(new Parser(ParseMachineList));
                        m_parsers.Add(new Parser(ParseUnixList));
                        m_parsers.Add(new Parser(ParseDosList));
                    }

                    parsers = m_parsers.ToArray();
                }
                finally {
                    m_parserLock.ReleaseMutex();
                }

                return parsers;
            }
        }

        

        /// <summary>
        /// Adds a custom parser
        /// </summary>
        /// <param name="parser">The parser delegate to add</param>
        /// <example><code source="..\Examples\CustomParser.cs" lang="cs" /></example>
        public static void AddParser(Parser parser) {
            try {
                m_parserLock.WaitOne();
                m_parsers.Add(parser);
            }
            finally {
                m_parserLock.ReleaseMutex();
            }
        }

        /// <summary>
        /// Removes all parser delegates
        /// </summary>
        public static void ClearParsers() {
            try {
                m_parserLock.WaitOne();
                m_parsers.Clear();
            }
            finally {
                m_parserLock.ReleaseMutex();
            }
        }

        /// <summary>
        /// Removes the specified parser
        /// </summary>
        /// <param name="parser">The parser delegate to remove</param>
        public static void RemoveParser(Parser parser) {
            try {
                m_parserLock.WaitOne();
                m_parsers.Remove(parser);
            }
            finally {
                m_parserLock.ReleaseMutex();
            }
        }

        /// <summary>
        /// Parses MLS* format listings
        /// </summary>
        /// <param name="buf">A line from the listing</param>
        /// <param name="capabilities">Server capabilities</param>
        /// <returns>FtpListItem if the item is able to be parsed</returns>
        static FtpListItem ParseMachineList(string buf, FtpCapability capabilities) {
            FtpListItem item = new FtpListItem();
            Match m;

            if (!(m = Regex.Match(buf, "^type=(?<type>.+?);", RegexOptions.IgnoreCase)).Success)
                return null;

            switch (m.Groups["type"].Value.ToLower()) {
                case "dir":
                    item.Type = FtpFileSystemObjectType.Directory;
                    break;
                case "file":
                    item.Type = FtpFileSystemObjectType.File;
                    break;
                // These are not supported for now.
                case "link":
                case "device":
                default:
                    return null;
            }

            if ((m = Regex.Match(buf, "; (?<name>.*)$", RegexOptions.IgnoreCase)).Success)
                item.Name = m.Groups["name"].Value;
            else // if we can't parse the file name there is a problem.
                return null;

            if ((m = Regex.Match(buf, "modify=(?<modify>.+?);", RegexOptions.IgnoreCase)).Success)
                item.Modified = m.Groups["modify"].Value.GetFtpDate();

            if ((m = Regex.Match(buf, "created?=(?<create>.+?);", RegexOptions.IgnoreCase)).Success)
                item.Created = m.Groups["create"].Value.GetFtpDate();

            if ((m = Regex.Match(buf, @"size=(?<size>\d+);", RegexOptions.IgnoreCase)).Success) {
                long size;

                if (long.TryParse(m.Groups["size"].Value, out size))
                    item.Size = size;
            }

            if((m = Regex.Match(buf, @"unix.mode=(?<mode>\d+);", RegexOptions.IgnoreCase)).Success) {
                if (m.Groups["mode"].Value.Length == 4) {
                    item.SpecialPermissions = (FtpSpecialPermissions)int.Parse(m.Groups["mode"].Value[0].ToString());
                    item.OwnerPermissions = (FtpPermission)int.Parse(m.Groups["mode"].Value[1].ToString());
                    item.GroupPermissions = (FtpPermission)int.Parse(m.Groups["mode"].Value[2].ToString());
                    item.OthersPermissions = (FtpPermission)int.Parse(m.Groups["mode"].Value[3].ToString());
                }
                else if(m.Groups["mode"].Value.Length == 3) {
                    item.OwnerPermissions = (FtpPermission)int.Parse(m.Groups["mode"].Value[0].ToString());
                    item.GroupPermissions = (FtpPermission)int.Parse(m.Groups["mode"].Value[1].ToString());
                    item.OthersPermissions = (FtpPermission)int.Parse(m.Groups["mode"].Value[2].ToString());
                }
            }

            return item;
        }

        /// <summary>
        /// Parses LIST format listings
        /// </summary>
        /// <param name="buf">A line from the listing</param>
        /// <param name="capabilities">Server capabilities</param>
        /// <returns>FtpListItem if the item is able to be parsed</returns>
        static FtpListItem ParseUnixList(string buf, FtpCapability capabilities) {
            string regex = 
                @"(?<permissions>[\w-]{10})\s+" +
                @"(?<objectcount>\d+)\s+" +
                @"(?<user>[\w\d]+)\s+" +
                @"(?<group>[\w\d]+)\s+" +
                @"(?<size>\d+)\s+" +
                @"(?<modify>\w+\s+\d+\s+\d+:\d+|\w+\s+\d+\s+\d+)\s+" +
                @"(?<name>.*)$";
            FtpListItem item = new FtpListItem();
            Match m;

            if (!(m = Regex.Match(buf, regex, RegexOptions.IgnoreCase)).Success)
                return null;

            if (m.Groups["permissions"].Value.StartsWith("d"))
                item.Type = FtpFileSystemObjectType.Directory;
            else if (m.Groups["permissions"].Value.StartsWith("-"))
                item.Type = FtpFileSystemObjectType.File;
            else
                return null;

            // if we can't determine a file name then
            // we are not considering this a successful parsing operation.
            if (m.Groups["name"].Value.Length < 1)
                return null;
            item.Name = m.Groups["name"].Value;

            if (item.Type == FtpFileSystemObjectType.Directory && (item.Name == "." || item.Name == ".."))
                return null;

            ////
            // Ignore the Modify times sent in LIST format for files
            // when the server has support for the MDTM command
            // because they will never be as accurate as what can be had 
            // by using the MDTM command. MDTM does not work on directories
            // so if a modify time was parsed from the listing we will try
            // to convert it to a DateTime object and use it for directories.
            ////
            if ((!capabilities.HasFlag(FtpCapability.MDTM) || item.Type == FtpFileSystemObjectType.Directory) && m.Groups["modify"].Value.Length > 0)
                item.Modified = m.Groups["modify"].Value.GetFtpDate();

            if (m.Groups["size"].Value.Length > 0) {
                long size;

                if (long.TryParse(m.Groups["size"].Value, out size))
                    item.Size = size;
            }

            if (m.Groups["permissions"].Value.Length > 0) {
                Match perms = Regex.Match(m.Groups["permissions"].Value, 
                    @"[\w-]{1}(?<owner>[\w-]{3})(?<group>[\w-]{3})(?<others>[\w-]{3})",
                    RegexOptions.IgnoreCase);

                if (perms.Success) {
                    if (perms.Groups["owner"].Value.Length == 3) {
                        if (perms.Groups["owner"].Value[0] == 'r')
                            item.OwnerPermissions |= FtpPermission.Read;
                        if (perms.Groups["owner"].Value[1] == 'w')
                            item.OwnerPermissions |= FtpPermission.Write;
                        if (perms.Groups["owner"].Value[2] == 'x' || perms.Groups["owner"].Value[2] == 's')
                            item.OwnerPermissions |= FtpPermission.Execute;
                        if (perms.Groups["owner"].Value[2] == 's' || perms.Groups["owner"].Value[2] == 'S')
                            item.SpecialPermissions |= FtpSpecialPermissions.SetUserID;
                    }

                    if (perms.Groups["group"].Value.Length == 3) {
                        if (perms.Groups["group"].Value[0] == 'r')
                            item.GroupPermissions |= FtpPermission.Read;
                        if (perms.Groups["group"].Value[1] == 'w')
                            item.GroupPermissions |= FtpPermission.Write;
                        if (perms.Groups["group"].Value[2] == 'x' || perms.Groups["group"].Value[2] == 's')
                            item.GroupPermissions |= FtpPermission.Execute;
                        if (perms.Groups["group"].Value[2] == 's' || perms.Groups["group"].Value[2] == 'S')
                            item.SpecialPermissions |= FtpSpecialPermissions.SetGroupID;
                    }

                    if (perms.Groups["others"].Value.Length == 3) {
                        if (perms.Groups["others"].Value[0] == 'r')
                            item.OthersPermissions |= FtpPermission.Read;
                        if (perms.Groups["others"].Value[1] == 'w')
                            item.OthersPermissions |= FtpPermission.Write;
                        if (perms.Groups["others"].Value[2] == 'x' || perms.Groups["others"].Value[2] == 't')
                            item.OthersPermissions |= FtpPermission.Execute;
                        if (perms.Groups["others"].Value[2] == 't' || perms.Groups["others"].Value[2] == 'T')
                            item.SpecialPermissions |= FtpSpecialPermissions.Sticky;
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Parses IIS DOS format listings
        /// </summary>
        /// <param name="buf">A line from the listing</param>
        /// <param name="capabilities">Server capabilities</param>
        /// <returns>FtpListItem if the item is able to be parsed</returns>
        static FtpListItem ParseDosList(string buf, FtpCapability capabilities) {
            FtpListItem item = new FtpListItem();
            Match m;

            // directory
            if ((m = Regex.Match(buf, @"(?<modify>\d+-\d+-\d+\s+\d+:\d+\w+)\s+<DIR>\s+(?<name>.*)$", RegexOptions.IgnoreCase)).Success) {
                DateTime modify;

                item.Type = FtpFileSystemObjectType.Directory;
                item.Name = m.Groups["name"].Value;

                if (DateTime.TryParse(m.Groups["modify"].Value, out modify))
                    item.Modified = modify;
            }
            // file
            else if ((m = Regex.Match(buf, @"(?<modify>\d+-\d+-\d+\s+\d+:\d+\w+)\s+(?<size>\d+)\s+(?<name>.*)$", RegexOptions.IgnoreCase)).Success) {
                DateTime modify;
                long size;

                item.Type = FtpFileSystemObjectType.File;
                item.Name = m.Groups["name"].Value;

                if (long.TryParse(m.Groups["size"].Value, out size))
                    item.Size = size;

                if (DateTime.TryParse(m.Groups["modify"].Value, out modify))
                    item.Modified = modify;
            }
            else
                return null;

            return item;
        }

        /// <summary>
        /// Ftp listing line parser
        /// </summary>
        /// <param name="line">The line from the listing</param>
        /// <param name="capabilities">The server capabilities</param>
        /// <returns>FtpListItem if the line can be parsed, null otherwise</returns>
        public delegate FtpListItem Parser(string line, FtpCapability capabilities);
    }
}
