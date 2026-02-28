using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Ganss.Xss;

namespace Anatini.Server.Utils
{
    public class HtmlValidationResult
    {
        public string? ErrorMessage { get; set; }
        public string? SanitizedHtml { get; set; }
    }

    public static class HtmlContentService
    {
        public static HtmlValidationResult ValidateAndNormalizeHtml(string input)
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(input);

            if (document.Body == null)
            {
                return new HtmlValidationResult
                {
                    ErrorMessage = "Unknown error"
                };
            }

            NormalizeTags(document);

            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Clear();

            var allowed = new HashSet<string> { "article", "p", "em", "strong" };
            foreach (var tag in allowed) sanitizer.AllowedTags.Add(tag);

            sanitizer.AllowedAttributes.Clear();

            var sanitized = sanitizer.Sanitize(document.Body.InnerHtml);

            var bodyChildren = document.Body.Children;

            if (bodyChildren.Length != 1 || !bodyChildren[0].TagName.Equals("article", StringComparison.CurrentCultureIgnoreCase))
            {
                return new HtmlValidationResult
                {
                    ErrorMessage = "The HTML must be wrapped in a single <article> tag."
                };
            }

            var article = bodyChildren[0];

            foreach (var child in article.Children)
            {
                if (!child.TagName.Equals("p", StringComparison.CurrentCultureIgnoreCase))
                {
                    return new HtmlValidationResult
                    {
                        ErrorMessage = $"Top-level elements inside <article> must be <p>. Found <{child.TagName.ToLower()}>."
                    };
                }

                foreach (var nested in child.Children)
                {
                    if (!nested.TagName.Equals("em", StringComparison.CurrentCultureIgnoreCase) && !nested.TagName.Equals("strong", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return new HtmlValidationResult
                        {
                            ErrorMessage = $"Invalid nested tag <{nested.TagName.ToLower()}> inside <p>."
                        };
                    }
                }
            }

            return new HtmlValidationResult
            {
                SanitizedHtml = sanitized
            };
        }

        private static void NormalizeTags(IHtmlDocument document)
        {
            var boldTags = document.QuerySelectorAll("b");
            foreach (var b in boldTags)
            {
                var strong = document.CreateElement("strong");
                strong.InnerHtml = b.InnerHtml;
                b.Replace(strong);
            }

            var italicTags = document.QuerySelectorAll("i");
            foreach (var i in italicTags)
            {
                var em = document.CreateElement("em");
                em.InnerHtml = i.InnerHtml;
                i.Replace(em);
            }
        }
    }
}
