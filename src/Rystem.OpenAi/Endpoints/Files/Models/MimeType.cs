namespace Rystem.OpenAi
{
    public sealed class MimeType
    {
        public MimeType(string name)
        {
            Name = name;
        }
        public string Name { get; private set; }
        public static implicit operator string(MimeType name)
            => name.Name;
        public static implicit operator MimeType(string name)
            => new(name);
        public override string ToString()
            => Name;
        /// <summary>
        /// MIME type for C source files.
        /// </summary>
        public static MimeType C { get; } = "text/x-c";

        /// <summary>
        /// MIME type for C++ source files.
        /// </summary>
        public static MimeType Cpp { get; } = "text/x-c++";

        /// <summary>
        /// MIME type for Comma-Separated Values (CSV) files.
        /// </summary>
        public static MimeType Csv { get; } = "application/csv";

        /// <summary>
        /// MIME type for Microsoft Word (.docx) documents.
        /// </summary>
        public static MimeType Docx { get; } = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        /// <summary>
        /// MIME type for HyperText Markup Language (.html) files.
        /// </summary>
        public static MimeType Html { get; } = "text/html";

        /// <summary>
        /// MIME type for Java source files.
        /// </summary>
        public static MimeType Java { get; } = "text/x-java";

        /// <summary>
        /// MIME type for JavaScript Object Notation (.json) files.
        /// </summary>
        public static MimeType Json { get; } = "application/json";

        /// <summary>
        /// MIME type for Markdown (.md) files.
        /// </summary>
        public static MimeType Markdown { get; } = "text/markdown";

        /// <summary>
        /// MIME type for Portable Document Format (.pdf) files.
        /// </summary>
        public static MimeType Pdf { get; } = "application/pdf";

        /// <summary>
        /// MIME type for PHP source files.
        /// </summary>
        public static MimeType Php { get; } = "text/x-php";

        /// <summary>
        /// MIME type for Microsoft PowerPoint (.pptx) presentations.
        /// </summary>
        public static MimeType Pptx { get; } = "application/vnd.openxmlformats-officedocument.presentationml.presentation";

        /// <summary>
        /// MIME type for Python source files (.py).
        /// </summary>
        public static MimeType Python { get; } = "text/x-python";

        /// <summary>
        /// Alternate MIME type for Python scripts.
        /// </summary>
        public static MimeType PythonScript { get; } = "text/x-script.python";

        /// <summary>
        /// MIME type for Ruby source files.
        /// </summary>
        public static MimeType Ruby { get; } = "text/x-ruby";

        /// <summary>
        /// MIME type for TeX (.tex) files.
        /// </summary>
        public static MimeType Tex { get; } = "text/x-tex";

        /// <summary>
        /// MIME type for plain text (.txt) files.
        /// </summary>
        public static MimeType Text { get; } = "text/plain";

        /// <summary>
        /// MIME type for Cascading Style Sheets (.css) files.
        /// </summary>
        public static MimeType Css { get; } = "text/css";

        /// <summary>
        /// MIME type for JPEG (.jpeg) image files.
        /// </summary>
        public static MimeType Jpeg { get; } = "image/jpeg";

        /// <summary>
        /// MIME type for JPEG (.jpg) image files (alternative).
        /// </summary>
        public static MimeType Jpg { get; } = "image/jpeg";

        /// <summary>
        /// MIME type for JavaScript (.js) files.
        /// </summary>
        public static MimeType Javascript { get; } = "text/javascript";

        /// <summary>
        /// MIME type for Graphics Interchange Format (.gif) image files.
        /// </summary>
        public static MimeType Gif { get; } = "image/gif";

        /// <summary>
        /// MIME type for PNG (.png) image files.
        /// </summary>
        public static MimeType Png { get; } = "image/png";

        /// <summary>
        /// MIME type for TAR (.tar) archive files.
        /// </summary>
        public static MimeType Tar { get; } = "application/x-tar";

        /// <summary>
        /// MIME type for TypeScript (.ts) files.
        /// </summary>
        public static MimeType TypeScript { get; } = "application/typescript";

        /// <summary>
        /// MIME type for Microsoft Excel (.xlsx) spreadsheet files.
        /// </summary>
        public static MimeType Xlsx { get; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        /// <summary>
        /// MIME type for Extensible Markup Language (.xml) files.
        /// </summary>
        public static MimeType Xml { get; } = "application/xml"; // Alternatively, "text/xml"

        /// <summary>
        /// MIME type for ZIP (.zip) archive files.
        /// </summary>
        public static MimeType Zip { get; } = "application/zip";
    }
}
